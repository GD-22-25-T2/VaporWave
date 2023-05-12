namespace UDK.API.Features.Core.Framework.WorldManager
{
    using UnityEngine;
    using System.Collections.Generic;
    using UDK.Events.DynamicEvents;

    /// <summary>
    /// The base class which keeps track of the current game mode data, allowing clients to communicate with the world context.
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/WorldManager/GameStateBase")]
    [DisallowMultipleComponent]
    public abstract class GameStateBase : Actor
    {
        protected readonly HashSet<PlayerState> playerStates = new();

        /// <summary>
        /// Gets all active player states.
        /// </summary>
        public IEnumerable<PlayerState> PlayerStates => playerStates;

        /// <summary>
        /// Gets the spectator class.
        /// </summary>
        public TSubclassOf<Pawn> SpectatorClass { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DynamicEventDispatcher"/> which handles start game.
        /// </summary>
        [DynamicEventDispatcher]
        public DynamicEventDispatcher StartGameDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="DynamicEventDispatcher"/> which handles end game.
        /// </summary>
        [DynamicEventDispatcher]
        public DynamicEventDispatcher EndGameDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the match has started.
        /// </summary>
        public bool? HasMatchStarted => World.GetWorld()?.GameManager?.HasMatchStarted;

        /// <summary>
        /// Gets or sets a value indicating whether the match has ended.
        /// </summary>
        public bool? HasMatchEnded => World.GetWorld()?.GameManager?.HasMatchEnded;

        /// <summary>
        /// Gets or sets the respawn delay given a player controller.
        /// </summary>
        /// <param name="playerController">The player controller instance.</param>
        public float GetRespawnDelay(PlayerController playerController) => playerController.RespawnDelay;

        /// <summary>
        /// Gets the start time given a player state.
        /// </summary>
        /// <param name="playerState">The player state instance.</param>
        /// <returns></returns>
        public float GetStartTime(PlayerState playerState) => playerState.StartTime;

        /// <summary>
        /// Adds a player state to the active player states.
        /// </summary>
        /// <param name="playerState">The player state to be added.</param>
        /// <returns><see langword="true"/> if the player state was added successfully; otherwise, <see langwor="false"/>.</returns>
        public bool AddPlayerState(PlayerState playerState) => playerStates.Add(playerState);

        /// <summary>
        /// Removes a player state from the active player states.
        /// </summary>
        /// <param name="playerState">The player state to be removed.</param>
        /// <returns><see langword="true"/> if the player state was removed successfully; otherwise, <see langwor="false"/>.</returns>
        public bool RemovePlayerState(PlayerState playerState) => playerStates.Remove(playerState);
    }
}
