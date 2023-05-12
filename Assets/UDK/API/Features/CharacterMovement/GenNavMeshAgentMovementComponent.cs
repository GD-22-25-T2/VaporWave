namespace UDK.API.Features.CharacterMovement
{
    using UnityEngine;

    /// <summary>
    /// The base Nav Mesh agent movement component which implements a set of tools to build a specific movement configuration.
    /// <para>
    /// The <see cref="GenRootCollisionComponent"/> is required to run this component, as it handles the pawn collision.
    /// </para>
    /// </summary>
    [AddComponentMenu("UDK/CharacterMovement/GenNavMeshAgentMovementComponent")]
    [RequireComponent(typeof(GenNavMeshAgentComponent))]
    public class GenNavMeshAgentMovementComponent : GenMovementComponent
    {
        /// <summary>
        /// Gets or sets the <see cref="GenNavMeshAgentComponent"/>.
        /// </summary>
        public GenNavMeshAgentComponent NavMeshAgent { get; protected set; }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            SubscribeEvents();
            OnLanded();
        }

        /// <inheritdoc/>
        protected override void InitializeBehaviours()
        {
            SkeletalMesh = GetComponentSafe<Renderer>();
            Rigidbody = GetComponentSafe<Rigidbody>();
        }

        /// <inheritdoc/>
        protected override void DefineInputBindings()
        {
        }

        /// <inheritdoc/>
        protected override void BindInputActions()
        {
        }

        /// <inheritdoc/>
        protected override void PreMovementUpdate(float deltaTime) => RayTracingMode = rayTracingMode;

        /// <inheritdoc/>
        protected override void FixedMovementUpdate(float fixedDeltaTime)
        {
            if (SpeedXZ > MaxSpeed)
            {
                Vector3 velNormal = Velocity.normalized;
                Velocity = new(velNormal.x * MaxSpeed, Velocity.y, velNormal.z * MaxSpeed);
            }

            Acceleration = (Velocity - VelocityLastFrame) / fixedDeltaTime;
            if (AccelerationMagnitude > MaxAcceleration)
                Acceleration = Acceleration.normalized * MaxAcceleration;

            VelocityLastFrame = Velocity;

            if (IsHovering)
            {
                if (UseHooksLaw && Velocity.y > MaxHoveringPitchVelocity)
                {
                    Vector3 velNormal = Velocity.normalized;
                    Velocity = new(Velocity.x, MaxHoveringPitchVelocity, Velocity.z);
                }

                ApplyHoveringForce(UseHooksLaw);
            }
        }

        /// <inheritdoc/>
        protected override void ProcessInputVector(float deltaTime)
        {
        }
    }
}

