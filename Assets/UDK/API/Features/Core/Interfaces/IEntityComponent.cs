namespace UDK.API.Features.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the contract for basic UCS implementation.
    /// </summary>
    public interface IEntityComponent
    {
        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="UActor"/> containing all the components in children.
        /// </summary>
        abstract IReadOnlyCollection<UActor> ComponentsInChildren { get; }

        /// <summary>
        /// Adds a component to the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="UActor"/> to be added.</typeparam>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added <see cref="UActor"/> component.</returns>
        public abstract T AddComponent<T>(string name = "")
            where T : UActor;

        /// <summary>
        /// Adds a component to the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="UActor"/> to be added.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added <see cref="UActor"/> component.</returns>
        public abstract UActor AddComponent(Type type, string name = "");

        /// <summary>
        /// Adds a component from the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> cast <see cref="UActor"/> type.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="UActor"/> to be added.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added <see cref="UActor"/> component.</returns>
        public abstract T AddComponent<T>(Type type, string name = "")
            where T : UActor;

        /// <summary>
        /// Gets a component from the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="UActor"/> to look for.</typeparam>
        /// <returns>The <see cref="UActor"/> component.</returns>
        public abstract T GetComponent<T>()
            where T : UActor;

        /// <summary>
        /// Gets a component from the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <typeparam name="T">The cast <typeparamref name="T"/> <see cref="UActor"/>.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="UActor"/> to look for.</param>
        /// <returns>The <see cref="UActor"/> component.</returns>
        public abstract T GetComponent<T>(Type type)
            where T : UActor;

        /// <summary>
        /// Gets a component from the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="UActor"/> to look for.</param>
        /// <returns>The <see cref="UActor"/> component.</returns>
        public abstract UActor GetComponent(Type type);

        /// <summary>
        /// Tries to get a component from the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="UActor"/> to look for.</typeparam>
        /// <param name="component">The <typeparamref name="T"/> <see cref="UActor"/>.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool TryGetComponent<T>(out T component)
            where T : UActor;

        /// <summary>
        /// Tries to get a component from the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="UActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool TryGetComponent(Type type, out UActor component);

        /// <summary>
        /// Tries to get a component from the <see cref="IEntityComponent"/>.
        /// </summary>
        /// <typeparam name="T">The cast <typeparamref name="T"/> <see cref="UActor"/>.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="UActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool TryGetComponent<T>(Type type, out T component)
            where T : UActor;

        /// <summary>
        /// Checks if the <see cref="IEntityComponent"/> has an active component.
        /// </summary>
        /// <typeparam name="T">The <see cref="UActor"/> to look for.</typeparam>
        /// <param name="depthInheritance">A value indicating whether or not subclasses should be considered.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool HasComponent<T>(bool depthInheritance = false);

        /// <summary>
        /// Checks if the <see cref="IEntityComponent"/> has an active component.
        /// </summary>
        /// <param name="type">The <see cref="UActor"/> to look for.</param>
        /// <param name="depthInheritance">A value indicating whether or not subclasses should be considered.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool HasComponent(Type type, bool depthInheritance = false);
    }
}
