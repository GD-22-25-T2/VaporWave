namespace UDK.API.Features.Processes
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using Processes.Structs;

    /// <summary>
    /// A helper class which handles external processes.
    /// </summary>
    public static class ExternalProcess
    {
        /// <summary>
        /// Creates a process.
        /// </summary>
        /// <param name="lpApplicationName">The application name.</param>
        /// <param name="lpCommandLine">The command line arg.</param>
        /// <param name="procSecAttrs">The security attributes.</param>
        /// <param name="threadSecAttrs">The threa security attributes.</param>
        /// <param name="bInheritHandles">Whether handles should be inherited.</param>
        /// <param name="dwCreationFlags">The process creation flags.</param>
        /// <param name="lpEnvironment">The running environment.</param>
        /// <param name="lpCurrentDirectory">The current directory.</param>
        /// <param name="lpStartupInfo">The startup info.</param>
        /// <param name="lpProcessInformation">The process information.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcessW(
            string lpApplicationName, [In] string lpCommandLine, IntPtr procSecAttrs,
            IntPtr threadSecAttrs, bool bInheritHandles, ProcessCreationFlags dwCreationFlags,
            IntPtr lpEnvironment, string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo, ref PROCESS_INFORMATION lpProcessInformation);

        /// <summary>
        /// Starts the specified process.
        /// </summary>
        /// <param name="path">The path of the process.</param>
        /// <param name="dir">The directory of the process.</param>
        /// <param name="hidden">Whether the process should be hidden.</param>
        /// <returns>The process id.</returns>
        /// <exception cref="Win32Exception"/>
        public static uint Start(string path, string dir, bool hidden = false)
        {
            ProcessCreationFlags dwCreationFlags = hidden ? ProcessCreationFlags.CREATE_NO_WINDOW : ProcessCreationFlags.NONE;
            STARTUPINFO startupinfo = new STARTUPINFO { cb = (uint)Marshal.SizeOf<STARTUPINFO>() };

            PROCESS_INFORMATION process_INFORMATION = default(PROCESS_INFORMATION);
            if (!CreateProcessW(null, path, IntPtr.Zero, IntPtr.Zero, false, dwCreationFlags, IntPtr.Zero, dir, ref startupinfo, ref process_INFORMATION))
                throw new Win32Exception();

            return process_INFORMATION.dwProcessId;
        }
    }
}
