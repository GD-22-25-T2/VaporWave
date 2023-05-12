namespace UDK.API.Features.CharacterMovement
{
    using UDK.API.Features.Core;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using UDK.Events.DynamicEvents;
    using UDK.MEC;
    using System.Linq;
    using UDK.API.Features.Core.Framework.Navigation;

    /// <summary>
    /// The base component which handles Nav Mesh agents.
    /// <para>
    /// <see cref="GenNavMeshAgentComponent"/> provides a basic implementation for footsteps.
    /// <br>The current implementation is intended to be rewritten as needed, as it doesn't really provide solutions other than the default ones.</br>
    /// <br>A new implementation will be eventually added on the next major version (2.0.0).</br>
    /// </para>
    /// <para>
    /// The current implementation is not network replicated.
    /// </para>
    /// <para>
    /// It's not strictly dependent on <see cref="GenMovementComponent"/> as the code base doesn't refer to it.
    /// <br>It's safe to use it along with different movement systems and implementations.</br>
    /// </para>
    /// </summary>
    [AddComponentMenu("UDK/CharacterMovement/GenNavMeshAgentComponent")]
    [DisallowMultipleComponent]
    public class GenNavMeshAgentComponent : Actor
    {
        #region Editor

        [SerializeField]
        protected int agentTypeID;

        [SerializeField]
        protected float radius;

        [SerializeField]
        protected float height;

        [SerializeField]
        protected LayerMask walkableMask;

        [SerializeField]
        protected float maxSpeed;

        [SerializeField]
        protected float maxAcceleration;

        [SerializeField]
        protected float maxAngularSpeed;

        [SerializeField]
        protected float stoppingDistance;

        [SerializeField]
        protected bool autoTraverseOffMeshLink = true;

        [SerializeField]
        protected bool autobraking = true;

        [SerializeField]
        protected bool autoRepath;

        [SerializeField]
        protected float baseOffset;

        [SerializeField]
        protected ObstacleAvoidanceType obstacleAvoidanceType;

        [SerializeField]
        protected int avoidancePriority = 50;

        [SerializeField]
        protected float walkPointRange;

        [SerializeField]
        protected float maxFloorDistance;

        #endregion

        #region BackingFields

        protected NavMeshAgent agentBase;
        protected NavMeshDataInstance NavMeshInstance;
        protected Vector3[] npcPoints;

        #endregion

        /// <summary>
        /// Gets the base <see cref="NavMeshAgent"/>.
        /// <br>It's recommended to not directly interact with the base object unless it's needed.</br>
        /// </summary>
        public NavMeshAgent Base
        {
            get
            {
                if (agentBase == null)
                {
                    agentBase = GameObject.TryGetComponent(out NavMeshAgent agent) ? agent : AddComponent<NavMeshAgent>();
                    return agentBase;
                }

                TransferAgent(this, agentBase);
                return agentBase;
            }
        }

        /// <summary>
        /// Fired when the agent is changed.
        /// </summary>
        [DynamicEventDispatcher]
        protected DynamicEventDispatcher AgentChangedDispatcher { get; set; }

        /// <summary>
        /// Fired when the agent mesh is changed.
        /// </summary>
        [DynamicEventDispatcher]
        protected DynamicEventDispatcher AgentMeshChangedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the type ID for the agent.
        /// </summary>
        public int AgentTypeID
        {
            get => agentTypeID;
            set
            {
                Base.agentTypeID = agentTypeID = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the avoidance radius for the agent.
        /// </summary>
        public float Radius
        {
            get => radius;
            set
            {
                Base.radius = radius = value;
                AgentMeshChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the height of the agent for path finding purposes.
        /// </summary>
        public float Height
        {
            get => height;
            set
            {
                Base.height = height = value;
                AgentChangedDispatcher.InvokeAll();
                AgentMeshChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the layer mask which specifies which <see cref="NavMesh"/> areas are passable.
        /// <para>Changing the <see cref="AreaMask"/> willl make the path stale.</para>
        /// </summary>
        public LayerMask AreaMask
        {
            get => walkableMask;
            set
            {
                Base.areaMask = walkableMask = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the maximum floor distance.
        /// </summary>
        public float MaxFloorDistance
        {
            get => maxFloorDistance;
            set => maxFloorDistance = value;
        }

        /// <summary>
        /// Gets or sets the max acceleration when following a path.
        /// </summary>
        public float MaxAcceleration
        {
            get => maxAcceleration;
            set
            {
                Base.acceleration = maxAcceleration = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the maximum agent angular velocity.
        /// </summary>
        public float Torque
        {
            get => maxAngularSpeed;
            set
            {
                Base.angularSpeed = maxAngularSpeed = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the agent max speed.
        /// </summary>
        public float MaxSpeed
        {
            get => maxSpeed;
            set
            {
                Base.speed = maxSpeed = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the stopping distance from the target position.
        /// </summary>
        public float StoppingDistance
        {
            get => stoppingDistance;
            set
            {
                Base.stoppingDistance = stoppingDistance = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the agent should move across <see cref="OffMeshLink"/>s automatically.
        /// </summary>
        public bool AutoTraverseOffMeshLink
        {
            get => autoTraverseOffMeshLink;
            set
            {
                Base.autoTraverseOffMeshLink = autoTraverseOffMeshLink = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the agent should brake automatically to avoid overshooting the destination point.
        /// </summary>
        public bool AutoBraking
        {
            get => autobraking;
            set
            {
                Base.autoBraking = autobraking = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the agent should acquire a new patch if the existing one becomes invalid.
        /// </summary>
        public bool AutoRepath
        {
            get => autoRepath;
            set
            {
                Base.autoRepath = autoRepath = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the relative vertical displacement of the owning <see cref="GameObject"/>.
        /// </summary>
        public float BaseOffset
        {
            get => baseOffset;
            set
            {
                Base.baseOffset = baseOffset = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the quality level of the avoidance.
        /// </summary>
        public ObstacleAvoidanceType ObstacleAvoidanceType
        {
            get => obstacleAvoidanceType;
            set
            {
                Base.obstacleAvoidanceType = obstacleAvoidanceType = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the agent should update the transform orientation.
        /// </summary>
        public bool UpdateRotation
        {
            get => Base.updateRotation;
            set => Base.updateRotation = value;
        }

        /// <summary>
        /// Gets or sets the avoidance priority level.
        /// </summary>
        public int AvoidancePriority
        {
            get => avoidancePriority;
            set
            {
                Base.avoidancePriority = avoidancePriority = value;
                AgentChangedDispatcher.InvokeAll();
            }
        }

        /// <summary>
        /// Gets or sets the walk point range.
        /// </summary>
        public float WalkPointRange
        {
            get => walkPointRange;
            set => walkPointRange = value;
        }

        /// <summary>
        /// Gets or sets the next walk point.
        /// </summary>
        public Vector3 NextWalkPoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the agent has a walk point.
        /// </summary>
        public bool HasWalkPoint { get; set; }

        /// <summary>
        /// Gets or sets the current agent path.
        /// </summary>
        public NavMeshPath CurrentPath
        {
            get => Base.path;
            set => Base.path = value;
        }

        /// <summary>
        /// Gets or sets the destination of the agent in world-space units.
        /// </summary>
        public Vector3 Destination
        {
            get => Base.destination;
            set => Base.destination = value;
        }

        /// <summary>
        /// Gets or sets the agent velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get => Base.velocity;
            set => Base.velocity = value;
        }

        /// <summary>
        /// Gets or sets the simulation position of the agent.
        /// </summary>
        public Vector3 NextPosition
        {
            get => Base.nextPosition;
            set => Base.nextPosition = value;
        }

        /// <summary>
        /// Gets or sets the rate at which the Nav Mesh generation will be baked.
        /// </summary>
        public float NavMeshGenBakingRate { get; set; } = 2.5f;

        /// <summary>
        /// Gets or sets the rate at which the Nav Mesh Path generation will be baked.
        /// </summary>
        public float NavMeshPathGenBakingRate { get; set; } = 1.5f;

        /// <summary>
        /// Transfers the parameters of a <see cref="GenNavMeshAgentComponent"/> to a <see cref="NavMeshAgent"/>.
        /// </summary>
        /// <param name="sourceAgent">The <see cref="GenNavMeshAgentComponent"/> to copy.</param>
        /// <param name="destAgent">The destination <see cref="NavMeshAgent"/>.</param>
        public static void TransferAgent(GenNavMeshAgentComponent sourceAgent, NavMeshAgent destAgent)
        {
            destAgent.agentTypeID = sourceAgent.AgentTypeID;
            destAgent.radius = sourceAgent.Radius;
            destAgent.height = sourceAgent.Height;
            destAgent.areaMask = sourceAgent.AreaMask;
            destAgent.speed = sourceAgent.MaxSpeed;
            destAgent.acceleration = sourceAgent.MaxAcceleration;
            destAgent.angularSpeed = sourceAgent.Torque;
            destAgent.stoppingDistance = sourceAgent.StoppingDistance;
            destAgent.autoTraverseOffMeshLink = sourceAgent.AutoTraverseOffMeshLink;
            destAgent.autoBraking = sourceAgent.AutoBraking;
            destAgent.autoRepath = sourceAgent.AutoRepath;
            destAgent.baseOffset = sourceAgent.BaseOffset;
            destAgent.obstacleAvoidanceType = sourceAgent.ObstacleAvoidanceType;
            destAgent.avoidancePriority = sourceAgent.AvoidancePriority;
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            npcPoints = FindObjectsOfType<AINavigationPoint>().Select(wp => wp.Position).ToArray();
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Base.enabled = true;

            Timing.RunCoroutine(BakeNavMeshPath());
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            AgentChangedDispatcher.Bind(this, OnAgentChanged);
            AgentMeshChangedDispatcher.Bind(this, OnAgentMeshChanged);
        }

        /// <summary>
        /// Applies relative movement to the current agent position.
        /// <br>If a path already exists, it will be adjusted.</br>
        /// </summary>
        /// <param name="offset"></param>
        public virtual void Move(Vector3 offset)
        {
            if (!Base.enabled)
                return;

            Base.Move(offset);
        }

        /// <summary>
        /// Clears the current path.
        /// </summary>
        public virtual void ClearPath() => Base.ResetPath();

        /// <summary>
        /// Warps the agent to the specified position.
        /// <br>If a path already exists, it will not be reset.</br>
        /// </summary>
        /// <param name="position">The position to warp the agent to.</param>
        /// <returns><see langword="true"/> if the agent was successfully warped; otherwise, <see langword="false"/>.</returns>
        public virtual bool Warp(Vector3 position) => Base.Warp(position);

        /// <summary>
        /// Sets or updates the destination.
        /// <br>A new path will be calculated.</br>
        /// </summary>
        /// <param name="target">The target point to navigate to.</param>
        /// <returns><see langword="true"/> if the destination was successfully requested; otherwise, <see langword="false"/>.</returns>
        public virtual bool SetDestination(Vector3 target) => Base.SetDestination(target);

        /// <summary>
        /// Gets a smooth path given the specified path.
        /// </summary>
        /// <param name="path">The path to curve.</param>
        /// <returns>The curved path.</returns>
        public virtual Vector3[] GetSmoothPath(Vector3[] path)
        {
            List<Vector3> newPath = new();
            for (int i = 1; i < path.Length; i++)
            {
                Vector3 vec = path[i];
                if (NavMesh.FindClosestEdge(vec, out NavMeshHit hit, NavMesh.AllAreas))
                {
                    newPath.Add(vec + hit.normal);
                    continue;
                }
                
                newPath.Add(vec);
            }

            return newPath.ToArray();
        }

        /// <summary>
        /// Calculates the next walk point.
        /// </summary>
        public virtual void CalculateWalkPoint()
        {
            if (npcPoints.Length > 0)
            {
                NextWalkPoint = npcPoints[UnityEngine.Random.Range(0, npcPoints.Length)];
            }
            else
            {
                float roll = Random.Range(-WalkPointRange, WalkPointRange);
                float yaw = Random.Range(-WalkPointRange, WalkPointRange);

                NextWalkPoint = new(Position.x + yaw, Position.y, Position.z + roll);
            }

            if (Physics.Raycast(NextWalkPoint, -Transform.up, MaxFloorDistance))
                HasWalkPoint = true;
        }

        /// <summary>
        /// A basic implementation of how the agent should patrol the specified position.
        /// </summary>
        public virtual void SimplePatrol()
        {
            if (!HasWalkPoint)
                CalculateWalkPoint();
            else
            {
                if ((Position - NextWalkPoint).magnitude < StoppingDistance)
                    HasWalkPoint = false;
            }
        }

        #region Callbacks

        /// <inheritdoc cref="AgentChangedDispatcher"/>
        protected virtual void OnAgentChanged()
        {
        }

        /// <inheritdoc cref="AgentMeshChangedDispatcher"/>
        protected virtual void OnAgentMeshChanged()
        {
        }

        #endregion

        #region Internal

        private IEnumerator<float> BakeNavMesh()
        {
            yield return Timing.WaitForSeconds(NavMeshGenBakingRate);

            if (!TryGetComponent(out NavMeshSurface surface))
            {
                if (NavMeshInstance.valid)
                    NavMeshInstance.Remove();

                NavMeshData data = new(surface.agentTypeID);
                yield return Timing.WaitUntilTrue(() => surface.UpdateNavMesh(data).isDone);
                NavMesh.RemoveAllNavMeshData();
                NavMeshInstance = NavMesh.AddNavMeshData(data);
                NavMeshInstance.owner = this;
            }
        }

        private IEnumerator<float> BakeNavMeshPath()
        {
            NavMeshPath path = new();
            for(; ; )
            {
                if (!HasWalkPoint)
                {
                    yield return Timing.WaitForOneFrame;
                    continue;
                }

                yield return Timing.WaitForSeconds(NavMeshPathGenBakingRate);

                if (Base.CalculatePath(NextWalkPoint, path))
                    Base.path = path;
            }

        }

        #endregion
    }
}
