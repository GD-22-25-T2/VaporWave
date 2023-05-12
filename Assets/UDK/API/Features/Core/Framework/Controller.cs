namespace UDK.API.Features.Core.Framework
{
    using UnityEngine;
    using UDK.API.Features.Core.Framework.WorldManager;

    /// <summary>
    /// The base class which defines a playable controller.
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/Controller")]
    [DisallowMultipleComponent]
    public abstract class Controller : PawnComponent
    {
        [SerializeField]
        protected string playerStartTag;

        /// <summary>
        /// Gets or sets a value indicating whether cheats are enabled.
        /// </summary>
        public virtual bool EnableCheats { get; set; }

        /// <summary>
        /// Gets or sets the player start tag.
        /// </summary>
        public virtual string PlayerStartTag
        {
            get => playerStartTag;
            set => playerStartTag = value;
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            GameStateBase gameState = World.GetWorld()?.GameManager?.GameState;
            if (gameState)
            {
                gameState.StartGameDispatcher.Bind(this, OnGameStarted);
                gameState.EndGameDispatcher.Bind(this, OnGameEnded);

                gameState.AddPlayerState(Owner.PlayerState);
            }
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            GameStateBase gameState = World.GetWorld()?.GameManager?.GameState;
            if (gameState)
                gameState.RemovePlayerState(Owner.PlayerState);
        }

        /// <summary>
        /// Clears out 'left-over' audio components.
        /// </summary>
        public virtual void CleanUpAudioComponents()
        {
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public virtual void Pause()
        {
        }

        /// <summary>
        /// Resets the controller.
        /// </summary>
        public virtual void ResetController()
        {
        }

        /// <summary>
        /// Fired when the game is started.
        /// </summary>
        protected virtual void OnGameStarted()
        {
        }

        /// <summary>
        /// Fired when the game is ended.
        /// </summary>
        protected virtual void OnGameEnded()
        {
        }
    }
}
