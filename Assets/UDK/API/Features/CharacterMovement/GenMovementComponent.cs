namespace UDK.API.Features.CharacterMovement
{
    using UDK.API.Features.Input;
    using UnityEngine;
    using MEC;
    using UDK.API.Features.Core;
    using UnityEngine.Rendering;
    using UnityEngine.Experimental.Rendering;
    using System;
    using UDK.Events.DynamicEvents;
    using System.Collections.Generic;
    using System.Linq;
    using Cursor = UnityEngine.Cursor;
    using UDK.API.Features.Entity;
    using UDK.API.Features.Entity.Roles.Modifiers;
    using UDK.API.Features.Enums;
    using UDK.API.Features.StatusEffects;
    using UDK.API.Features.Core.Framework;

    /// <summary>
    /// The base movement component which implements a set of tools to build a specific movement configuration.
    /// <br>It allows switching between multiple input orientation modes, affecting the movement in a considerable way.</br>
    /// <para>
    /// The <see cref="GenCameraComponent"/> is required to run this component, as it handles the pawn camera.
    /// </para>
    /// <para>
    /// The <see cref="Features.Input.InputActionComponent"/> is required to run this component, as it handles the pawn input.
    /// </para>
    /// <para>
    /// The <see cref="GenRootCollisionComponent"/> is required to run this component, as it handles the pawn collision.
    /// </para>
    /// <para>
    /// The <see cref="Renderer"/> handles the skeletal mesh of the pawn.
    /// <br>No additional features are included within the base <see cref="Renderer"/>.</br>
    /// <br>A SkeletalMeshComponent will be added in the future, in order to easily manage the base component class, with a few more tools and features.</br>
    /// <br>Currently, only the <see cref="UseFirstPersonPerspective"/> property is handling some properties of the <see cref="Renderer"/>.</br>
    /// </para>
    /// <para>
    /// Unity physics is supported; although, it's recommended using already implemented features within the <see cref="GenMovementComponent"/>
    /// <br>in order to avoid conflicts with the internal and the native calculation and evaultation operations.</br>
    /// <br>Friction and angular friction are fully supported and they also support negative values, whereas Unity's physics won't allow to do so.</br>
    /// <br>Friction will affect the <see cref="Velocity"/>; angular friction will affect the <see cref="Torque"/>.</br>
    /// <br>Unity's physics simulation will be eventually rewritten in order to gain full control over it, thus dropping the native support.</br>
    /// </para>
    /// <para>
    /// <see cref="GenMovementComponent"/> isn't currently network replicated.
    /// <br>Network replication will be implemented in the next major version (2.0.0).</br>
    /// </para>
    /// <para>A built-in subsystem is running over the <see cref="MonoBehaviour"/>'s one, it's recommended to not override nor directly interact with <see cref="MonoBehaviour"/>'s base methods.</para>
    /// </summary>
    [AddComponentMenu("UDK/CharacterMovement/GenMovementComponent")]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class GenMovementComponent : PlayerController
    {
        #region Editor

        [Space(5)]
        [Header("Physics Settings")]

        [SerializeField]
        protected bool isHovering;

        [SerializeField]
        protected bool useHooksLaw;

        [SerializeField]
        protected float maxFloorTracelineHeight;

        [SerializeField]
        protected float maxFloorDistance;

        [SerializeField]
        protected float defaultMaxSpeed;

        [SerializeField]
        protected float currentSpeed;

        [SerializeField]
        protected float maxAcceleration;

        [SerializeField]
        protected float accelerationRate = 3f;

        [SerializeField]
        protected float defaultFriction = 0.1f;

        [SerializeField]
        protected float defaultAngularFriction = 0.1f;

        [SerializeField]
        protected float groundedFriction;

        [SerializeField]
        protected float airborneFriction;

        [SerializeField]
        protected float groundedAngularFriction;

        [SerializeField]
        protected float airborneAngularFriction;

        [SerializeField]
        protected float bouncingCollisionForce;

        [SerializeField]
        protected float maxHoveringDistance;

        [SerializeField]
        protected float hooksLawStrength;

        [SerializeField]
        protected float maxHoveringPitchVelocity;

        [SerializeField, Range(0, 10)]
        protected int activeThrusters;

        [Space(5)]
        [Header("Orientation Settings")]

        [SerializeField]
        protected bool orientToMovement;

        [SerializeField]
        protected bool orientToRotation;

        [SerializeField]
        protected bool orientToInputDirection;

        [SerializeField]
        protected bool orientToAnalog;

        [SerializeField]
        protected bool useFirstPersonPerspective;

        [Space(5)]
        [Header("Collision Settings")]

        [SerializeField]
        protected float rootCollisionHalfHeight;

        [SerializeField]
        protected float rootCollisionRadius;

        [SerializeField]
        protected Vector3 rootCollisionExtent;

        [Space(5)]
        [Header("Axis Settings")]

        [SerializeField]
        protected bool invertAxis;

        [SerializeField]
        protected bool useRotationSensitivity;

        [SerializeField, Range(0f, 100f)]
        protected float rotationDamp = 0.1f;

        [SerializeField]
        protected float yawSensitivity;

        [SerializeField]
        protected float analogSensitivity;

        [Space(5)]
        [Header("Actions Settings")]

        [SerializeField]
        protected float sprintSpeed;

        [SerializeField]
        protected float airborneSpeed;

        [SerializeField]
        protected float jumpForce;

        [SerializeField]
        protected float jumpRate;

        [SerializeField]
        protected float hoveringJumpPitchVelocity;

        [Space(5)]
        [Header("Layer Masks")]

        [SerializeField]
        protected LayerMask walkableMask;

        [SerializeField]
        protected LayerMask bouncingCollisionMask;

        [Space(5)]
        [Header("Experimental")]

        [SerializeField]
        protected RayTracingMode rayTracingMode;

        #endregion

        #region BackingFields
#if DEBUG
        protected bool isPaused;
#endif
        protected sbyte movementState;
        protected RaycastHit floorHit;
        protected bool canJump, canSprint;
        protected bool isJumpAvailable;
        protected float ref_rotationRate;
        protected float ref_accel_velX, ref_accel_velZ;
        protected float ref_velX, ref_velZ;
        protected float hooksLastHitDistance;
        protected float lastPitchVelocity;
        protected List<GameObject> cachedThrusters;

        #endregion

        /// <summary>
        /// Gets the <see cref="GenCameraComponent"/>.
        /// </summary>
        public GenCameraComponent Camera { get; protected set; }

        /// <summary>
        /// Gets the <see cref="MeshRenderer"/>.
        /// </summary>
        public Renderer SkeletalMesh { get; protected set; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.Rigidbody"/>.
        /// </summary>
        public Rigidbody Rigidbody { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Features.Input.InputActionComponent"/>.
        /// </summary>
        public InputActionComponent InputActionComponent { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the axis should be inverted.
        /// </summary>
        public bool InvertAxis
        {
            get => invertAxis;
            protected set => invertAxis = value;
        }

        /// <summary>
        /// Gets the current movement state.
        /// </summary>
        public sbyte MovementState
        {
            get => movementState;
            protected set
            {
                sbyte prevState = movementState;
                movementState = value;
                OnMovementStateChanged(prevState);
            }
        }

        /// <summary>
        /// Gets the root collision.
        /// </summary>
        public GenRootCollisionComponent RootCollision { get; protected set; }

        /// <inheritdoc cref="GenRootCollisionComponent.Extent"/>
        public Vector3 RootCollisionExtent
        {
            get => RootCollision.Extent;
            protected set => RootCollision.Extent = value;
        }

        /// <inheritdoc cref="GenRootCollisionComponent.HalfHeight"/>
        public float RootCollisionHalfHeight
        {
            get => RootCollision.HalfHeight;
            protected set => RootCollision.HalfHeight = value;
        }

        /// <inheritdoc cref="GenRootCollisionComponent.radius"/>
        public float RootCollisionRadius
        {
            get => RootCollision.Radius;
            protected set => RootCollision.Radius = value;
        }

        /// <inheritdoc cref="GenRootCollisionComponent.HasBoxCollision"/>
        public bool HasBoxCollision => RootCollision.HasBoxCollision;

        /// <inheritdoc cref="GenRootCollisionComponent.HasVerticalCollision"/>
        public bool HasVerticalCollision => RootCollision.HasVerticalCollision;

        /// <inheritdoc cref="GenRootCollisionComponent.HasSphereCollision"/>
        public bool HasSphereCollision => RootCollision.HasSphereCollision;

        /// <inheritdoc cref="GenRootCollisionComponent.HasValidCollision"/>
        public bool HasValidCollision => RootCollision.HasValidCollision;

        /// <inheritdoc cref="GenRootCollisionComponent.CollisionShape"/>
        public ECollisionShape RootCollisionShape => RootCollision.CollisionShape;

        /// <inheritdoc cref="GenRootCollisionComponent.Transform"/>
        public Transform RootCollisionTransform => RootCollision.Transform;

        /// <inheritdoc cref="GenRootCollisionComponent.Center"/>
        public Vector3 RootCollisionCenter => RootCollision.Center;

        /// <summary>
        /// Gets the previous movement state.
        /// </summary>
        public sbyte PreviousMovementState { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="UnityEngine.Experimental.Rendering.RayTracingMode"/> for the <see cref="SkeletalMesh"/>.
        /// </summary>
        public RayTracingMode RayTracingMode
        {
            get => SkeletalMesh.rayTracingMode != rayTracingMode ? rayTracingMode = SkeletalMesh.rayTracingMode : rayTracingMode;
            protected set => SkeletalMesh.rayTracingMode = rayTracingMode = value;
        }

        /// <summary>
        /// Gets or sets the acceleration input vector.
        /// </summary>
        public Vector3 AccelerationInputVector { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pawn should be oriented to input direction.
        /// </summary>
        public virtual bool OrientToInputDirection
        {
            get => orientToInputDirection;
            protected set => orientToInputDirection = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pawn should be oriented to movement.
        /// </summary>
        public bool OrientToMovement
        {
            get => orientToMovement;
            protected set => orientToMovement = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pawn should be oriented to rotation.
        /// </summary>
        public bool OrientToRotation
        {
            get => orientToRotation;
            protected set => orientToRotation = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pawn should be oriented based on analog movement.
        /// </summary>
        public bool OrientToAnalog
        {
            get => orientToAnalog;
            protected set => orientToAnalog = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the rotation sensitivity should be used.
        /// </summary>
        public bool UseRotationSensitivity
        {
            get => useRotationSensitivity;
            protected set => useRotationSensitivity = value;
        }

        /// <summary>
        /// Gets or sets the rotation damp.
        /// </summary>
        public float RotationDamp
        {
            get => rotationDamp;
            protected set => rotationDamp = value;
        }

        /// <summary>
        /// Gets or sets the acceleration rate.
        /// </summary>
        public float AccelerationRate
        {
            get => accelerationRate;
            protected set => accelerationRate = value;
        }

        /// <summary>
        /// Gets or sets the yaw sensitivity.
        /// </summary>
        public float YawSensitivity
        {
            get => yawSensitivity;
            protected set => yawSensitivity = value;
        }

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        public virtual Vector3 Velocity
        {
            get => Rigidbody.velocity;
            protected set => Rigidbody.velocity = value;
        }

        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        public virtual Vector3 Torque
        {
            get => Rigidbody.angularVelocity;
            protected set => Rigidbody.angularVelocity = value;
        }

        /// <summary>
        /// Gets or sets a value indicating the gravity affects the pawn.
        /// </summary>
        public bool UseGravity
        {
            get => Rigidbody.useGravity;
            set => Rigidbody.useGravity = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether first person perspective should be used.
        /// </summary>
        public bool UseFirstPersonPerspective
        {
            get => useFirstPersonPerspective;
            protected set
            {
                useFirstPersonPerspective = value;
                SkeletalMesh.shadowCastingMode = useFirstPersonPerspective ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="LayerMask"/> to evaluate objects which make the pawn bounce when they collide.
        /// </summary>
        public LayerMask BouncingCollisionMask
        {
            get => bouncingCollisionMask;
            set => bouncingCollisionMask = value;
        }

        /// <summary>
        /// Gets or sets the bouncing collision force.
        /// </summary>
        public float BouncingCollisionForce
        {
            get => bouncingCollisionForce;
            set => bouncingCollisionForce = value;
        }

        /// <summary>
        /// Gets or sets the time since last bounce.
        /// </summary>
        public float TimeSinceLastBounce { get; protected set; }

        /// <summary>
        /// Gets the current acceleration.
        /// </summary>
        public Vector3 Acceleration { get; protected set; }

        /// <summary>
        /// Gets the current acceleration's magnitude.
        /// </summary>
        public float AccelerationMagnitude => Acceleration.magnitude;

        /// <summary>
        /// Gets the velocity during the last frame.
        /// </summary>
        public Vector3 VelocityLastFrame { get; protected set; }

        /// <summary>
        /// Gets the velocity's magnitude during the last frame.
        /// </summary>
        public float VelocityLastFrameMagnitude => VelocityLastFrame.magnitude;

        /// <summary>
        /// Gets the current speed vector's magnitude.
        /// </summary>
        public float Speed => Velocity.magnitude;

        /// <summary>
        /// Gets the current speed vector of both x and z axis.
        /// </summary>
        public Vector3 VelocityXZ => new(Velocity.x, 0f, Velocity.z);

        /// <summary>
        /// Gets the current speed vector's magnitude of both x and z axis.
        /// </summary>
        public float SpeedXZ => VelocityXZ.magnitude;

        /// <summary>
        /// Gets the floor distance.
        /// </summary>
        public float FloorDistance => FloorHit.distance;

        /// <summary>
        /// Gets the floor collider, if any.
        /// </summary>
        public Collider FloorCollider => FloorHit.collider;

        /// <summary>
        /// Gets the floor <see cref="UnityEngine.Transform"/>, or <see langword="null"/> if not found.
        /// </summary>
        public Transform FloorTransform => FloorCollider.transform;

        /// <summary>
        /// Gets or sets the walkable <see cref="LayerMask"/>.
        /// </summary>
        public virtual LayerMask WalkableMask
        {
            get => walkableMask;
            set => walkableMask = value;
        }

        /// <summary>
        /// Gets or sets the max floor distance tolerance before the pawn will be considered in air.
        /// </summary>
        public float MaxFloorDistance
        {
            get => maxFloorDistance;
            set => maxFloorDistance = value;
        }

        /// <summary>
        /// Gets or sets the max ground traceline height.
        /// </summary>
        public float MaxFloorTracelineHeight
        {
            get => maxFloorTracelineHeight;
            protected set => maxFloorTracelineHeight = value;
        }

        /// <summary>
        /// Gets the floor hit, or <see langword="default"/> if not valid.
        /// </summary>
        public RaycastHit FloorHit => floorHit;

        /// <summary>
        /// Gets or sets the max speed.
        /// </summary>
        public virtual float MaxSpeed
        {
            get => ProcessMaxSpeed(currentSpeed);
            set => currentSpeed = ProcessMaxSpeed(value);
        }

        /// <summary>
        /// Gets or sets the max acceleration.
        /// </summary>
        public virtual float MaxAcceleration
        {
            get => ProcessMaxAcceleration(maxAcceleration);
            set => maxAcceleration = ProcessMaxAcceleration(value);
        }

        /// <summary>
        /// Gets the processed input vector.
        /// </summary>
        public Vector3 InputVector { get; protected set; }

        /// <summary>
        /// Gets the processed input direction.
        /// </summary>
        public Vector3 InputDirection { get; protected set; }

        /// <summary>
        /// Gets the horizontal input vector value.
        /// </summary>
        public float Horizontal { get; protected set; }

        /// <summary>
        /// Gets the vertical input vector value.
        /// </summary>
        public float Vertical { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pawn can turn.
        /// </summary>
        public bool CanTurn { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the pawn can move.
        /// </summary>
        public bool CanMove { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether the pawn wants to move.
        /// </summary>
        public virtual bool WantsToMove => InputVector.magnitude > 0f;

        /// <summary>
        /// Gets the current friction based on the the floor's properties.
        /// </summary>
        public float CurrentFriction { get; protected set; }

        /// <summary>
        /// Gets the current angular friction based on the floor's properties.
        /// </summary>
        public float CurrentAngularFriction { get; protected set; }

        /// <summary>
        /// Gets or sets the grounded friction.
        /// </summary>
        public float GroundedFriction
        {
            get => groundedFriction;
            protected set => groundedFriction = value;
        }

        /// <summary>
        /// Gets or sets the airborne friction.
        /// </summary>
        public float AirborneFriction
        {
            get => airborneFriction;
            protected set => airborneFriction = value;
        }

        /// <summary>
        /// Gets or sets the grounded angular friction.
        /// </summary>
        public float GroundedAngularFriction
        {
            get => groundedAngularFriction;
            protected set => groundedAngularFriction = value;
        }

        /// <summary>
        /// Gets or sets the airborne angular friction.
        /// </summary>
        public float AirborneAngularFriction
        {
            get => airborneAngularFriction;
            protected set => airborneAngularFriction = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pawn is hovering.
        /// </summary>
        public bool IsHovering
        {
            get => isHovering;
            protected set => isHovering = value;
        }

        /// <summary>
        /// Gets or sets the maximum hovering distance.
        /// </summary>
        public float MaxHoveringDistance
        {
            get => maxHoveringDistance;
            protected set => maxHoveringDistance = value;
        }

        /// <summary>
        /// Gets or sets the max hovering pitch velocity.
        /// </summary>
        public float MaxHoveringPitchVelocity
        {
            get => maxHoveringPitchVelocity;
            protected set => maxHoveringPitchVelocity = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the hooks law should be used instead.
        /// </summary>
        public bool UseHooksLaw
        {
            get => useHooksLaw;
            protected set => useHooksLaw = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pawn movement is locked to forward direction.
        /// </summary>
        public bool LockForwardDirection { get; set; }

        /// <summary>
        /// Gets the thrusters.
        /// </summary>
        public virtual IEnumerable<GameObject> Thrusters => cachedThrusters;

        /// <summary>
        /// Gets or sets a value indicating whether the pawn is moving in air.
        /// </summary>
        public virtual bool IsAirborne => FloorDistance > MaxFloorDistance;

        /// <summary>
        /// Gets or sets a value indicating whether the pawn is jumping.
        /// </summary>
        public bool IsJumping { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pawn is sprinting.
        /// </summary>
        public bool IsSprinting { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the pawn has just jumped;
        /// </summary>
        public bool JustJumped { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the pawn wants to jump.
        /// </summary>
        protected bool WantsToJump { get; set; }

        /// <summary>
        /// Gets a value indicating whether the pawn wants to sprint.
        /// </summary>
        protected bool WantsToSprint { get; set; }

        /// <summary>
        /// Gets a value indicating whether the pawn can jump.
        /// </summary>
        protected virtual bool CanJump => IsHovering ? (!JustJumped && isJumpAvailable) : (!IsAirborne && !JustJumped && isJumpAvailable);

        /// <summary>
        /// Gets a value indicating whether the pawn can sprint.
        /// </summary>
        protected virtual bool CanSprint => !IsAirborne && !JustJumped;

#if DEBUG
        /// <summary>
        /// Gets or sets a value indicating whether the pawn is paused.
        /// </summary>
        protected virtual bool IsPaused
        {
            get => isPaused;
            set
            {
                isPaused = value;

                if (value)
                {
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    return;
                }

                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
#endif
        /// <summary>
        ///Stat which handles all the delegates fired when the movement state is changing.
        /// </summary>
        [DynamicEventDispatcher]
        protected DynamicEventDispatcher BeginMovementStateMulticastDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DynamicEventDispatcher"/> which handles all the delegates fired when the movement state is changed.
        /// </summary>
        [DynamicEventDispatcher]
        protected DynamicEventDispatcher EndMovementStateMulticastDispatcher { get; set; }

        /// <summary>
        /// Gets the current movement state.
        /// </summary>
        /// <typeparam name="T">The movement state enum.</typeparam>
        /// <returns>The current movement state as the enum value.</returns>
        public T GetMovementState<T>()
            where T : Enum => (T)Enum.ToObject(typeof(T), MovementState);

        /// <summary>
        /// Gets the <see cref="RootCollision"/> as the specified collision shape.
        /// </summary>
        /// <typeparam name="T">The type of the collision shape.</typeparam>
        /// <returns>The cast <see cref="RootCollision"/> or a <see cref="InvalidCastException"/> if the specified cast is not valid.</returns>
        /// <exception cref="InvalidCastException">Thrown when the specified cast is not valid.</exception>
        public T GetRootCollision<T>()
            where T : Collider => RootCollision.GetRootCollision<T>();

        /// <summary>
        /// Sets the movement state to the specified value.
        /// </summary>
        /// <param name="newState">The new movement state.</param>
        protected void SetMovementState(object newState)
        {
            try
            {
                MovementState = (sbyte)newState;
            }
            catch
            {
            }
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            InitializeBehaviours();
            InitializeEObjects();
            DefineInputBindings();
            BindInputActions();
            InitializeComponentData();
        }

#if UNITY_EDITOR && DEBUG
        /// <inheritdoc/>
        protected override void OnValidating()
        {
            base.OnValidating();

            Camera = Camera ?? GetComponentSafe<GenCameraComponent>();
            Rigidbody = Rigidbody ?? GetComponentSafe<Rigidbody>();
            InputActionComponent = InputActionComponent ?? GetComponentSafe<InputActionComponent>();
            RootCollision ??= UObject.CreateDefaultSubobject<GenRootCollisionComponent>(GetComponent<Collider>(), gameObject);
            cachedThrusters ??= new();
        }

        /// <inheritdoc/>
        protected override void EditorTick()
        {
            RegenerateThrusters();

            foreach (Transform t in Transform)
            {
                if (!t.gameObject.name.Contains("Thruster#") || cachedThrusters.Contains(t.gameObject))
                    continue;

                cachedThrusters.Add(t.gameObject);
            }

            cachedThrusters.RemoveAll(go => !go);
        }

        /// <summary>
        /// Draws gizmos if the object is selected.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            foreach (GameObject thruster in Thrusters.Where(t => t))
                UnityEngine.Debug.DrawLine(thruster.transform.position, thruster.transform.position + -Transform.up * MaxHoveringDistance, Color.red);
        }
#endif

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            foreach (Transform t in Transform)
            {
                if (!t.gameObject.name.Contains("Thruster#") || cachedThrusters.Contains(t.gameObject))
                    continue;

                cachedThrusters.Add(t.gameObject);
            }

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (Camera)
                Camera.ParentTransform = Transform;

            OnLanded();
        }

        /// <inheritdoc/>
        protected override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            PreMovementUpdate(deltaTime);
            MovementUpdate(deltaTime);
        }

        /// <inheritdoc/>
        protected override void FixedTick(float fixedDeltaTime)
        {
            base.FixedTick(fixedDeltaTime);

            FixedMovementUpdate(Time.fixedDeltaTime);
        }

        /// <inheritdoc/>
        protected override void LateTick(float deltaTime)
        {
            base.LateTick(deltaTime);

            PostMovementUpdate(Time.deltaTime);
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            UnsubscribeEvents();
        }

        /// <summary>
        /// <inheritdoc cref="Awake"/>
        /// <para>
        /// <see cref="MonoBehaviour"/> components should be initialized here.
        /// </para>
        /// </summary>
        protected virtual void InitializeBehaviours()
        {
            Camera = GetComponentSafe<GenCameraComponent>();
            SkeletalMesh = GetComponentSafe<Renderer>();
            Rigidbody = GetComponentSafe<Rigidbody>();
            InputActionComponent = GetComponentSafe<InputActionComponent>();
        }

        /// <summary>
        /// <inheritdoc cref="OnPostInitialize"/>
        /// <para>
        /// <see cref="UObject"/> components should be initialized here.
        /// </para>
        /// </summary>
        protected virtual void InitializeEObjects() => RootCollision = UObject.CreateDefaultSubobject<GenRootCollisionComponent>(GetComponent<Collider>(), gameObject);

        /// <summary>
        /// <inheritdoc cref="OnPostInitialize"/>
        /// <para>
        /// All the component data should be initialized here <i>(i.e. default values [...])</i>.
        /// </para>
        /// </summary>
        protected virtual void InitializeComponentData()
        {
            MaxSpeed = defaultMaxSpeed;
            MaxAcceleration = maxAcceleration;
            CurrentFriction = defaultFriction;
            CurrentAngularFriction = defaultAngularFriction;

            if (rootCollisionExtent.magnitude > 0f)
                RootCollision.Extent = rootCollisionExtent;

            if (rootCollisionHalfHeight > 0f)
                RootCollision.HalfHeight = rootCollisionHalfHeight;

            if (rootCollisionRadius > 0f)
                RootCollision.Radius = rootCollisionRadius;

            Physics.defaultMaxDepenetrationVelocity = 100f;

            cachedThrusters ??= new();
        }

        /// <summary>
        /// <inheritdoc cref="OnPostInitialize"/>
        /// <para>
        /// <see cref="InputBinding"/>'s should be defined here, in order to avoid conflicts with the current execution pipeline.
        /// </para>
        /// </summary>
        protected virtual void DefineInputBindings()
        {
            InputBinding.Create("Horizontal");
            InputBinding.Create("Vertical");
            InputBinding.Create("Jump", KeyCode.Space);
            InputBinding.Create("Sprint", KeyCode.LeftShift);
#if DEBUG
            InputBinding.Create("Pause", KeyCode.Escape);
#endif
        }

        /// <summary>
        /// <inheritdoc cref="OnPostInitialize"/>
        /// <para>
        /// <see cref="Features.Input.InputBinding"/>'s should be bound here, in order to avoid conflicts with the current execution pipeline.
        /// </para>
        /// </summary>
        protected virtual void BindInputActions()
        {
            InputActionComponent.BindInputAxis(InputBinding.Get("Horizontal"), MoveForward);
            InputActionComponent.BindInputAxis(InputBinding.Get("Vertical"), MoveRight);

            InputActionComponent.BindInputAction(InputBinding.Get("Jump"), Jump);
            InputActionComponent.BindInputAction(InputBinding.Get("Sprint"), Sprint);
#if DEBUG
            InputActionComponent.BindInputAction(InputBinding.Get("Pause"), Pause);
#endif
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            BeginMovementStateMulticastDispatcher.Bind(EMovementState.None, OnBeginIdling);
            BeginMovementStateMulticastDispatcher.Bind(EMovementState.Jogging, OnBeginJogging);
            BeginMovementStateMulticastDispatcher.Bind(EMovementState.Sprinting, OnBeginSprinting);
            BeginMovementStateMulticastDispatcher.Bind(EMovementState.Airborne, OnBeginAirborne);

            EndMovementStateMulticastDispatcher.Bind(EMovementState.None, OnEndIdling);
            EndMovementStateMulticastDispatcher.Bind(EMovementState.Jogging, OnEndJogging);
            EndMovementStateMulticastDispatcher.Bind(EMovementState.Sprinting, OnEndSprinting);
            EndMovementStateMulticastDispatcher.Bind(EMovementState.Airborne, OnEndAirborne);
        }

        /// <summary>
        /// <inheritdoc cref="Tick(float)"/>
        /// <br>All the timers should be updated and managed here.</br>
        /// </summary>
        protected virtual void TimersUpdate()
        {
            TimeSinceLastBounce += Time.deltaTime;
        }

        /// <summary>
        /// Fired before <see cref="MovementUpdate(float)"/>.
        /// <para>
        /// All the movement code logic to be executed before updating the actual movement should be placed and managed here.
        /// <br>This is mandatory in case of networked solutions.</br>
        /// </para>
        /// </summary>
        /// <param name="deltaTime">The delta time value.</param>
        protected virtual void PreMovementUpdate(float deltaTime)
        {
            RayTracingMode = rayTracingMode;
            UseFirstPersonPerspective = useFirstPersonPerspective;
            ProcessInputVector(deltaTime);
        }

        /// <summary>
        /// Fired every frame after <see cref="PreMovementUpdate(float)"/>.
        /// <para>
        /// The code which handles and manages the movement logic should be placed and managed here.
        /// <br>This is mandatory is case of networked solutions.</br>
        /// </para>
        /// </summary>
        /// <param name="deltaTime">The delta time value.</param>
        protected virtual void MovementUpdate(float deltaTime)
        {
            CalculateFloorHit();
            CalculateFriction();
            CalculateAngularFriction();

            Velocity -= CurrentFriction * deltaTime * Velocity;
            Torque -= CurrentAngularFriction * deltaTime * Torque;

            ResolveMovementState();
        }

        /// <summary>
        /// Fired after <see cref="PreMovementUpdate(float)"/>.
        /// <para>
        /// The code which handles and manages the movement and physics logic should be placed and managed here.
        /// <br>This is mandatory is case of networked solutions.</br>
        /// </para>
        /// </summary>
        /// <param name="fixedDeltaTime">The fixed delta time value.</param>
        protected virtual void FixedMovementUpdate(float fixedDeltaTime)
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

            if (OrientToAnalog)
            {
                if (UseRotationSensitivity && CanTurn)
                    Rigidbody.AddTorque(YawSensitivity * fixedDeltaTime * Horizontal * Transform.up);

                if (CanMove)
                {
                    Vector3 direction = Transform.worldToLocalMatrix.inverse * (Vector3.forward * Vertical);
                    Rigidbody.AddForce(MaxAcceleration * fixedDeltaTime * direction);
                }
            }

            if (IsHovering)
            {
                if (Velocity.y > MaxHoveringPitchVelocity)
                {
                    Vector3 velNormal = Velocity.normalized;
                    Velocity = new(Velocity.x, MaxHoveringPitchVelocity, Velocity.z);
                }

                ApplyHoveringForce(UseHooksLaw);
            }
        }

        /// <summary>
        /// Fired every frame after <see cref="MovementUpdate(float)"/> and <see cref="FixedMovementUpdate(float)"/>.
        /// <para>
        /// The code to be executed after updating the actual movement and physics should be placed here.
        /// <br>This is mandatory is case of networked solutions.</br>
        /// </para>
        /// </summary>
        /// <param name="deltaTime">The delta time value.</param>
        protected virtual void PostMovementUpdate(float deltaTime)
        {
            if (!IsAirborne && JustJumped)
            {
                JustJumped = false;
                OnLanded();
            }
        }

        /// <summary>
        /// Processes the input vector.
        /// </summary>
        /// <param name="deltaTime">The delta time value.</param>
        protected virtual void ProcessInputVector(float deltaTime)
        {
            InputVector = new Vector3(Vertical, 0f, Horizontal).normalized;
            InputDirection = OrientToInputDirection ? InputVector : (Transform.right * Horizontal) + (Transform.forward * Vertical);
            float cameraYaw = Camera.Yaw;

            if (InputVector.magnitude < 0.1f)
            {
                if (OrientToRotation && CanTurn)
                {
                    Rotation = UseRotationSensitivity ?
                        Quaternion.Slerp(Rotation, Quaternion.Euler(0, cameraYaw, 0), YawSensitivity / 5f * Time.fixedDeltaTime) :
                        Quaternion.Euler(new(0f, cameraYaw, 0f));
                }
            }
            else
            {
                if (OrientToInputDirection)
                {
                    if (CanTurn)
                    {
                        float targetAngle = Mathf.Atan2(InputVector.x, -InputVector.z) * Mathf.Rad2Deg;
                        float angle = Mathf.SmoothDampAngle(Transform.eulerAngles.y, targetAngle, ref ref_rotationRate, RotationDamp);

                        Rotation = Quaternion.Euler(0f, angle, 0f);
                    }

                    if (CanMove)
                    {
                        Velocity = new(
                            Mathf.SmoothDamp(Velocity.x, InputVector.x * MaxAcceleration, ref ref_accel_velX, AccelerationRate),
                            Velocity.y,
                            -Mathf.SmoothDamp(Velocity.z, InputVector.z * MaxAcceleration, ref ref_accel_velZ, AccelerationRate));
                    }
                }
                else if (OrientToMovement)
                {
                    if (OrientToRotation && CanTurn)
                    {
                        Rotation = UseRotationSensitivity ?
                            Quaternion.Slerp(Rotation, Quaternion.Euler(0, cameraYaw, 0), YawSensitivity / 5f * Time.fixedDeltaTime) :
                            Quaternion.Euler(new(0f, cameraYaw, 0f));
                    }

                    Vector3 direction = InputVector;
                    direction = Transform.worldToLocalMatrix.inverse * direction;

                    if (CanMove)
                    {
                        Velocity = new(
                            Mathf.SmoothDamp(Velocity.x, direction.x * MaxAcceleration, ref ref_accel_velX, AccelerationRate),
                            Velocity.y,
                            Mathf.SmoothDamp(Velocity.z, direction.z * MaxAcceleration, ref ref_accel_velZ, AccelerationRate));
                    }
                }
                else
                {
                    if (OrientToAnalog && !UseRotationSensitivity && CanTurn)
                    {
                        Quaternion rotation = Quaternion.Euler(YawSensitivity * deltaTime * Horizontal * Transform.up);
                        Transform.Rotate(rotation.eulerAngles);

                        return;
                    }

                    if (CanMove)
                    {
                        Velocity = new(
                            Mathf.SmoothDamp(InputDirection.x, InputDirection.x * MaxAcceleration, ref ref_accel_velZ, AccelerationRate),
                            Velocity.y,
                            Mathf.SmoothDamp(InputDirection.z, InputDirection.z * MaxAcceleration, ref ref_accel_velZ, AccelerationRate));
                    }
                }
            }
        }
        
        /// <summary>
        /// Processes the max speed by evaluating all active <see cref="IMovementSpeedModifier"/> instances.
        /// </summary>
        /// <param name="curSpeed">The current speed.</param>
        /// <returns>The processed max speed.</returns>
        protected virtual float ProcessMaxSpeed(float curSpeed)
        {
            float speedMultiplier = 1f;

            if (Owner.Cast(out EntityBase entity) && entity.Inventory)
            {
                speedMultiplier = Mathf.Max(speedMultiplier, entity.Inventory.MovementSpeedMultiplier);
                curSpeed = Mathf.Max(curSpeed, entity.Inventory.MovementSpeedLimit);

                if (entity.EffectsController)
                {
                    foreach (StatusEffectBase effect in entity.EffectsController.AllEffects)
                    {
                        if (!effect.IsEnabled || effect is not IMovementSpeedModifier speedModifier || !speedModifier.MovementModifierActive)
                            continue;

                        curSpeed = Mathf.Max(curSpeed, speedModifier.MovementSpeedLimit);
                        speedMultiplier += speedModifier.MovementSpeedMultiplier;
                    }
                }
            }

            if (speedMultiplier > 0f)
                curSpeed *= speedMultiplier;

            return curSpeed; 
        }

        /// <summary>
        /// Processes the max acceleration by evaluating all active <see cref="IMovementSpeedModifier"/> instances.
        /// </summary>
        /// <param name="curAccel">The current acceleration.</param>
        /// <returns>The processed max acceleration.</returns>
        protected virtual float ProcessMaxAcceleration(float curAccel)
        {
            if (Owner.Cast(out EntityBase entity) && entity.Inventory)
            {
                curAccel = Math.Max(curAccel, entity.Inventory.MovementAccelerationMultiplier);

                if (entity.EffectsController)
                {
                    foreach (StatusEffectBase effect in entity.EffectsController.AllEffects)
                    {
                        if (!effect.IsEnabled || effect is not IMovementSpeedModifier speedModifier || !speedModifier.MovementModifierActive)
                            continue;

                        curAccel += speedModifier.MovementAccelerationMultiplier;
                    }
                }
            }

            return curAccel;
        }

        /// <summary>
        /// Fired when the pawn has begun colliding with a <see cref="Collider"/>.
        /// </summary>
        /// <param name="collision">The <see cref="Collision"/> object.</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if ((BouncingCollisionMask & (1 << collision.gameObject.layer)) != 0)
            {
                Rigidbody.AddForce(collision.GetContact(0).normal * VelocityLastFrameMagnitude * BouncingCollisionForce, ForceMode.Impulse);
                TimeSinceLastBounce = 0f;
            }
        }

        /// <summary>
        /// Fired when the <see cref="MovementState"/> changes.
        /// </summary>
        /// <param name="previousMovementState"></param>
        protected virtual void OnMovementStateChanged(sbyte previousMovementState)
        {
            if (previousMovementState == MovementState)
                return;

            PreviousMovementState = previousMovementState;
            EndMovementStateMulticastDispatcher.Invoke(PreviousMovementState);
            BeginMovementStateMulticastDispatcher.Invoke(MovementState);
        }

        /// <summary>
        /// Resolves the current movement state based on the evaluated data.
        /// </summary>
        protected virtual void ResolveMovementState() =>
            SetMovementState(IsAirborne ? EMovementState.Airborne :
            !WantsToMove || SpeedXZ <= 0.1f ? EMovementState.None :
            IsSprinting ? EMovementState.Sprinting :
            EMovementState.Jogging);

        /// <summary>
        /// Calculates the floor hit.
        /// </summary>
        protected virtual void CalculateFloorHit() =>
            Physics.SphereCast(Position + (Vector3.up * (RootCollision.Radius + Physics.defaultContactOffset)),
                RootCollision.Radius - Physics.defaultContactOffset,
                Vector3.down, out floorHit, MaxFloorTracelineHeight,
                WalkableMask, QueryTriggerInteraction.Ignore);

        /// <summary>
        /// Calculates the friction.
        /// </summary>
        protected virtual void CalculateFriction() => CurrentFriction = IsAirborne ? AirborneFriction : GroundedFriction;

        /// <summary>
        /// Calculates the angualar friction.
        /// </summary>
        protected virtual void CalculateAngularFriction() => CurrentAngularFriction = IsAirborne ? AirborneAngularFriction : GroundedAngularFriction;

        /// <summary>
        /// Applies hovering force to the pawn.
        /// </summary>
        /// <param name="isHooks">Whether the hooks law should be used instead.</param>
        protected virtual void ApplyHoveringForce(bool isHooks = false)
        {
            void AddHoveringForce(RaycastHit hit)
            {
                if (isHooks)
                {
                    Rigidbody.AddForce(Transform.up * (hooksLawStrength / (hit.distance / 2)));
                    return;
                }

                Velocity = new(Velocity.x, Velocity.y + ((MaxHoveringDistance - hit.distance) * hooksLawStrength), Velocity.z);
            }

            List<RaycastHit> doneHits = null;
            foreach (GameObject thruster in Thrusters)
            {
                if (Physics.Raycast(thruster.transform.position, -Transform.up, out RaycastHit hit, MaxHoveringDistance, WalkableMask))
                {
                    if (doneHits is null)
                    {
                        AddHoveringForce(hit);

                        doneHits = new();
                        doneHits.Add(hit);
                        continue;
                    }

                    if (!doneHits.Any(rHit => (Mathf.Abs(rHit.point.y - hit.point.y)) > 0.2f))
                        continue;

                    AddHoveringForce(hit);
                }
            }
        }

        /// <summary>
        /// Regenerates the active thrusters.
        /// </summary>
        protected virtual void RegenerateThrusters()
        {
            if (activeThrusters == cachedThrusters.Count)
                return;

            if (activeThrusters < cachedThrusters.Count)
            {
                for (int i = cachedThrusters.Count - 1; i == activeThrusters; i--)
                    DestroyImmediate(cachedThrusters[i]);
            }

            for (int i = 0; i < activeThrusters; i++)
            {
                bool alreadyExists = false;
                foreach (var _ in
                    from Transform t in Transform
                    where t.gameObject.name == $"Thruster#{i}"
                    select new { })
                {
                    alreadyExists = true;
                    break;
                }

                if (alreadyExists)
                    continue;

                GameObject thruster = new($"Thruster#{i}");
                thruster.transform.parent = Transform;
                thruster.transform.position = Transform.position;
            }
        }

        /// <summary>
        /// Stops the pawn movement.
        /// </summary>
        /// <param name="canMove">Whether the pawn can move.</param>
        /// <param name="interp">The interpolation value.</param>
        protected virtual void StopMovement(bool canMove, float interp)
        {
            CanMove = canMove;
            Timing.CallContinuously(Time.fixedDeltaTime, () => Velocity = Vector3.Lerp(Velocity, Vector3.zero, interp * Time.fixedDeltaTime));
        }

        /// <summary>
        /// Stops the pawn movement immediately.
        /// </summary>
        /// <param name="canMove">Whether the pawn can move.</param>
        protected virtual void StopMovementImmediately(bool canMove = true)
        {
            CanMove = canMove;
            Velocity = Vector3.zero;
        }

        #region InputActions

        /// <summary>
        /// Fired when the vertical axis changes.
        /// </summary>
        /// <param name="scale">The scale of the input.</param>
        protected virtual void MoveForward(float scale)
        {
            if (InvertAxis)
                Horizontal = LockForwardDirection ? Mathf.Abs(scale) : scale;
            else
                Vertical = LockForwardDirection ? Mathf.Abs(scale) : scale;
        }

        /// <summary>
        /// Fired when the horizontal axis changes.
        /// </summary>
        /// <param name="scale">The scale of the input.</param>
        protected virtual void MoveRight(float scale)
        {
            if (InvertAxis)
                Vertical = scale;
            else
                Horizontal = scale;
        }

#if DEBUG
        /// <summary>
        /// Pauses the pawn.
        /// </summary>
        protected virtual void Pause(bool wantsToPause) => IsPaused = wantsToPause;
#endif
        /// <summary>
        /// Performs a jump action.
        /// </summary>
        /// <param name="wantsToJump">Whether the pawn wants to jump.</param>
        protected virtual void Jump(bool wantsToJump)
        {
            WantsToJump = wantsToJump;

            if (!WantsToJump)
            {
                canJump = CanJump;
                return;
            }

            if (!canJump)
                return;

            isJumpAvailable = canJump = false;
            JustJumped = true;

            if (!IsHovering)
            {
                Rigidbody.AddForce(Transform.up * jumpForce, ForceMode.Impulse);
            }
            else
            {
                lastPitchVelocity = MaxHoveringPitchVelocity;
                MaxHoveringPitchVelocity = hoveringJumpPitchVelocity;
                Rigidbody.AddForce(Transform.up * jumpForce, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Performs a sprint action.
        /// </summary>
        /// <param name="wantsToSprint">Whether the pawn wants to sprint.</param>
        protected virtual void Sprint(bool wantsToSprint)
        {
            WantsToSprint = wantsToSprint;

            if (!WantsToSprint || !canSprint)
            {
                IsSprinting = false;
                canSprint = CanSprint;
                return;
            }

            IsSprinting = true;
        }

        #endregion

        #region MovementDelegates

        /// <summary>
        /// Fired when the pawn lands.
        /// </summary>
        protected virtual void OnLanded()
        {
            Timing.CallDelayed(jumpRate, () => isJumpAvailable = true);

            if (IsHovering)
            {
                float curPitchVel = MaxHoveringPitchVelocity;
                MaxHoveringPitchVelocity = lastPitchVelocity;
                lastPitchVelocity = curPitchVel;
            }
        }

        /// <summary>
        /// Fired when the pawn begins idling.
        /// </summary>
        protected virtual void OnBeginIdling() => MaxSpeed = defaultMaxSpeed;

        /// <summary>
        /// Fired when the pawn begins jogging.
        /// </summary>
        protected virtual void OnBeginJogging() => MaxSpeed = defaultMaxSpeed;

        /// <summary>
        /// Fired when the pawn begins sprinting.
        /// </summary>
        protected virtual void OnBeginSprinting() => MaxSpeed = sprintSpeed;

        /// <summary>
        /// Fired when the pawn begins airborne.
        /// </summary>
        protected virtual void OnBeginAirborne() => MaxSpeed = airborneSpeed;

        /// <summary>
        /// Fired when the pawn ends idling.
        /// </summary>
        protected virtual void OnEndIdling()
        {
        }

        /// <summary>
        /// Fired when the pawn ends jogging.
        /// </summary>
        protected virtual void OnEndJogging()
        {
        }

        /// <summary>
        /// Fired when the pawn ends sprinting.
        /// </summary>
        protected virtual void OnEndSprinting()
        {
        }

        /// <summary>
        /// Fired when the pawn ends airborne.
        /// </summary>
        protected virtual void OnEndAirborne()
        {
        }

        #endregion
    }
}
