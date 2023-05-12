namespace UDK.API.Features.Core.Framework.Pools
{
    using UDK.API.Features.Core.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.Concurrent;

    /// <summary>
    /// Returns pooled <see cref="HashSet{T}"/>.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public sealed class HashSetPool<T> : IPool<HashSet<T>>
    {

        /// <summary>
        /// Gets a shared <see cref="HashSetPool{T}"/> instance.
        /// </summary>
        public static readonly HashSetPool<T> Shared = new();

        private readonly ConcurrentQueue<HashSet<T>> _pool = new();

        private HashSetPool()
        {
        }

        /// <summary>
        /// Gets a <see cref="HashSetPool{T}"/> that stores hash sets.
        /// </summary>
        public static HashSetPool<T> Pool { get; } = new();

        /// <inheritdoc cref="Rent"/>
        public HashSet<T> Get() => Shared.Rent();

        /// <summary>
        /// Retrieves a stored object of type <see cref="HashSet{T}"/>, or creates it if it does not exist. The hashset will be filled with all the provided <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to fill the hashset with.</param>
        /// <returns>The stored object, or a new object, of type <see cref="HashSet{T}"/>.</returns>
        public HashSet<T> Get(IEnumerable<T> items) => Shared.Rent(items);

        /// <inheritdoc cref="Return(HashSet{T})"/>
        public void SharedReturn(HashSet<T> obj) => Shared.Return(obj);

        /// <summary>
        /// Returns the <see cref="HashSet{T}"/> to the pool and returns its contents as an array.
        /// </summary>
        /// <param name="obj">The <see cref="HashSet{T}"/> to return.</param>
        /// <returns>The contents of the returned hashset as an array.</returns>
        public T[] ToArrayReturn(HashSet<T> obj)
        {
            T[] array = obj.ToArray();
            Return(obj);
            return array;
        }

        /// <summary>
        /// Gives a pooled <see cref="HashSet{T}"/>.
        /// </summary>
        /// <returns><see cref="HashSet{T}"/> from the pool.</returns>
        public HashSet<T> Rent() => _pool.TryDequeue(out HashSet<T> set) ? set : new();

        /// <summary>
        /// Gives a pooled <see cref="HashSet{T}"/> with provided capacity.
        /// </summary>
        /// <param name="capacity">Requested capacity.</param>
        /// <returns><see cref="HashSet{T}"/> from the pool.</returns>
        public HashSet<T> Rent(int capacity) => _pool.TryDequeue(out HashSet<T> set) ? set : new();

        /// <summary>
        /// Gives a pooled <see cref="HashSet{T}"/> with initial content.
        /// </summary>
        /// <param name="enumerable">Initial content.</param>
        /// <returns><see cref="HashSet{T}"/> from the pool.</returns>
        public HashSet<T> Rent(IEnumerable<T> enumerable)
        {
            if (!_pool.TryDequeue(out HashSet<T> set))
                return new(enumerable);

            if (enumerable is IReadOnlyList<T> list)
            {
                for (int i = 0; i < list.Count; i++)
                    set.Add(list[i]);
            }
            else
            {
                foreach (T t in enumerable)
                    set.Add(t);
            }

            return set;
        }

        /// <summary>
        /// Returns a <see cref="HashSet{T}"/> to the pool.
        /// </summary>
        /// <param name="set">Returned <see cref="HashSet{T}"/>.</param>
        public void Return(HashSet<T> set)
        {
            set.Clear();
            _pool.Enqueue(set);
        }
    }
}
