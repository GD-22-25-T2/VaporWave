namespace UDK.API.Features.Core.Generic
{
    using UDK.API.Features.Core.Interfaces;

    /// <summary>
    /// The interface which allows defined objects to be cast to each other.
    /// </summary>
    /// <typeparam name="T">The type of the object to cast.</typeparam>
    public abstract class TypeCastObject<T> : ITypeCast<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCastObject{T}"/> class.
        /// </summary>
        protected TypeCastObject()
        {
        }

        /// <inheritdoc/>
        public TObject Cast<TObject>()
            where TObject : class => this as T as TObject;

        /// <inheritdoc/>
        public bool Cast<TObject>(out TObject param)
            where TObject : class
        {
            param = default;

            if (this as TObject is not TObject cast)
                return false;

            return (param = cast) is not null;
        }

        /// <inheritdoc/>
        public TObject As<TObject>()
            where TObject : class => Cast<TObject>();

        /// <inheritdoc/>
        public bool Is<TObject>(out TObject param)
            where TObject : class => Cast(out param);
    }
}
