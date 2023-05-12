namespace UDK.API.Features.Core.Framework.WorldManager
{
    using UnityEngine;
    using UDK.API.Events.EventArgs;
    using System.Diagnostics;
    using UDK.API.Features.Core.Framework.Navigation;
    using System.Linq;

    /// <summary>
    /// The base class which defines in-game rules.
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/WorldManager/GameModeBase")]
    [DisallowMultipleComponent]
    public abstract class GameModeBase : Actor
    {
        #region Editor

        [SerializeField]
        private bool _isPauseable;

        [SerializeField]
        private string _defaultPlayerName;

        [SerializeField]
        private TSubclassOf<Pawn> _defaultPawnClass = new();

        [SerializeField]
        private TSubclassOf<Pawn> _spectatorClass = new();

        [SerializeField]
        private TSubclassOf<GameStateBase> _gameStateClass = new();

        [SerializeField]
        private TSubclassOf<Controller> _playerControllerClass = new();

        #endregion

        #region BackingFields

        protected bool hasMatchStarted, hasMatchEnded;
        protected Stopwatch timerManager;
        private GameStateBase _gameState;

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the game mode is pauseable.
        /// </summary>
        public virtual bool IsPauseable
        {
            get => _isPauseable;
            set => _isPauseable = value;
        }

        /// <summary>
        /// Gets or sets the default player name.
        /// </summary>
        public virtual string DefaultPlayerName
        {
            get => _defaultPlayerName;
            set => _defaultPlayerName = value;
        }

        /// <summary>
        /// Gets or sets the default pawn class.
        /// </summary>
        public virtual TSubclassOf<Pawn> DefaultPawnClass
        {
            get => _defaultPawnClass;
            set => _defaultPawnClass = value;
        }

        /// <summary>
        /// Gets or sets the spectator pawn class.
        /// </summary>
        public virtual TSubclassOf<Pawn> SpectatorPawnClass
        {
            get => _spectatorClass;
            set => _spectatorClass = value;
        }

        /// <summary>
        /// Gets or sets the game state class.
        /// </summary>
        public virtual TSubclassOf<GameStateBase> GameStateClass
        {
            get => _gameStateClass;
            set => _gameStateClass = value;
        }

        /// <summary>
        /// Gets or sets the player controller class.
        /// </summary>
        public virtual TSubclassOf<Controller> PlayerControllerClass
        {
            get => _playerControllerClass;
            set => _playerControllerClass = value;
        }

        /// <summary>
        /// Gets or sets the game state.
        /// </summary>
        public virtual GameStateBase GameState
        {
            get => _gameState ? _gameState : _gameState = GetComponentSafe<GameStateBase>();
            protected set => _gameState = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the match has started.
        /// </summary>
        public virtual bool HasMatchStarted
        {
            get => hasMatchStarted;
            set
            {
                hasMatchStarted = value;
                hasMatchEnded = !hasMatchStarted;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the match has ended.
        /// </summary>
        public virtual bool HasMatchEnded
        {
            get => hasMatchEnded;
            set
            {
                hasMatchEnded = value;
                hasMatchStarted = !hasMatchEnded;
            }
        }

        /// <summary>
        /// Gets the timer manager.
        /// </summary>
        public virtual Stopwatch TimerManager => timerManager;

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            if (!GameState || GameState && GameState.GetType() != GameStateClass)
                GameState = AddComponent<GameStateBase>(GameStateClass);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the game is paused.
        /// </summary>
        public virtual bool IsPaused { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the specified player controller can restart.
        /// </summary>
        /// <param name="playerController">The player controller to check.</param>
        /// <returns><see langword="true"/> if the specified player controller can restart; otherwise, <see langword="false"/>.</returns>
        public virtual bool? PlayerCanRestart(PlayerController playerController) => playerController?.CanRestartPlayer;

        /// <summary>
        /// Initializes the game.
        /// </summary>
        /// <param name="levelName">The name of the level to load.</param>
        public virtual void InitGame(string levelName) => LevelManager.Instance.ChangeSceneTo(levelName, OnLoadingScene);

        /// <summary>
        /// Initializes the game state.
        /// </summary>
        public virtual void InitGameState()
        {
        }

        /// <summary>
        /// Gets a random <see cref="PlayerStart"/>.
        /// </summary>
        public PlayerStart RandomPlayerStart
        {
            get
            {
                PlayerStart[] startSpots = NavigationObjectBase.List.Where(@object => @object.Is(out PlayerStart _)).Cast<PlayerStart>().ToArray();
                return startSpots[UnityEngine.Random.Range(0, startSpots.Length)];
            }
        }

        /// <summary>
        /// Gets a random <see cref="PlayerStart"/> matching the specified controller's tag.
        /// </summary>
        /// <param name="controller">The controller to find the player start for.</param>
        /// <returns>A random <see cref="PlayerStart"/> matching the specified controller's tag.</returns>
        public PlayerStart FindPlayerStart(Controller controller)
        {
            PlayerStart[] startSpots = NavigationObjectBase.List
                .Where(@object => @object.Is(out PlayerStart playerStart) && playerStart.ControllerTag == controller.PlayerStartTag)
                .Cast<PlayerStart>()
                .ToArray();

            return startSpots[UnityEngine.Random.Range(0, startSpots.Length)];
        }

        /// <summary>
        /// Restarts the specified player controller.
        /// </summary>
        /// <param name="playerController">The player controller to restart.</param>
        public virtual void RestartPlayer(PlayerController playerController)
        {
        }

        /// <summary>
        /// Restarts the specified player controller given a start spot.
        /// </summary>
        /// <param name="playerController">The player controller to restart.</param>
        /// <param name="startSpot">The start spot to restart the player at.</param>
        public virtual void RestartPlayerAtPlayerStart(PlayerController playerController, GameObject startSpot)
        {
        }

        /// <summary>
        /// Restarts the specified player controller given a transform.
        /// </summary>
        /// <param name="playerController">The player controller to restart.</param>
        /// <param name="transform">The transform to restart the player at.</param>
        public virtual void RestartPlayerAtTransform(PlayerController playerController, Transform transform)
        {
        }

        /// <summary>
        /// Returns to main menu.
        /// </summary>
        public virtual void ReturnToMainMenu()
        {
        }

        /// <summary>
        /// Fired when the scene is changed.
        /// </summary>
        /// <param name="ev">The <see cref="LoadingSceneEventArgs"/> instance.</param>
        protected virtual void OnLoadingScene(LoadingSceneEventArgs ev)
        {
        }
    }
}
