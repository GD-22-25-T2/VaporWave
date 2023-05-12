namespace UDK.API.Features.Core.Framework.WorldManager
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using UDK.API.Events.EventArgs;
    using UnityEngine.SceneManagement;
    using UDK.MEC;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Central point for loading scenes either syncronous or asynchronous.
    /// When loading scenes in an async way the SceneManager manages the loading process,
    /// so you don't need to spawn Coroutines.
    /// 
    /// <para>
    /// The SceneManager keeps track of:
    /// <br>- The name of the scene used to initialize the game (allows testing individual scenes along the project depending on which scene you started from).</br>
    /// <br>- The names of the scenes loaded in the game and the order in which they were loaded. You'll need to put the inspector in debug mode to check this information.</br>
    /// </para>
    /// 
    /// <para>
    /// When loading a new scene you can be notified of the progress using these events:
    /// <br>- SceneWillLoad: invoked when the SceneManager starts to load the scene.</br>
    /// <br>- SceneDidLoad: invoked when the SceneManager finished loading the scene and the elements of the scene are accesible.</br>
    /// <br>- AsyncSceneLoadingProgress: invoked periodically to notify the load progress when a scene is being loaded asynchronous.</br>
    /// </para>
    /// 
    /// When loading an scene in an asynchronous way, the method will return a <see cref="Events.EventArgs.LoadingSceneEventArgs"/>
    /// <br>instance, that invokes events notifying only if the scene is loading or if the scene finished loading.</br>
    /// <br>This serves as a simple way to attach a listener if you only are interested on the events related to that scene.</br>
    /// <br>It is similar to the AsyncOperation instance returned by the Application.</br>
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/WorldManager/LevelManager")]
    [DisallowMultipleComponent]
    public class LevelManager : StaticActor<LevelManager>
    {
        [SerializeField]
        [Range(0.8f, 1f)]
        private float _percentTriggerSceneLoaded = 0.9f;

        private string _initialSceneName;
        private int _initialSceneIndex;
        List<string> _loadedScenes = new();

        /// <summary>
        /// Gets the current <see cref="LoadingSceneEventArgs"/>.
        /// </summary>
        public LoadingSceneEventArgs CurrentLoadingSceneArgs { get; set; }

        /// <summary>
        /// Invoked when the <see cref="LevelManager"/> starts to load the scene
        /// </summary>
        public event EventHandler<LoadingSceneEventArgs> SceneWillLoad;

        /// <summary>
        /// Invoked when the <see cref="LevelManager"/> finished loading the scene and the elements of the scene are accesible
        /// </summary>
        public event EventHandler<LoadingSceneEventArgs> SceneDidLoad;

        /// <summary>
        /// Gets the name of the scene used to initializea the game
        /// </summary>
        /// <value>The initial name of the scene.</value>
        public string InitialSceneName => _initialSceneName;

        /// <summary>
        /// Gets the index of the scene used to initialize the game
        /// </summary>
        /// <value>The initial name of the scene.</value>
        public int InitialSceneIndex => _initialSceneIndex;

        /// <summary>
        /// Gets a value indicating whether we are currently loading a scene.
        /// </summary>
        public bool IsLoadingScene => CurrentLoadingSceneArgs is not null;

        /// <summary>
        /// Gets the current loaded scenes.
        /// </summary>
        public ReadOnlyCollection<string> LoadedScenes => _loadedScenes.AsReadOnly();

        /// <summary>
        /// Adds the contents of the scene with the given name to the current scene and the waits one frame so all the newly loaded objects are actually accessible.
        /// <br>The SceneDidLoad event is fired after that frame has passed.</br>
        /// </summary>
        /// <param name="sceneName">
        /// The scene name which contents will be added to the current scene.
        /// </param>
        /// <param name="finishedCallback">
        /// This optional callback will be called when all the scenes have been loaded and one frame has passed,
        /// <br>which is the moment the contents of the scenes are actually accesible by code.</br>
        /// </param>
        public void AddScene(string sceneName, Action<LoadingSceneEventArgs> finishedCallback = null)
        {
            LoadingSceneEventArgs loadArgs = new(sceneName, true, false, finishedCallback);

            Log.Debug($"Start {loadArgs}");
            OnSceneWillLoad(loadArgs);

            SceneManager.LoadScene(loadArgs.SceneName, LoadSceneMode.Additive);

            Timing.RunCoroutine(WaitOneFrameUntilSceneIsAvailableCO(loadArgs));
        }

        /// <summary>
        /// Adds the contents of a list of scenes to the current scene. 
        /// <br>After all the scenes are loaded it waits one frame so all the newly loaded objects in all the scenes are accessible.</br>
        /// </summary>
        /// <param name="sceneNames">
        /// A list with the names of the scenes to be added to the current scene.
        /// <br>Scenes are added in the order they appear in the list.</br>
        /// </param>
        /// <param name="finishedCallback">
        /// This optional callback will be called when all the scenes have been loaded and one frame has passed,
        /// <br>which is the moment the contents of the scenes are actually accesible by code.</br>
        /// </param>
        public void AddScenes(IList<string> sceneNames, Action<LoadingSceneEventArgs> finishedCallback = null)
        {
            if (sceneNames.Count <= 0)
                return;

            int i = 0;
            LoadingSceneEventArgs loadArgs = null;

            do
            {
                loadArgs = new LoadingSceneEventArgs(sceneNames[i], true, false, finishedCallback);
                Log.Debug($"Start {loadArgs}");
                OnSceneWillLoad(loadArgs);
                SceneManager.LoadScene(loadArgs.SceneName, LoadSceneMode.Additive);
                CurrentLoadingSceneArgs = null;
                i++;

            } while (i < sceneNames.Count);

            CurrentLoadingSceneArgs = loadArgs;

            Timing.RunCoroutine(WaitOneFrameUntilSceneIsAvailableCO(loadArgs));
        }

        /// <summary>
        /// Adds the contents of the scene with the given name to the current scene in an asynchronous process so all the newly loaded objects are actually accessible. 
        /// <br>The scene is activated after it has finished loading.</br>
        /// <br>The SceneDidLoad event is raised after that frame has passed.</br>
        /// </summary>
        /// <param name="sceneName">The scene name which contents will be added to the current scene.</param>
        /// <returns>
        /// An <see cref="LoadingSceneEventArgs"/> instance that allows subscribing to events related to this scene loading process.
        /// </returns>
        /// <param name="finishedCallback">
        /// This callback is called each frame until the scene is being loaded, so you can query the loading progress, using <see cref="LoadingSceneEventArgs.LoadingProgress"/>.
        /// <br>The callback is called one last time when the scene has actually finished loading, has been loaded, and the elements in the scene are accesible by code.</br>
        /// <br>The property <see cref="LoadingSceneEventArgs.IsSceneLoaded"/> can be queried to check in which of the two states we are.</br>
        /// </param>
        public void AddSceneAsync(string sceneName, Action<LoadingSceneEventArgs> finishedCallback = null) => AddSceneAsyncInternal(sceneName, true, finishedCallback);

        /// <summary>
        /// Adds the contents of the scene with the given name to the current scene in an asynchronous process
        /// so all the newly loaded objects are actually accessible.
        /// <br>The scene is NOT activated after it has finished loading, you need to use the LoadingSceneEventArgs instance passed in the callback</br>
        /// <br>method to manually activate the scene after it finished loading.</br>
        /// <para>
        /// The SceneDidLoad event is raised after one frame has passed after the activation of the scene
        /// This is equivalent to Application.LoadLevelAdditive() method and setting the
        /// <see cref="AsyncOperation.allowSceneActivation"/> property returned by that method call to false.
        /// </para>
        /// </summary>
        /// <param name="sceneName">
        /// The scene name which contents will be added to the current scene
        /// </param>
        /// <returns>
        /// A <see cref="LoadingSceneEventArgs"/> instance that allows subscribing to events related to this scene loading process.
        /// </returns>
        public void AddSceneAsyncWaitActivation(string sceneName, Action<LoadingSceneEventArgs> finishedCallback) => AddSceneAsyncInternal(sceneName, false, finishedCallback);

        /// <summary>
        /// Changes the current scene to a new specified scene.
        /// <para>
        /// All <see cref="GameObject"/>s in the current scene will be deleted (except those marked as DontDestroyOnLoad) and replaced by the contents of the new scene.
        /// </para>
        /// </summary>
        /// <param name="sceneName">The new scene to load.</param>
        /// <param name="finishedCallback">When the scene has been loaded and a frame has passed, this callback is called.</param>
        public void ChangeSceneTo(string sceneName, Action<LoadingSceneEventArgs> finishedCallback = null)
        {
            LoadingSceneEventArgs loadArgs = new(sceneName, false, false, finishedCallback);

            Log.Debug($"Start {loadArgs}");

            OnSceneWillLoad(loadArgs);

            SceneManager.LoadScene(loadArgs.SceneName);
            Timing.RunCoroutine(WaitOneFrameUntilSceneIsAvailableCO(loadArgs));
        }

        /// <summary>
        /// Changes the current scene to the new one specified in an asynchronous way.
        /// <br>All GameObjects in the current scene will be deleted (except those marked as DontDestroyOnLoad) and replaced by the contents of the new scene.</br>
        /// </summary>
        /// <param name="sceneName">The new scene to load.</param>
        /// <param name="activateOnLoad">
        /// If set to <c>true</c> the GameObjects in the scene will activate automatically
        /// when the scene is finished loading.
        /// </param>
        /// <param name="finishedCallback">
        /// This callback is called each frame until the scene is loaded, so you can query the loading progress, using <see cref="LoadingSceneEventArgs.LoadingProgress"/>.
        /// </param>
        public void ChangeSceneToAsync(string sceneName, bool activateOnLoad, Action<LoadingSceneEventArgs> finishedCallback)
        {
            LoadingSceneEventArgs loadArgs = new(sceneName, false, true, finishedCallback);

            Log.Debug($"Start {loadArgs}");

            OnSceneWillLoad(loadArgs);

            // Wait one frame so the GameObjects of the loaded scene are accesible, and then
            // this scene will be added to the list of loaded scenes
            Timing.RunCoroutine(LoadLevelAsyncCO(activateOnLoad));
        }

        /// <summary>
        /// Queries if the given scene is already loaded
        /// </summary>
        /// <param name="sceneName">Name of the scene to check.</param>
        /// <returns>
        /// <see langword="true"></see> if the given scene was loaded successfully; otherwise, <see langword="false"></see>.
        /// </returns>
        public bool IsSceneLoaded(string sceneName) => LoadedScenes.Contains(sceneName);

        /// <inheritdoc/>
        protected override void SingletonPostInitialize()
        {        
            _initialSceneName = SceneManager.GetActiveScene().name;
            _initialSceneIndex = SceneManager.GetActiveScene().buildIndex;
            _loadedScenes.Add(_initialSceneName);

            Log.Debug($"Game started with scene {_initialSceneName}");
        }

        private void AddSceneAsyncInternal(string sceneName, bool activateOnLoad, Action<LoadingSceneEventArgs> finishedCallback)
        {
            LoadingSceneEventArgs loadArgs = new(sceneName, true, true, finishedCallback);

            Log.Debug($"Start {loadArgs}");

            OnSceneWillLoad(loadArgs);

            Timing.RunCoroutine(LoadLevelAsyncCO(activateOnLoad));
        }

        private IEnumerator<float> LoadLevelAsyncCO(bool activateOnLoad)
        {
            AsyncOperation asyncOp = CurrentLoadingSceneArgs.IsSceneLoadingAdditive ?
                SceneManager.LoadSceneAsync(CurrentLoadingSceneArgs.SceneName, LoadSceneMode.Additive) :
                SceneManager.LoadSceneAsync(CurrentLoadingSceneArgs.SceneName);

            asyncOp.allowSceneActivation = activateOnLoad;
            CurrentLoadingSceneArgs.SaveAsyncOperationRef(asyncOp);

            while (!IsSceneLoaded(asyncOp))
            {
                CurrentLoadingSceneArgs.NotifySceneLoadingProgress(asyncOp.progress);

                yield return Timing.WaitForOneFrame;
            }

            if (!asyncOp.allowSceneActivation) CurrentLoadingSceneArgs.NotifySceneLoaded();

            while (!asyncOp.allowSceneActivation)
                yield return Timing.WaitForOneFrame;

            OnSceneDidLoad();
        }

        private bool IsSceneLoaded(AsyncOperation asyncOp) => asyncOp.allowSceneActivation ? asyncOp.isDone : asyncOp.progress >= this._percentTriggerSceneLoaded;

        private IEnumerator<float> WaitOneFrameUntilSceneIsAvailableCO(LoadingSceneEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            OnSceneDidLoad();
        }

        private void OnSceneDidLoad()
        {
            if (!CurrentLoadingSceneArgs.IsSceneLoadingAdditive)
                _loadedScenes.Clear();

            _loadedScenes.Add(CurrentLoadingSceneArgs.SceneName);

            LoadingSceneEventArgs justLoadedSceneArgs = CurrentLoadingSceneArgs;
            CurrentLoadingSceneArgs = null;

            justLoadedSceneArgs.NotifySceneLoadedAndActive();

            if (SceneDidLoad != null)
                SceneDidLoad(this, justLoadedSceneArgs);

            Debug.Log($"Loaded {justLoadedSceneArgs}");
        }

        private void OnSceneWillLoad(LoadingSceneEventArgs ev)
        {
            if (IsLoadingScene)
            {
                Debug.Log($"Could not load {ev}: Already loading {CurrentLoadingSceneArgs}");
                return;
            }

            CurrentLoadingSceneArgs = ev;

            if (SceneWillLoad != null)
                SceneWillLoad(this, CurrentLoadingSceneArgs);
        }
    }
}
