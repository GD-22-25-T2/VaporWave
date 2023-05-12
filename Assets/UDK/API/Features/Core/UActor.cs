namespace UDK.API.Features.Core
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using UDK.API.Features.Core.Framework.StateLib;
    using UDK.API.Features.Core.Interfaces;
    using UDK.MEC;
    using UDK.API.Features.Core.Framework.Pools;

    /// <summary>
    /// UActor is the base class for a <see cref="UObject"/> that can be placed or spawned in-game.
    /// </summary>
    public abstract class UActor : UObject, IEntityComponent, ITransform
    {
        /// <summary>
        /// The default fixed tick rate.
        /// </summary>
        public const float DefaultFixedTickRate = TickComponent.DefaultFixedTickRate;

        private readonly HashSet<UActor> componentsInChildren = HashSetPool<UActor>.Pool.Get();
        private CoroutineHandle clientTick;
        private bool canEverTick;
        private float fixedTickRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UActor"/> class.
        /// </summary>
        protected UActor()
            : base()
        {
            IsEditable = true;
            CanEverTick = true;
            fixedTickRate = DefaultFixedTickRate;
            PostInitialize();
            Timing.CallDelayed(fixedTickRate, () => OnBeginPlay());
            Timing.CallDelayed(fixedTickRate * 2, () => clientTick = Timing.RunCoroutine(ClientTick()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UActor"/> class.
        /// </summary>
        /// <param name="gameObject">The base <see cref="GameObject"/>.</param>
        protected UActor(GameObject gameObject = null)
            : this()
        {
            if (gameObject)
                Base = gameObject;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<UActor> ComponentsInChildren => componentsInChildren;

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets or sets the <see cref="Vector3">position</see>.
        /// </summary>
        public virtual Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Quaternion">rotation</see>.
        /// </summary>
        public virtual Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector3">scale</see>.
        /// </summary>
        public virtual Vector3 Scale
        {
            get => Transform.localScale;
            set => Transform.localScale = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="UActor"/> can tick.
        /// </summary>
        public virtual bool CanEverTick
        {
            get => canEverTick;
            set
            {
                if (!IsEditable)
                    return;

                canEverTick = value;

                if (canEverTick)
                {
                    Timing.ResumeCoroutines(clientTick);
                    return;
                }

                Timing.PauseCoroutines(clientTick);
            }
        }

        /// <summary>
        /// Gets or sets the value which determines the size of every tick.
        /// </summary>
        public virtual float FixedTickRate
        {
            get => fixedTickRate;
            set
            {
                if (!IsEditable)
                    return;

                fixedTickRate = value;
            }
        }

        /// <summary>
        /// Gets a <see cref="UActor"/>[] containing all the components in parent.
        /// </summary>
        protected IEnumerable<UActor> ComponentsInParent => FindActiveObjectsOfType<UActor>().Where(actor => actor.ComponentsInChildren.Any(comp => comp == this));

        /// <summary>
        /// Attaches a <see cref="UActor"/> to the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="comp"><see cref="UActor"/>.</param>
        /// <param name="gameObject"><see cref="GameObject"/>.</param>
        public static void AttachTo(UActor comp, GameObject gameObject) => comp.Base = gameObject;

        /// <summary>
        /// Attaches a <see cref="UActor"/> to the specified <see cref="UActor"/>.
        /// </summary>
        /// <param name="to">The actor to be modified.</param>
        /// <param name="from">The source actor.</param>
        public static void AttachTo(UActor to, UActor from) => to.Base = from.Base;

        /// <inheritdoc/>
        public T AddComponent<T>(string name = "")
            where T : UActor
        {
            T component = CreateDefaultSubobject<T>(Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<T>();
            if (component is null)
                return null;

            componentsInChildren.Add(component);
            return component.Cast<T>();
        }

        /// <inheritdoc/>
        public UActor AddComponent(Type type, string name = "")
        {
            UActor component = CreateDefaultSubobject(type, Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<UActor>();
            if (component is null)
                return null;

            componentsInChildren.Add(component);
            return component;
        }

        /// <inheritdoc/>
        public T AddComponent<T>(Type type, string name = "")
            where T : UActor => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public UActor GetComponent(Type type) => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType());

        /// <inheritdoc/>
        public T GetComponent<T>()
            where T : UActor => ComponentsInChildren.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public T GetComponent<T>(Type type)
            where T : UActor => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public bool TryGetComponent<T>(Type type, out T component)
            where T : UActor
        {
            UActor actor = GetComponent(type);

            if (actor.Cast(out component))
                component = actor.Cast<T>();

            return component;
        }

        /// <inheritdoc/>
        public bool TryGetComponent<T>(out T component)
            where T : UActor
        {
            component = null;

            if (HasComponent<T>())
                component = GetComponent<T>().Cast<T>();

            return component is not null;
        }

        /// <inheritdoc/>
        public bool TryGetComponent(Type type, out UActor component)
        {
            component = null;

            if (HasComponent(type))
                component = GetComponent(type);

            return component is not null;
        }

        /// <inheritdoc/>
        public bool HasComponent<T>(bool depthInheritance = false) => depthInheritance
            ? ComponentsInChildren.Any(comp => typeof(T).IsSubclassOf(comp.GetType()))
            : ComponentsInChildren.Any(comp => typeof(T) == comp.GetType());

        /// <inheritdoc/>
        public bool HasComponent(Type type, bool depthInheritance = false) => depthInheritance
            ? ComponentsInChildren.Any(comp => type.IsSubclassOf(comp.GetType()))
            : ComponentsInChildren.Any(comp => type == comp.GetType());

        /// <summary>
        /// Fired after the <see cref="UActor"/> instance is created.
        /// </summary>
        protected virtual void PostInitialize()
        {
        }

        /// <summary>
        /// Fired after the first fixed tick.
        /// </summary>
        protected virtual void OnBeginPlay()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// Fired every tick.
        /// </summary>
        protected virtual void Tick()
        {
        }

        /// <summary>
        /// Fired before the current <see cref="UActor"/> instance is destroyed.
        /// </summary>
        protected virtual void OnEndPlay()
        {
            UnsubscribeEvents();
        }

        /// <summary>
        /// Subscribes all the events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Unsubscribes all the events.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }

        /// <inheritdoc/>
        protected override void OnBeginDestroy()
        {
            base.OnBeginDestroy();

            HashSetPool<UActor>.Pool.Return(componentsInChildren);
            Timing.KillCoroutines(clientTick);

            OnEndPlay();
        }

        private IEnumerator<float> ClientTick()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(FixedTickRate);

                Tick();
            }
        }
    }
}
