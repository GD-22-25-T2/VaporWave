namespace UDK.API.Features.CharacterMovement
{
    using UDK.API.Features.Core;
    using UDK.API.Features.Input;
    using UnityEngine;

    /// <summary>
    /// The base camera component which implements a set of tools to build a specific camera configuration.
    /// <para>
    /// A <see cref="Camera"/> is wrapped and used as the base value, in order to avoid conflicts overriding the base <see cref="Camera"/> component.
    /// </para>
    /// <para>
    /// The <see cref="GenMovementComponent"/> is required to run this component as it needs pawn movement data.
    /// </para>
    /// <para>
    /// <see cref="SpringArmDistance"/> and <see cref="SpringArmHeight"/>
    /// <br>will be deprecated in the next major version (2.0.0) in favor of using the SpringArmComponent.</br>
    /// </para>
    /// <para>
    /// <see cref="GenCameraComponent"/> allows conflicts between different input orientation modes, whereas <see cref="GenMovementComponent"/> doesn't.
    /// <br>Be careful when setting parameters and using multiple orientation modes as they're not intended to be used in that way;</br>
    /// <br>but still, it's possible to do so.</br>
    /// </para>
    /// </summary>
    [AddComponentMenu("UDK/CharacterMovement/GenCameraComponent")]
    public class GenCameraComponent : PawnComponent
    {
        #region Editor

        [Space(5)]
        [Header("Camera Settings")]

        [SerializeField]
        protected Transform cameraTransform;

        [SerializeField]
        protected bool borderMode;

        [SerializeField]
        protected bool pitchMode;

        [SerializeField]
        protected float springArmDistance;

        [SerializeField]
        protected float springArmHeight;

        [SerializeField]
        protected Vector3 adjustPositionOffset;

        [SerializeField]
        protected Vector3 adjustRotationOffset;

        [Space(5)]
        [Header("Orientation Settings")]

        [SerializeField]
        protected bool orientToMovement;

        [SerializeField]
        protected bool orientToRotation;

        [SerializeField]
        protected bool orientToInputDirection;

        [SerializeField]
        protected float orientToInputDamp;

        [Space(5)]
        [Header("Sensitivity Settings")]

        [SerializeField]
        protected bool usePitch;

        [SerializeField]
        protected bool useYaw;

        [SerializeField]
        protected float yawSensitivity;

        [SerializeField]
        protected float pitchSensitivity;

        [SerializeField]
        protected float orientToMovementDamp;

        #endregion

        #region BackingFields

        protected float yaw;
        protected Vector3 orientToRotation_Pos_CE_Internal;
        protected Vector3 orientToRotation_Quat_CE_Internal;
        protected Vector3 orientToMovement_CE_Internal;
        protected Vector3 orientToInputDirection_CE_Internal;

        #endregion

        /// <inheritdoc/>
        public override Transform Transform => Base.transform;

        /// <summary>
        /// Gets the base <see cref="Camera"/>.
        /// </summary>
        public Camera Base { get; protected set; }

        /// <summary>
        /// Gets the <see cref="GenMovementComponent"/>.
        /// </summary>
        public GenMovementComponent PlayerController { get; protected set; }

        /// <summary>
        /// Gets or sets the parent transform.
        /// </summary>
        public Transform ParentTransform { get; set; }

        /// <summary>
        /// Gets or sets the spring arm distance.
        /// </summary>
        public float SpringArmDistance
        {
            get => springArmDistance;
            set => springArmDistance = value;
        }

        /// <summary>
        /// Gets or sets the spring arm height.
        /// </summary>
        public float SpringArmHeight
        {
            get => springArmHeight;
            set => springArmHeight = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the camera should be oriented to the input direction.
        /// </summary>
        public bool OrientToInputDirection
        {
            get => orientToInputDirection;
            set
            {
                orientToInputDirection = value;

                if (value)
                    OrientToMovement = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the camera should be oriented to the movement.
        /// </summary>
        public bool OrientToMovement
        {
            get => orientToMovement;
            set
            {
                orientToMovement = value;

                if (value)
                    OrientToInputDirection = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the camera should be oriented to the rotation.
        /// </summary>
        public bool OrientToRotation
        {
            get => orientToRotation;
            set
            {
                if (!value)
                {
                    orientToRotation_Pos_CE_Internal = Position;
                    orientToRotation_Quat_CE_Internal = Rotation.eulerAngles;
                }
                else
                {
                    Position = orientToRotation_Pos_CE_Internal;
                    Rotation = Quaternion.Euler(orientToRotation_Quat_CE_Internal);
                }

                orientToRotation = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the border mode is enabled.
        /// </summary>
        public bool IsBorderMode
        {
            get => borderMode;
            set
            {
                borderMode = value;

                if (value)
                    IsPitchMode = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pitch mode is enabled.
        /// </summary>
        public bool IsPitchMode
        {
            get => pitchMode;
            set
            {
                pitchMode = value;

                if (value)
                    IsBorderMode = false;
            }
        }

        /// <summary>
        /// Gets or sets the yaw value.
        /// </summary>
        public float Yaw
        {
            get => yaw;
            protected set => yaw = value;
        }

        /// <summary>
        /// Gets or sets the pitch value.
        /// </summary>
        public float Pitch { get; protected set; }

        /// <summary>
        /// Gets or sets the yaw sensitivity.
        /// </summary>
        public float YawSensitivity { get; protected set; }

        /// <summary>
        /// Gets or sets the pitch sensitivity.
        /// </summary>
        public float PitchSensitivity { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pitch should be inverted.
        /// </summary>
        public bool InvertPitch { get; set; }

        /// <inheritdoc cref="YawSensitivity"/>
        public float MouseXSensitivity
        {
            get => YawSensitivity;
            set => YawSensitivity = value;
        }

        /// <inheritdoc cref="PitchSensitivity"/>
        public float MouseYSensitivity
        {
            get => PitchSensitivity;
            set => PitchSensitivity = value;
        }

        /// <inheritdoc cref="InvertPitch"/>
        public bool InvertYAxis
        {
            get => InvertPitch;
            set => InvertPitch = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pitch should be used.
        /// </summary>
        public bool UsePitch
        {
            get => usePitch;
            set => usePitch = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the yaw should be used.
        /// </summary>
        public bool UseYaw
        {
            get => useYaw;
            set => useYaw = value;
        }

        /// <inheritdoc cref="Yaw"/>
        public float MouseX => Yaw;

        /// <inheritdoc cref="Pitch"/>
        public float MouseY => Pitch;

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            Base = cameraTransform ?
                GetComponentSafe<Camera>(cameraTransform.gameObject) :
                GetComponentSafe<Camera>(Transform.parent.gameObject);

            PlayerController = GetComponentSafe<GenMovementComponent>();
            ParentTransform = PlayerController.Transform;
        }

#if UNITY_EDITOR
        /// <inheritdoc/>
        protected override void OnValidating()
        {
            base.OnValidating();

            Base = Base ?? (cameraTransform ? GetComponentSafe<Camera>(cameraTransform.gameObject) : GetComponentSafe<Camera>(Transform.parent.gameObject));
            PlayerController = PlayerController ?? GetComponentSafe<GenMovementComponent>();
        }
#endif

        /// <inheritdoc/>
        protected override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            Vector3 parentPosition = ParentTransform.position + adjustPositionOffset;
            Position = IsBorderMode ? Vector3.Slerp(Position, ParentTransform.position + adjustPositionOffset, Time.deltaTime) :
                IsPitchMode ? new(parentPosition.x, Mathf.Lerp(Position.y, parentPosition.y, Time.deltaTime), parentPosition.z) :
                parentPosition;

            Yaw += InputBinding.GetAxis("Mouse X");
            Pitch = Mathf.Clamp(InvertPitch ? Pitch + InputBinding.GetAxis("Mouse Y") : Pitch - InputBinding.GetAxis("Mouse Y"), -90f, 90f);

            if (OrientToInputDirection)
            {
                Position = ParentTransform.position - (ParentTransform.forward * SpringArmDistance);
                Transform.LookAt(ParentTransform.position);
                Position = new(Position.x, Position.y + SpringArmHeight, Position.z);
                return;
            }
            else if (OrientToMovement)
            {
                Vector3 direction = new(PlayerController.Vertical, PlayerController.Horizontal, 0f);
                Transform.Rotate(orientToMovementDamp * Time.deltaTime * direction);
            }
            else
            {
                Rotation = OrientToRotation ?
                    Quaternion.Euler(UsePitch ? Pitch : Rotation.eulerAngles.x, UseYaw ? Yaw : Rotation.eulerAngles.y, Rotation.eulerAngles.z) :
                    Quaternion.Euler(ParentTransform.rotation.eulerAngles + adjustRotationOffset);
            }
        }
    }
}
