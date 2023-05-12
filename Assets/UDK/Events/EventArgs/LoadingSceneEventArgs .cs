namespace UDK.API.Events.EventArgs
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Fired as soon as the scene is being loaded.
    /// </summary>
    public class LoadingSceneEventArgs : EventArgs
    {
        private static int _nextId = 1;
        private long _loadStartTime, _loadLoadEndTime;
        private int _id;
        private Action<LoadingSceneEventArgs> _loadSceneStepCallback;
        private AsyncOperation _asyncOp;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingSceneEventArgs"/> class.
        /// </summary>
        /// <param name="sceneName"><inheritdoc cref="SceneName"/></param>
        /// <param name="loadAdditive"><inheritdoc cref="IsSceneLoadingAdditive"/></param>
        /// <param name="loadAsync"><inheritdoc cref="IsSceneLoadingAsync"/></param>
        /// <param name="loadSceneStepCallback"><inheritdoc cref="_loadSceneStepCallback"/></param>
        public LoadingSceneEventArgs(string sceneName, bool loadAdditive, bool loadAsync, Action<LoadingSceneEventArgs> loadSceneStepCallback)
        {
            if (string.IsNullOrEmpty(sceneName))
                throw new ArgumentNullException("sceneName");

            _id = _nextId++;
            _loadStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _loadSceneStepCallback = loadSceneStepCallback;

            SceneName = sceneName;
            IsSceneLoadingAdditive = loadAdditive;
            IsSceneLoadingAsync = loadAsync;
        }

        /// <summary>
        /// Gets the name of the scene which is being loaded.
        /// </summary>
        public string SceneName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the scene was loaded additive.
        /// <para>
        /// Contents of the newly scene are added to the scene currently playing.
        /// </para>
        /// </summary>
        public bool IsSceneLoadingAdditive { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the scene was loaded asyncronously.
        /// </summary>
        public bool IsSceneLoadingAsync { get; private set; }

        /// <summary>
        /// Gets the loading progress percent.
        /// <para>
        /// Range is from 0 to 1.
        /// </para>
        /// </summary>
        public float LoadingProgress { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the scene was loaded in memory. 
        /// <para>
        /// Note: for async loaded scenes, the scene may be loaded in memory but not activated meaning,
        /// <br>the scene contents are not available yet in the scene tree.</br>
        /// </para>
        /// </summary>
        public bool IsSceneInMemory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the scene was loaded and ready to use.
        /// </summary>
        public bool IsSceneLoaded => IsSceneInMemory && IsSceneActivated;

        /// <summary>
        /// Gets a value indicating whether the scene is active in unity. Call <see cref="ActivateScene()"/> to actually activate a loaded scene.
        /// </summary>
        public bool IsSceneActivated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the scene will activate automatically after loading.
        /// </summary>
        public bool IsSceneActivatedOnLoad => _asyncOp == null ? true : _asyncOp.allowSceneActivation;

        /// <summary>
        /// Gets the time in milliseconds that took the scene to load
        /// </summary>
        public long ElapsedTime
        {
            get
            {
                if (IsSceneInMemory) return _loadLoadEndTime - _loadStartTime;
                return (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _loadStartTime;
            }
        }

        /// <summary>
        /// Gets the time when the scene finished loading.
        /// </summary>
        public long LoadEndTime => IsSceneInMemory ? _loadLoadEndTime : 0;

        /// <summary>
        /// Call this method for scenes loaded asynchronously to activate the scene in unity and load its contents into the scene tree.
        /// </summary>
        /// <remarks>
        /// You can call this method even if the scene has not finished loading, in which case the scene will be activated after is finished loading,
        /// <br>effectively behaving as if the IsSceneActivatedOnLoad flag were set to true.</br>
        /// </remarks>
        public void ActivateScene()
        {
            if (_asyncOp == null) return;
            _asyncOp.allowSceneActivation = true;
        }

        internal void SaveAsyncOperationRef(UnityEngine.AsyncOperation asyncOp) => _asyncOp = asyncOp;

        internal void NotifySceneLoadingProgress(float progress)
        {
            if (IsSceneInMemory) throw new Exception("Scene load finished, Progress update not allowed");

            LoadingProgress = progress;

            OnLoadSceneStep();
        }

        internal void NotifySceneLoaded()
        {
            IsSceneInMemory = true;
            LoadingProgress = 1f;

            OnLoadSceneStep();
        }

        internal void NotifySceneLoadedAndActive()
        {
            if (IsSceneInMemory)
                return;

            IsSceneInMemory = true;
            IsSceneActivated = true;

            LoadingProgress = 1f;
            _loadLoadEndTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            _asyncOp = null;

            OnLoadSceneStep();
        }

        public override string ToString()
        {
            return string.Format("Scene \"{0}\"{1}{2} Id:{3}",
                SceneName,
                IsSceneLoadingAdditive ? " Additive" : string.Empty,
                IsSceneLoadingAsync ? " Async" : string.Empty,
                _id);
        }

        private void OnLoadSceneStep()
        {
            if (_loadSceneStepCallback != null)
                _loadSceneStepCallback(this);
        }
    }
}
