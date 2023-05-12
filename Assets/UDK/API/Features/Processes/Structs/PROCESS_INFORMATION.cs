namespace UDK.API.Features.Processes.Structs
{
    using System;

    /// <summary>
    /// A struct containing all process information.
    /// </summary>
    public struct PROCESS_INFORMATION
    {
        /// <summary>
        /// The process handle.
        /// </summary>
        public IntPtr hProcess;

        /// <summary>
        /// The thread handle.
        /// </summary>
        public IntPtr hThread;

        /// <summary>
        /// The process id.
        /// </summary>
        public uint dwProcessId;

        /// <summary>
        /// The thread id.
        /// </summary>
        public uint dwThreadId;
    }
}
