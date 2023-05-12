namespace UDK.API.Features.Core.Interfaces
{
    /// <summary>
    /// The interface which allows defined objects to be cast to each other.
    /// </summary>
    /// <typeparam name="T">The type of the object to cast.</typeparam>
    public interface ITypeCast<T>
    {
        /// <summary>
        /// Unsafely casts the current <typeparamref name="T"/> instance to the specified <typeparamref name="TObject"/> type.
        /// </summary>
        /// <typeparam name="TObject">The type to which to cast the <typeparamref name="T"/> instance.</typeparam>
        /// <returns>The cast <typeparamref name="T"/> instance.</returns>
        public TObject Cast<TObject>() where TObject : class;

        /// <summary>
        /// Safely casts the current <typeparamref name="TObject"/> instance to the specified <typeparamref name="TObject"/> type.
        /// </summary>
        /// <typeparam name="TObject">The type to which to cast the <typeparamref name="TObject"/> instance.</typeparam>
        /// <param name="param">The cast object.</param>
        /// <returns><see langword="true"/> if the <typeparamref name="TObject"/> instance was successfully cast; otherwise, <see langword="false"/>.</returns>
        public bool Cast<TObject>(out TObject param) where TObject : class;

        /// <inheritdoc cref="Cast{T}"/>
        public TObject As<TObject>() where TObject : class;

        /// <inheritdoc cref="Cast{T}(out T)"/>
        public bool Is<TObject>(out TObject param) where TObject : class;
    }
}
