namespace UDK.API.Features.Core.Framework.Pools
{
    using UDK.API.Features.Core.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;

    /// <summary>
	/// Returns pooled <see cref="List{T}"/>.
	/// </summary>
	/// <typeparam name="T">Element type.</typeparam>
	public sealed class ListPool<T> : IPool<List<T>>
    {
        /// <summary>
        /// Gets a shared <see cref="ListPool{T}"/> instance.
        /// </summary>
        public static readonly ListPool<T> Shared = new();

        private readonly ConcurrentQueue<List<T>> _pool = new();

        private ListPool()
        {
        }

        /// <summary>
        /// Gets a <see cref="ListPool{T}"/> that stores lists.
        /// </summary>
        public static ListPool<T> Pool { get; } = new();

        /// <inheritdoc cref="Rent"/>
        public List<T> Get() => Shared.Rent();

        /// <inheritdoc cref="Return(List{T})"/>
        public void SharedReturn(List<T> obj) => Shared.Return(obj);

        /// <summary>
        /// Returns the <see cref="List{T}"/> to the pool and returns its contents as an array.
        /// </summary>
        /// <param name="obj">The <see cref="List{T}"/> to return.</param>
        /// <returns>The contents of the returned list as an array.</returns>
        public T[] ToArrayReturn(List<T> obj)
        {
            T[] array = obj.ToArray();
            Return(obj);
            return array;
        }

        /// <summary>
        /// Gives a pooled <see cref="List{T}"/>.
        /// </summary>
        /// <returns><see cref="List{T}"/> from the pool.</returns>
        public List<T> Rent() => _pool.TryDequeue(out List<T> list) ? list : new(512);

        /// <summary>
        /// Gives a pooled <see cref="List{T}"/> with provided capacity.
        /// </summary>
        /// <param name="capacity">Requested capacity.</param>
        /// <returns><see cref="List{T}"/> from the pool.</returns>
        public List<T> Rent(int capacity)
        {
            if (_pool.TryDequeue(out List<T> list))
            {
                if (list.Capacity < capacity)
                    list.Capacity = capacity;

                return list;
            }

            return new(Math.Max(capacity, 512));
        }

        /// <summary>
        /// Gives a pooled <see cref="List{T}"/> with initial content.
        /// </summary>
        /// <param name="enumerable">Initial content.</param>
        /// <returns><see cref="List{T}"/> from the pool.</returns>
        public List<T> Rent(IEnumerable<T> enumerable)
        {
            if (_pool.TryDequeue(out List<T> list))
            {
                list.AddRange(enumerable);
                return list;
            }

            return new(enumerable);
        }

        /// <summary>
        /// Returns a <see cref="List{T}"/> to the pool.
        /// </summary>
        /// <param name="list">Returned <see cref="List{T}"/>.</param>
        public void Return(List<T> list)
        {
            list.Clear();
            _pool.Enqueue(list);
        }

        /// <summary>
        /// Retrieves a stored object of type <see cref="List{T}"/>, or creates it if it does not exist. The capacity of the list will be equal to or greater than <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The capacity of content in the <see cref="List{T}"/>.</param>
        /// <returns>The stored object, or a new object, of type <see cref="List{T}"/>.</returns>
        public List<T> Get(int capacity) => Shared.Rent(capacity);

        /// <summary>
        /// Retrieves a stored object of type <see cref="List{T}"/>, or creates it if it does not exist. The list will be filled with all the provided <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to fill the list with.</param>
        /// <returns>The stored object, or a new object, of type <see cref="List{T}"/>.</returns>
        public List<T> Get(IEnumerable<T> items) => Shared.Rent(items);
    }
}
