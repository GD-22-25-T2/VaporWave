namespace UDK.API.Features.Processes
{
    using System;
    using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

    /// <summary>
    /// All process creation flags.
    /// </summary>
    [Flags]
    public enum ProcessCreationFlags : uint
    {
        /// <summary>
        /// None.
        /// </summary>
        NONE = 0U,

        /// <summary>
        /// The child processes of a process associated with a job are not associated with the job.
        /// <br> If the calling process is not associated with a job, this constant has no effect.</br>
        /// <br>If the calling process is associated with a job, the job must set the JOB_OBJECT_LIMIT_BREAKAWAY_OK limit.</br>
        /// </summary>
        CREATE_BREAKAWAY_FROM_JOB = 16777216U,

        /// <summary>
        /// The new process does not inherit the error mode of the calling process. Instead, the new process gets the default error mode.
        /// <br>This feature is particularly useful for multithreaded shell applications that run with hard errors disabled.</br>
        /// <br>The default behavior is for the new process to inherit the error mode of the caller.Setting this flag changes that default behavior.</br>
        /// </summary>
        CREATE_DEFAULT_ERROR_MODE = 67108864U,

        /// <summary>
        /// The new process has a new console, instead of inheriting its parent's console (the default).
        /// <br>For more information, see Creation of a Console. This flag cannot be used with DETACHED_PROCESS.</br>
        /// </summary>
        CREATE_NEW_CONSOLE = 16U,

        /// <summary>
        /// The new process is the root process of a new process group. The process group includes all processes that are descendants of this root process.
        /// <br>The process identifier of the new process group is the same as the process identifier, which is returned in the lpProcessInformation parameter.</br>
        /// <br>Process groups are used by the GenerateConsoleCtrlEvent function to enable sending a CTRL+BREAK signal to a group of console processes.</br>
        /// <br>If this flag is specified, CTRL+C signals will be disabled for all processes within the new process group.</br>
        /// <br>This flag is ignored if specified with CREATE_NEW_CONSOLE.</br>
        /// </summary>
        CREATE_NEW_PROCESS_GROUP = 512U,

        /// <summary>
        /// The process is a console application that is being run without a console window. Therefore, the console handle for the application is not set.
        /// <br>This flag is ignored if the application is not a console application, or if it is used with either CREATE_NEW_CONSOLE or DETACHED_PROCESS.</br>
        /// </summary>
        CREATE_NO_WINDOW = 134217728U,

        /// <summary>
        /// The process is to be run as a protected process. The system restricts access to protected processes and the threads of protected processes.
        /// <br>For more information on how processes can interact with protected processes, see Process Security and Access Rights.</br>
        /// <br>To activate a protected process, the binary must have a special signature. This signature is provided by Microsoft but not currently available for non-Microsoft binaries.</br>
        /// <br>There are currently four protected processes: media foundation, audio engine, Windows error reporting, and system.</br>
        /// <br>Components that load into these binaries must also be signed.</br>
        /// <br>Multimedia companies can leverage the first two protected processes.</br>
        /// <br>For more information, see Overview of the Protected Media Path.</br>
        /// <br>Windows Server 2003 and Windows XP: This value is not supported.</br>
        /// </summary>
        CREATE_PROTECTED_PROCESS = 262144U,

        /// <summary>
        /// Allows the caller to execute a child process that bypasses the process restrictions that would normally be applied automatically to the process.
        /// </summary>
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 33554432U,

        /// <summary>
        /// This flag allows secure processes, that run in the Virtualization-Based Security environment, to launch.
        /// </summary>
        CREATE_SECURE_PROCESS = 4194304U,

        /// <summary>
        /// This flag is valid only when starting a 16-bit Windows-based application. If set, the new process runs in a private Virtual DOS Machine (VDM).
        /// <br>By default, all 16-bit Windows-based applications run as threads in a single, shared VDM.</br>
        /// <br>The advantage of running separately is that a crash only terminates the single VDM; any other programs running in distinct VDMs continue to function normally.</br>
        /// <br>Also, 16-bit Windows-based applications that are run in separate VDMs have separate input queues.</br>
        /// <br>That means that if one application stops responding momentarily, applications in separate VDMs continue to receive input.</br>
        /// <br>The disadvantage of running separately is that it takes significantly more memory to do so.</br>
        /// <br>You should use this flag only if the user requests that 16-bit applications should run in their own VDM.</br>
        /// </summary>
        CREATE_SEPARATE_WOW_VDM = 2048U,

        /// <summary>
        /// The flag is valid only when starting a 16-bit Windows-based application.
        /// <br>If the DefaultSeparateVDM switch in the Windows section of WIN.INI is TRUE, this flag overrides the switch.</br>
        /// <br>The new process is run in the shared Virtual DOS Machine.</br>
        /// </summary>
        CREATE_SHARED_WOW_VDM = 4096U,

        /// <summary>
        /// The primary thread of the new process is created in a suspended state, and does not run until the ResumeThread function is called.
        /// </summary>
        CREATE_SUSPENDED = 4U,

        /// <summary>
        /// If this flag is set, the environment block pointed to by lpEnvironment uses Unicode characters. Otherwise, the environment block uses ANSI characters.
        /// </summary>
        CREATE_UNICODE_ENVIRONMENT = 1024U,

        /// <summary>
        /// The calling thread starts and debugs the new process. It can receive all related debug events using the WaitForDebugEvent function.
        /// </summary>
        DEBUG_ONLY_THIS_PROCESS = 2U,

        /// <summary>
        /// The calling thread starts and debugs the new process and all child processes created by the new process.
        /// <br>It can receive all related debug events using the WaitForDebugEvent function.</br>
        /// <br>A process that uses DEBUG_PROCESS becomes the root of a debugging chain. </br>
        /// <br>This continues until another process in the chain is created with DEBUG_PROCESS.</br>
        /// <br>If this flag is combined with DEBUG_ONLY_THIS_PROCESS, the caller debugs only the new process, not any child processes.</br>
        /// </summary>
        DEBUG_PROCESS = 1U,

        /// <summary>
        /// For console processes, the new process does not inherit its parent's console (the default).
        /// <br>The new process can call the AllocConsole function at a later time to create a console.</br>
        /// <br>For more information, see Creation of a Console. This value cannot be used with CREATE_NEW_CONSOLE.</br>
        /// </summary>
        DETACHED_PROCESS = 8U,

        /// <summary>
        /// The process is created with extended startup information; the lpStartupInfo parameter specifies a STARTUPINFOEX structure.
        /// <br>Windows Server 2003 and Windows XP: This value is not supported.</br>
        /// </summary>
        EXTENDED_STARTUPINFO_PRESENT = 524288U,

        /// <summary>
        /// The process inherits its parent's affinity.
        /// <br>If the parent process has threads in more than one processor group, the new process inherits the group-relative affinity of an arbitrary group in use by the parent.</br>
        /// <br>Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This value is not supported.</br>
        /// </summary>
        INHERIT_PARENT_AFFINITY = 65536U
    }
}
