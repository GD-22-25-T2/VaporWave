namespace UDK.API.Events
{
    using System;
    using static UDK.API.Events.EventManager;

    /// <summary>
    /// A set of tools to execute events safely and without breaking other plugins.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Executes all <see cref="FDelegate{TEventArgs}"/> listeners safely.
        /// </summary>
        /// <typeparam name="T">Event arg type.</typeparam>
        /// <param name="ev">Source event.</param>
        /// <param name="arg">Event arg.</param>
        /// <exception cref="ArgumentNullException">Event or its arg is <see langword="null"/>.</exception>
        public static void InvokeSafely<T>(FDelegate<T> ev, T arg)
            where T : System.EventArgs => EventManager.InvokeSafely<T>(ev, arg);

        /// <summary>
        /// Executes all <see cref="FDelegate"/> listeners safely.
        /// </summary>
        /// <param name="ev">Source event.</param>
        /// <exception cref="ArgumentNullException">Event is <see langword="null"/>.</exception>
        public static void InvokeSafely(FDelegate ev) => EventManager.InvokeSafely(ev);
    }
}
