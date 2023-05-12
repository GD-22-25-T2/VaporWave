namespace UDK.API.Features.CharacterMovement
{
    using UDK.API.Features.Core;
    using UnityEngine;

    /// <summary>
    /// The base component which handles IK (inverse kinematics).
    /// <para>
    /// <see cref="GenIKComponent"/> provides a basic implementation for IK solvers, they may be extended in order to improve IK solving and make it more accurate.
    /// <br>An IK API will be eventually implemented in the next major version (2.0.0).</br>
    /// </para>
    /// <para>
    /// The current implementation is not network replicated.
    /// </para>
    /// <para>
    /// It's not strictly dependent on <see cref="GenMovementComponent"/> as the code base refers to it to get the walkable <see cref="LayerMask"/>,
    /// <br>although, CharacterMovement components should strictly depend on each other due to how movement-related features are being implemented.</br>
    /// </para>
    /// <para>
    /// Even if it's not recommended, unless movement is already handled by a non-CharacterMovement component and it's safe interacting with it,
    /// <br>all the base methods and their implementation can be overridden in order to adapt it to the current movement system, especially on the animation side.</br>
    /// </para>
    /// </summary>
    [AddComponentMenu("UDK/CharacterMovement/GenIKComponent")]
    [RequireComponent(typeof(GenMovementComponent))]
    public class GenIKComponent : PawnComponent
    {
        #region Editor

        [Space(5)]
        [Header("IK Ground Evaluation")]

        [SerializeField]
        protected bool footIK;

        [SerializeField]
        protected bool depthIKEvaluation;

        [SerializeField]
        protected bool visibleSolver;

        [Space(5)]
        [Header("IK Parameters")]

        [Range(0, 2)]
        [SerializeField]
        protected float maxLineTraceHeight = 1.14f;

        [Range(0, 2)]
        [SerializeField]
        protected float naxInverseLineTraceHeight = 1.5f;

        [SerializeField]
        protected float pelvisOffset = 0f;

        [Range(0, 1)]
        [SerializeField]
        protected float pelvisAxisSpeed = 0.05f;

        [Range(0, 1)]
        [SerializeField]
        protected float targetIKSpeed = 0.05f;

        [SerializeField]
        protected string leftFoot;

        [SerializeField]
        protected string rightFoot;

        #endregion

        #region BackingFields

        protected Vector3 rtFootPosition, rtFootIKPosition;
        protected Vector3 ltFootPosition, ltFootIKPosition;
        protected Quaternion ltFootIKRotation, rtFootIKRotation;
        protected float lastPelvisPitch, lastRtFootPitch, lastLtFootPitch;

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="UnityEngine.Animator"/>.
        /// </summary>
        public Animator Animator { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="GenMovementComponent"/>.
        /// </summary>
        public GenMovementComponent MovementComponent { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the IK solver should be displayed.
        /// </summary>
        public bool IsSolverVisible
        {
            get => visibleSolver;
            protected set => visibleSolver = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the foor IK solver is enabled.
        /// </summary>
        public bool IsFootIKEnabled
        {
            get => footIK;
            protected set => footIK = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the depth IK evaluation is enabled.
        /// </summary>
        public bool IsDepthIKEvaluationEnabled
        {
            get => depthIKEvaluation;
            protected set => depthIKEvaluation = value;
        }

        /// <summary>
        /// Gets or sets the maximum line trace height.
        /// </summary>
        public float MaxLineTraceHeight
        {
            get => maxLineTraceHeight;
            protected set => maxLineTraceHeight = value;
        }

        /// <summary>
        /// Gets or sets the maximum line trace height.
        /// </summary>
        public float MaxInverseLineTraceHeight
        {
            get => naxInverseLineTraceHeight;
            protected set => naxInverseLineTraceHeight = value;
        }

        /// <summary>
        /// Gets or sets the pelvis offset.
        /// </summary>
        public float PelvisOffset
        {
            get => pelvisOffset;
            protected set => pelvisOffset = value;
        }

        /// <summary>
        /// Gets or sets the pelvis axis speed.
        /// </summary>
        public float PelvisAxisSpeed
        {
            get => pelvisAxisSpeed;
            protected set => pelvisAxisSpeed = value;
        }

        /// <summary>
        /// Gets or sets the target IK speed.
        /// </summary>
        public float TargetIKSpeed
        {
            get => targetIKSpeed;
            protected set => targetIKSpeed = value;
        }

        /// <summary>
        /// Gets or sets the left foot animation variable.
        /// </summary>
        public string LeftFoot
        {
            get => leftFoot;
            protected set => leftFoot = value;
        }

        /// <summary>
        /// Gets or sets the right foot animation variable.
        /// </summary>
        public string RightFoot
        {
            get => rightFoot;
            protected set => rightFoot = value;
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            MovementComponent = GetComponent<GenMovementComponent>();
            Animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Fired when setting up inverse kinematics.
        /// </summary>
        /// <param name="layerIndex">The layer index.</param>
        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (!Animator || !IsFootIKEnabled)
                return;

            SolveIKPelvis();
            Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);

            if (IsDepthIKEvaluationEnabled)
                Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, Animator.GetFloat(RightFoot));

            PlaceFeetToIKTarget(AvatarIKGoal.RightFoot, rtFootIKPosition, rtFootIKRotation, ref lastRtFootPitch);

            Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);

            if (IsDepthIKEvaluationEnabled)
                Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, Animator.GetFloat(LeftFoot));

            PlaceFeetToIKTarget(AvatarIKGoal.LeftFoot, ltFootIKPosition, ltFootIKRotation, ref lastLtFootPitch);
        }

        /// <summary>
        /// Places the feet to the IK target.
        /// </summary>
        /// <param name="footIKGoal">The foot IK goal.</param>
        /// <param name="positionIKHolder">The IK holder position.</param>
        /// <param name="rotationIKHolder">The IK holder rotation</param>
        /// <param name="lastFootPitch">The last foot pitch.</param>
        protected virtual void PlaceFeetToIKTarget(AvatarIKGoal footIKGoal, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPitch)
        {
            Vector3 targetIKPosition = Animator.GetIKPosition(footIKGoal);

            if (positionIKHolder != default)
            {
                targetIKPosition = Transform.InverseTransformPoint(targetIKPosition);
                positionIKHolder = Transform.InverseTransformPoint(positionIKHolder);

                float targetPitch = Mathf.Lerp(lastFootPitch, positionIKHolder.y, TargetIKSpeed);
                targetIKPosition.y += targetPitch;
                lastFootPitch = targetPitch;
                targetIKPosition = transform.TransformPoint(targetIKPosition);

                Animator.SetIKRotation(footIKGoal, rotationIKHolder);
            }

            Animator.SetIKPosition(footIKGoal, targetIKPosition);
        }

        /// <summary>
        /// Solves the pelvis IK properties, such as the height.
        /// </summary>
        protected virtual void SolveIKPelvis()
        {
            if (rtFootIKPosition == default || ltFootIKPosition == default || lastPelvisPitch == 0f)
            {
                lastPelvisPitch = Animator.bodyPosition.y;
                return;
            }

            float lOffsetPosition = ltFootIKPosition.y - transform.position.y;
            float rOffsetPosition = rtFootIKPosition.y - transform.position.y;
            float absOffset = lOffsetPosition < rOffsetPosition ? lOffsetPosition : rOffsetPosition;

            Vector3 nextPelvisPos = Animator.bodyPosition + Vector3.up * absOffset;
            nextPelvisPos.y = Mathf.Lerp(lastPelvisPitch, nextPelvisPos.y, PelvisAxisSpeed);
            Animator.bodyPosition = nextPelvisPos;
            lastPelvisPitch = Animator.bodyPosition.y;
        }

        /// <summary>
        /// Solves the feet IK properties, such as position and rotation.
        /// </summary>
        /// <param name="upwardPosition">The upward position pointing to the ground.</param>
        /// <param name="feetIKPosition">The feet IK position reference.</param>
        /// <param name="feetIKRotation">The feet IK rotation reference.</param>
        protected virtual void SolveIKFeet(Vector3 upwardPosition, ref Vector3 feetIKPosition, ref Quaternion feetIKRotation)
        {
            RaycastHit feetOutHit;

            if (IsSolverVisible)
                Debug.DrawLine(upwardPosition, upwardPosition + Vector3.down * (MaxInverseLineTraceHeight + MaxLineTraceHeight), Color.red);

            if (Physics.Raycast(upwardPosition, Vector3.down, out feetOutHit, MaxInverseLineTraceHeight + MaxLineTraceHeight, MovementComponent.WalkableMask))
            {
                feetIKPosition = upwardPosition;
                feetIKPosition.y = feetOutHit.point.y + PelvisOffset;
                feetIKRotation = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
                return;
            }

            feetIKPosition = default;
        }

        /// <summary>
        /// Adjusts the feet target bone.
        /// </summary>
        /// <param name="feetPosition">The current feet position.</param>
        /// <param name="foot">The foot bone.</param>
        protected virtual void AdjustFeetTargetBone(ref Vector3 feetPosition, HumanBodyBones foot)
        {
            feetPosition = Animator.GetBoneTransform(foot).position;
            feetPosition.y = transform.position.y + MaxLineTraceHeight;
        }
    }
}
