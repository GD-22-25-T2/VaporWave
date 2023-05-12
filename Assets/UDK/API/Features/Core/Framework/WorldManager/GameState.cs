namespace UDK.API.Features.Core.Framework.WorldManager
{
    using UnityEngine;

    /// <inheritdoc/>
    [AddComponentMenu("UDK/Core/Framework/WorldManager/PlayerController")]
    [DisallowMultipleComponent]
    public abstract class GameState : GameStateBase
    {
        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        public int ElapsedTime => World.GetWorld().GameManager.TimerManager.Elapsed.Seconds;

        /// <summary>
        /// Gets the previous match state.
        /// </summary>
        public string PreviousMatchState => World.GetWorld().GameManager.Cast<GameState>().PreviousMatchState;

        /// <summary>
        /// Gets the match state.
        /// </summary>
        public string MatchState => World.GetWorld().GameManager.Cast<GameState>().MatchState;

        /// <summary>
        /// Gets a value indicating whether the match is in progress.
        /// </summary>
        public bool IsMatchInProgress
        {
            get
            {
                GameState gameState = World.GetWorld().GameManager.Cast<GameState>();
                return (gameState.HasMatchEnded.HasValue && !gameState.HasMatchEnded.Value) &&
                    (gameState.HasMatchStarted.HasValue && gameState.HasMatchStarted.Value);
            }
        }
    }
}
