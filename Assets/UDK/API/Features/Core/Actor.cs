namespace UDK.API.Features.Core
{
    using UDK.API.Features.Core.Interfaces;
    using System;
    using UnityEngine;
    using UDK.API.Features.Core.Generic;
    using UDK.API.Events;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// The base class used to define in-game components.
    /// </summary>
    [AddComponentMenu("UDK/Core/Actor")]
    [ExecuteAlways]
    public abstract class Actor : TypeCastMono<Actor>, ITransform
    {
        /// <summary>
        /// A delegate which can be used to register callback methods to be invoked
        /// <br>before this component gets destroyed when "Remove Component" is clicked in the prefab asset inspector.</br>
        /// </summary>
        public delegate void OnPrefabAssetRemoveComponent(GameObject root);

        /// <summary>
        /// Set a function to be called before this component gets destroyed when "Remove Component" is clicked in the prefab asset inspector.
        /// </summary>
        public OnPrefabAssetRemoveComponent OnPrefabAssetRemoveComponentDel;

        /// <summary>
        /// Gets the component's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public virtual GameObject GameObject => gameObject;

        /// <inheritdoc/>
        public virtual Transform Transform => transform;

        /// <inheritdoc/>
        public virtual Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }

        /// <inheritdoc/>
        public virtual Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }

        /// <inheritdoc/>
        public virtual Vector3 Scale
        {
            get => Transform.localScale;
            set => Transform.localScale = value;
        }

        /// <summary>
        /// Safely gets the specified component.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/> to check.</param>
        /// <returns>The corresponding component, or <see langword="null"/> if not found.</returns>
        public static T GetComponentSafe<T>(GameObject gameObject)
            where T : Component
        {
            T comp;
            if (comp = gameObject.GetComponent<T>())
                return comp;

            if (comp = gameObject.GetComponentInChildren<T>())
                return comp;

            return gameObject.GetComponentInParent<T>();
        }

        /// <summary>
        /// Safely gets the specified components.
        /// </summary>
        /// <typeparam name="T">The type of the components.</typeparam>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/> to check.</param>
        /// <returns>The corresponding components, or <see langword="null"/> if not found.</returns>
        public static IEnumerable<T> GetComponentsSafe<T>(GameObject gameObject)
            where T : Component => gameObject.GetComponents<T>().Concat(gameObject.GetComponentsInChildren<T>()).Concat(gameObject.GetComponentsInParent<T>());

        /// <summary>
        /// Safely gets the specified component.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <returns>The corresponding component, or <see langword="null"/> if not found.</returns>
        public T GetComponentSafe<T>()
            where T : Component
        {
            T comp;
            if (comp = GetComponent<T>())
                return comp;

            if (comp = GetComponentInChildren<T>())
                return comp;

            return GetComponentInParent<T>();
        }

        /// <summary>
        /// Safely gets the specified components.
        /// </summary>
        /// <typeparam name="T">The type of the components.</typeparam>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/> to check.</param>
        /// <returns>The corresponding components, or <see langword="null"/> if not found.</returns>
        public IEnumerable<T> GetComponentsSafe<T>()
            where T : Component => GetComponents<T>().Concat(GetComponentsInChildren<T>()).Concat(GetComponentsInParent<T>());

        /// <summary>
        /// Adds a component to the <see cref="Actor.GameObject"/>
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <returns>The added component.</returns>
        public T AddComponent<T>()
            where T : Component => GameObject.AddComponent<T>();

        /// <summary>
        /// Adds a component to the <see cref="Actor.GameObject"/>
        /// </summary>
        /// <param name="type">The type of the component.</param>
        /// <returns>The added component.</returns>
        public Component AddComponent(Type type) => GameObject.AddComponent(type);

        /// <summary>
        /// Adds a component to the <see cref="Actor.GameObject"/>
        /// </summary>
        /// <typeparam name="T">The cast type.</typeparam>
        /// <param name="type">The type of the component.</param>
        /// <returns>The added component.</returns>
        public T AddComponent<T>(Type type)
            where T : Component => (T)AddComponent(type);

        /// <summary>
        /// Removes a component from the <see cref="Actor.GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <returns><see langword="true"/> if the component was removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveComponent<T>()
            where T : Component
        {
            if (TryGetComponent<T>(out T component))
            {
                Destroy(component);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a component from the <see cref="Actor.GameObject"/>.
        /// </summary>
        /// <param name="type">The type of the component.</param>
        /// <returns><see langword="true"/> if the component was removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveComponent(Type type)
        {
            if (TryGetComponent(type, out Component component))
            {
                Destroy(component);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Destroys the <see cref="Actor"/> instance.
        /// </summary>
        public virtual void Destroy()
        {
            try
            {
                Destroy(this);
            }
            catch (Exception ex)
            {
                print($"Couldn't destroy a {GetType().Name} instance: {ex}");
            }
        }

        /// <summary>
        /// Called from editor and play mode when the <see cref="Actor"/> is added, becomes enabled, is instantiated,
        /// <br> upon entering Play and prefab mode, on script reload and moving </br>
        /// game object to the Assets folder to create a prefab.
        /// <para>
        /// Only called if <see cref="UnityEngine.GameObject"/> is active and in the scene.
        /// </para>
        /// </summary>
        protected virtual void OnConstruct()
        {
        }

        /// <summary>
        /// Called from editor and play mode when the <see cref="Actor"/> goes from enabled to disabled 
        /// <br>(<i>i.e. when toggling it off, removing it through the inspector, destroying it, on script reload,</i></br>
        /// <br><i> upon leaving Play mode and moving <see cref="UnityEngine.GameObject"/> to the Assets folder to create a prefab.</i>)</br>
        /// <para>
        /// Only called if <see cref="UnityEngine.GameObject"/> is active and in the scene.
        /// </para>
        /// </summary>
        protected virtual void OnDeconstruct()
        {
        }

        /// <summary>
        /// Fired when the <see cref="Actor"/> is being loaded.
        /// </summary>
        protected virtual void OnPostInitialize()
        {
        }

#if UNITY_EDITOR
        /// <summary>
        /// Fired when the <see cref="Actor"/> is being loaded or a value has been modified in the inspector. 
        /// </summary>
        protected virtual void OnValidating()
        {
        }
#endif

        /// <summary>
        /// Fired after the first tick.
        /// </summary>
        protected virtual void OnBeginPlay()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// Fired every frame.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        protected virtual void Tick(float deltaTime)
        {
        }

        /// <summary>
        /// Fired every fixed framerate frame.
        /// </summary>
        /// <param name="fixedDeltaTime">The fixed delta time.</param>
        protected virtual void FixedTick(float fixedDeltaTime)
        {
        }

        /// <summary>
        /// Fired every tick after all the Update methods.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        protected virtual void LateTick(float deltaTime)
        {
        }

        /// <summary>
        /// Fired before the <see cref="Actor"/> instance is destroyed.
        /// </summary>
        protected virtual void OnEndPlay() => UnsubscribeEvents();

        /// <summary>
        /// Fired after the first tick.
        /// </summary>
        protected virtual void SubscribeEvents() => EventManager.CreateFromTypeInstance(this);

        /// <summary>
        /// Fired before the <see cref="Actor"/> instance is destroyed.
        /// </summary>
        protected virtual void UnsubscribeEvents() => EventManager.UnbindAllFromTypeInstance(this);

        /// <summary>
        /// Fired every editor frame.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        protected virtual void EditorTick()
        {
        }

        protected void OnEnable()
        {
#if UNITY_EDITOR
            if (Time.frameCount == 0 && !Application.isPlaying)
                return;
#endif
            OnConstruct();
        }

        protected void OnDisable()
        {
#if UNITY_EDITOR
            if (Time.frameCount == 0 && !Application.isPlaying)
                return;
#endif
            OnDeconstruct();
        }

        protected void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            OnPostInitialize();
        }

#if UNITY_EDITOR
        protected void OnValidate() => OnValidating();
#endif

        protected void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            OnBeginPlay();
        }

        protected void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorTick();
                return;
            }
#endif
            Tick(Time.deltaTime);
        }

        protected void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            FixedTick(Time.fixedDeltaTime);
        }

        protected void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            LateTick(Time.deltaTime);
        }

        protected void OnDestroy()
        {
#if UNITY_EDITOR
            if (Time.frameCount == 0 || !gameObject.activeInHierarchy)             
                return;
#endif
            OnEndPlay();
        }
    }
}
