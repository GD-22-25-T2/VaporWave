namespace UDK.Events.DynamicEvents
{
    using System.Collections.Generic;
    using System;
    using UDK.API.Features.Core.Generic;

    /// <summary>
    /// The class which handles delegates dynamically acting as multicast listener.
    /// </summary>
    public class DynamicEventDispatcher : TypeCastObject<DynamicEventDispatcher>
    {
        private readonly Dictionary<object, List<Action>> _boundDelegates = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEventDispatcher"/> class.
        /// </summary>
        public DynamicEventDispatcher()
        {
        }

        /// <summary>
        /// Gets all the bound delegates.
        /// </summary>
        public IReadOnlyDictionary<object, List<Action>> BoundDelegates => _boundDelegates;

        /// <summary>
        /// Binds a listener to the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        /// <param name="del">The delegate to be bound.</param>
        public virtual void Bind(object obj, Action del)
        {
            if (!_boundDelegates.ContainsKey(obj))
                _boundDelegates.Add(obj, new List<Action>() { del });
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
        public virtual void Invoke(object obj)
        {
            if (_boundDelegates.TryGetValue(obj, out List<Action> delegates))
                delegates.ForEach(del => del());
        }

        /// <summary>
        /// Invokes all the delegates from all the bound delegates.
        /// </summary>
        public virtual void InvokeAll()
        {
            foreach (KeyValuePair<object, List<Action>> kvp in _boundDelegates)
                kvp.Value.ForEach(del => del());
        }


        public virtual void UnbindAll() => _boundDelegates.Clear();
    }
}
