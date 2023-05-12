namespace UDK.API.Features.Core.Interfaces
{
    using UDK.API.Features.Core.Framework.StateLib;

    /// <summary>
    /// Defines the contract for basic state features.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets the state's id.
        /// </summary>
        public byte Id { get; }
        
        /// <summary>
        /// Gets the state's name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the state's description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Fired when entering the state.
        /// </summary>
        public abstract void OnEnter(StateController stateController);

        /// <summary>
        /// Fired when exiting the state.
        /// </summary>
        public abstract void OnExit(StateController stateController);
    }
}
