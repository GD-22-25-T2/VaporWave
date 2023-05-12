namespace UDK.API.Features.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for basic actor component features.
    /// </summary>
    public interface IActorComponent<T>
        where T : class
    {
        /// <summary>
        /// Gets the root <see cref="Actor"/>.
        /// </summary>
        public T Owner { get; }

        /// <summary>
        /// Gets the root <see cref="Actor"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Actor"/>.</typeparam>
        /// <returns>The root <see cref="Actor"/>.</returns>
        public TObject GetOwner<TObject>() where TObject : T;
    }
}
