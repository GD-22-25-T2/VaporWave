namespace UDK.Events.DynamicEvents
{
    public interface IDynamicEventDispatcher
    {
        /// <summary>
        /// Unbinds all the delegates from all the bound delegates.
        /// </summary>
        void UnbindAll();
    }
}
