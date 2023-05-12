namespace UDK.API.Features
{
    using System;
    using System.Reflection;
    using UnityEngine;

    /// <summary>
    /// A set of tools to print messages on the console.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Sends a <see cref="LogType.Log"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogType.Log);

        /// <summary>
        /// Sends a <see cref="LogType.Log"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogType.Log);

        /// <summary>
        /// Sends a <see cref="LogType.Log"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Debug(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] [DEBUG] {message}", LogType.Log);

        /// <summary>
        /// Sends a <see cref="LogType.Log"/> level messages to the game console.
        /// </summary>
        /// <typeparam name="T">The inputted object's type.</typeparam>
        /// <param name="object">The object to be logged and returned.</param>
        /// <returns>The <typeparamref name="T"/> object inputted in <paramref name="object"/>.</returns>
        public static T DebugObject<T>(T @object)
        {
            Debug(@object);
            return @object;
        }

        /// <summary>
        /// Sends a <see cref="LogType.Warning"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogType.Warning);

        /// <summary>
        /// Sends a <see cref="LogType.Warning"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogType.Warning);

        /// <summary>
        /// Sends a <see cref="LogType.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogType.Error);

        /// <summary>
        /// Sends a <see cref="LogType.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogType.Error);

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        public static void Send(object message, LogType level) => SendRaw($"[{level.ToString().ToUpper()}] {message}");

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        public static void Send(string message, LogType level) => SendRaw($"[{level.ToString().ToUpper()}] {message}");

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void SendRaw(object message) => UnityEngine.Debug.Log(message);

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void SendRaw(string message) => UnityEngine.Debug.Log(message);

        /// <summary>
        /// Sends a <see cref="Error(object)"/> with the provided message if the condition is false and stops the code execution.
        /// </summary>
        /// <param name="condition">The conditional expression to evaluate. If the condition is true it will continue.</param>
        /// <param name="message">The information message. The error and exception will show this message.</param>
        /// <exception cref="Exception">If the condition is false. It throws an exception stopping the execution.</exception>
        public static void Assert(bool condition, object message)
        {
            if (condition)
                return;

            Error(message);

            Exception ex = new(message.ToString());
            UnityEngine.Debug.LogException(ex);

            throw ex;
        }

        /// <summary>
        /// Clears the console.
        /// </summary>
        public static void Clear() => UnityEngine.Debug.ClearDeveloperConsole();
    }
}
