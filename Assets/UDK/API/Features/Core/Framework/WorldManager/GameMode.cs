namespace UDK.API.Features.Core.Framework.WorldManager
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UDK.API.Events.EventArgs;

    /// <summary>
    /// The class which handles game modes match-based.
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/WorldManager/GameMode")]
    [DisallowMultipleComponent]
    public class GameMode : GameModeBase
    {
        protected string matchState;
        
        /// <summary>
        /// Gets or sets the previous match state.
        /// </summary>
        public string PreviousMatchState { get; protected set; }

        /// <summary>
        /// Gets or sets the match state.
        /// </summary>
        public virtual string MatchState
        {
            get => matchState;
            set
            {
                string oldState = matchState;
                matchState = value;
                PreviousMatchState = oldState;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the match can be started.
        /// </summary>
        public virtual bool IsReadyToStartMatch { get; }

        /// <summary>
        /// Gets a value indicating whether the match can be ended.
        /// </summary>
        public virtual bool IsReadyToEndMatch { get; }

        /// <summary>
        /// Starts the match.
        /// </summary>
        public virtual void StartMatch()
        {
            HasMatchStarted = true;
            GameState?.StartGameDispatcher?.InvokeAll();
        }

        /// <summary>
        /// Ends the match.
        /// </summary>
        public virtual void EndMatch()
        {
            HasMatchEnded = true;
            GameState?.EndGameDispatcher?.InvokeAll();
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
        public virtual void RestartGame() => LevelManager.Instance.ChangeSceneTo(SceneManager.GetActiveScene().name, OnGameRestarted);

        /// <summary>
        /// Fired when the game is started.
        /// </summary>
        /// <param name="ev">The <see cref="LoadingSceneEventArgs"/> instance.</param>
        protected virtual void OnGameRestarted(LoadingSceneEventArgs ev)
        {
        }
    }
}
