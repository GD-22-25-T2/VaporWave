namespace UDK.API.Features.Core.Framework
{
    using UnityEngine;
    using UDK.Events.DynamicEvents;

    /// <summary>
    /// The base class which keeps track of the pawn's game state.
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/PlayerState")]
    [DisallowMultipleComponent]
    public class PlayerState : PawnComponent
    {
        protected Pawn pawn;
        protected string oldPlayerName;
        protected string playerName;
        protected string customPlayerName;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when the player name is changed.
        /// </summary>
        [DynamicEventDispatcher]
        protected TDynamicEventDispatcher<string> PlayerNameChanged { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether custom player names should be used.
        /// </summary>
        public virtual bool UseCustomPlayerNames { get; set; }

        /// <summary>
        /// Gets or sets the controller's name.
        /// </summary>
        public virtual string PlayerName
        {
            get => UseCustomPlayerNames ? customPlayerName : playerName;
            set
            {
                string oldName;
                if (UseCustomPlayerNames)
                {
                    oldName = customPlayerName;
                    customPlayerName = value;
                    PlayerNameChanged.InvokeAll(oldName);
                    return;
                }

                oldName = playerName;
                playerName = value;
                PlayerNameChanged.InvokeAll(oldName);
            }
        }

        /// <summary>
        /// Sets the player name without firing any delegates.
        /// </summary>
        /// <param name="value">The name to be set.</param>
        public void SetPlayerNameInternal(string value)
        {
            if (UseCustomPlayerNames)
            {
                customPlayerName = value;
                return;
            }

            playerName = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the controller is a bot.
        /// </summary>
        public virtual bool IsABot { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the controller is a spectatoor.
        /// </summary>
        public virtual bool IsSpectator { get; set; }

        /// <summary>
        /// Gets or sets the controller's id.
        /// </summary>
        public virtual int PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the controller's score.
        /// </summary>
        public virtual float Score { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public virtual int StartTime { get; set; }

        /// <summary>
        /// Gets or sets the old controller's name.
        /// </summary>
        public virtual string OldPlayerName
        {
            get => oldPlayerName;
            set => oldPlayerName = value;
        }

        /// <summary>
        /// Fired when the controller's name is changed.
        /// </summary>
        /// <param name="oldName">The old controller's name.</param>
        protected virtual void OnPlayerNameChanged(string oldName) => oldPlayerName = oldName;
    }
}
