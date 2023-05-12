namespace UDK.Events.DynamicEvents
{
    using System.Collections.Generic;
    using System;
    using UDK.API.Features.Core.Generic;

    /// <summary>
    /// The <see cref="DynamicEventDispatcher"/>'s generic version which accepts a type parameter.
    /// </summary>
    public class TDynamicEventDispatcher<T> : TypeCastObject<DynamicEventDispatcher>
    {
        private readonly Dictionary<object, List<Action<T>>> _boundDelegates = new();

        public TDynamicEventDispatcher()
        {
        }

        /// <summary>
        /// Gets all the bound delegates.
        /// </summary>
        public IReadOnlyDictionary<object, List<Action<T>>> BoundDelegates => _boundDelegates;

        /// <summary>
        /// Binds a listener to the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        /// <param name="del">The delegate to be bound.</param>
        public virtual void Bind(object obj, Action<T> del)
        {
            if (!_boundDelegates.ContainsKey(obj))
                _boundDelegates.Add(obj, new List<Action<T>>() { del });
            else
                _boundDelegates[obj].Add(del);  
        }

        /// <summary>
        /// Unbinds a listener from the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        public virtual void Unbind(object obj) => _boundDelegates.Remove(obj);

        /// <summary>
        /// Invokes the delegates from the specified listener.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        public virtual void Invoke(object obj, T instance)
        {
            if (_boundDelegates.TryGetValue(obj, out List<Action<T>> delegates))
                delegates.ForEach(del => del(instance));
        }

        /// <summary>
        /// Invokes all the delegates from all the bound delegates.
        /// </summary>
        public virtual void InvokeAll(T instance)
        {
            foreach (KeyValuePair<object, List<Action<T>>> kvp in _boundDelegates)
                kvp.Value.ForEach(del => del(instance));
        }

        /// <inheritdoc/>
        public virtual void UnbindAll() => _boundDelegates.Clear();
    }
}
