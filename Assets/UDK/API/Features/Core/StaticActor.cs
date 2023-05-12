namespace UDK.API.Features.Core
{
    using System;
    using UnityEngine;

    /// <summary>
    /// This is a generic Singleton implementation for components.
    /// <br>Create a derived class where the type T is the script you want to "Singletonize"</br>
    /// <br>Upon loading it will call DontDestroyOnLoad on the gameobject where this script is contained so it persists upon scene changes.</br>
    /// </summary>
    /// <remarks>
    /// Do not redefine <see cref="OnPostInitialize()"/> <see cref="OnBeginPlay()"/> or <see cref="OnEndPlay()"/> in derived classes.
    /// <br>Instead, use <see langword="protected virtual"/> methods:</br>
    /// <br><see cref="SingletonPostInitialize()"/></br>
    /// <br><see cref="SingletonBeginPlay()"/></br>
    /// <br><see cref="SingletonEndPlay()"/></br>
    /// <para>
    /// To perform the initialization/cleanup: those methods are guaranteed to only be called once in the entire lifetime of the component.
    /// </para>
    /// </remarks>
    [AddComponentMenu("UDK/Core/ActorComponent")]
    public abstract class StaticActor<T> : Actor
        where T : Actor
    {
        private static T _instance;

        [Header("Debug")]
        [SerializeField]
        private bool _printTrace = false;

        /// <summary>
        /// Gets a value indicating whether the Singleton <see cref="OnPostInitialize()"/> method has already been called by Unity.
        /// </summary>
        public static bool IsAwakened { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Singleton <see cref="OnBeginPlay()"/> method has already been called by Unity.
        /// </summary>
        public static bool IsStarted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Singleton <see cref="OnEndPlay()"/> method has already been called by Unity.
        /// </summary>
        public static bool IsDestroyed { get; private set; }

        /// <summary>
        /// Gets the global access point to the unique instance of this class.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance)
                    return _instance;

                if (IsDestroyed)
                    return null;

                _instance = FindExistingInstance() ?? CreateNewInstance();

                return _instance;
            }
        }

        private static T FindExistingInstance()
        {
            T[] existingInstances = FindObjectsOfType<T>();
            return existingInstances == null || existingInstances.Length == 0 ? null : existingInstances[0];
        }

        private static T CreateNewInstance()
        {
            GameObject containerGO = new("__" + typeof(T).Name + " (Singleton)");
            return containerGO.AddComponent<T>();
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            T ldarg_0 = GetComponent<T>();

            if (_instance == null)
            {
                _instance = ldarg_0;
                DontDestroyOnLoad(_instance.gameObject);
            }
            else if (ldarg_0 != _instance)
            {
                Log.Warn($"Found a duplicated instance of a Singleton with type {GetType().Name} in the GameObject {name} that will be ignored");
                NotifyInstanceRepeated();
                return;
            }

            if (!IsAwakened)
            {
                PrintLog($"Start() Singleton with type {GetType().Name} in the GameObject {name}");
                SingletonPostInitialize();
                IsAwakened = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            if (IsStarted)
                return;

            PrintLog($"Start() Singleton with type {GetType().Name} in the GameObject {name}");
            SingletonBeginPlay();
            IsStarted = true;
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            if (this != _instance)
                return;

            IsDestroyed = true;
            PrintLog($"Start() Singleton with type {GetType().Name} in the GameObject {name}");
            SingletonEndPlay();
        }

        /// <summary>
        /// Flushes the current actor.
        /// </summary>
        protected virtual void Flush() => Destroy();

        /// <summary>
        /// Fired on <see cref="OnPostInitialize()"/>.
        /// </summary>
        /// <remarks>
        /// This method will only be called once even if multiple instances of the singleton component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void SingletonPostInitialize()
        {
        }

        /// <summary>
        /// Fired on <see cref="OnBeginPlay()"/>.
        /// </summary>
        /// <remarks>
        /// This method will only be called once even if multiple instances of the singleton component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void SingletonBeginPlay()
        {
        }

        /// <summary>
        /// Fired on <see cref="OnEndPlay()"/>.
        /// </summary>
        /// <remarks>
        /// This method will only be called once even if multiple instances of the singleton component exist in the scene.
        /// <br>You can override this method in derived classes to customize the initialization of the component.</br>
        /// </remarks>
        protected virtual void SingletonEndPlay()
        {
        }

        /// <summary>
        /// If a duplicated instance of a Singleton component is loaded into the scene this method will be called instead of <see cref="SingletonPostInitialize()"/>.
        /// <br>That way you can customize what to do with repeated instances.</br>
        /// </summary>
        /// <remarks>
        /// The default approach is delete the duplicated component.
        /// </remarks>
        protected virtual void NotifyInstanceRepeated() => Destroy(GetComponent<T>());

        /// <summary>
        /// Prints a debug log.
        /// </summary>
        /// <param name="str">The message to print.</param>
        /// <param name="args">Defined arguments.</param>
        protected void PrintLog(string str, params object[] args) => Print(Debug.Log, _printTrace, str, args);

        /// <summary>
        /// Prints a warn log.
        /// </summary>
        /// <param name="str">The message to print.</param>
        /// <param name="args">Defined arguments.</param>
        protected void PrintWarn(string str, params object[] args) => Print(Debug.LogWarning, _printTrace, str, args);

        /// <summary>
        /// Prints an error log.
        /// </summary>
        /// <param name="str">The message to print.</param>
        /// <param name="args">Defined arguments.</param>
        protected void PrintError(string str, params object[] args) => Print(Log.Error, true, str, args);

        private void Print(Action<string> printFunc, bool doPrint, string str, params object[] args)
        {
            if (doPrint)
                printFunc($"<b>[{Time.frameCount}][{GetType().Name.ToUpper()}] {string.Format(str, args)} </b>");
        }
    }
}
