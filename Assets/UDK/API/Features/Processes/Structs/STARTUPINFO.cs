namespace UDK.API.Features.Processes.Structs
{
    using System;

    /// <summary>
    /// A struct containing all startup info.
    /// </summary>
    public struct STARTUPINFO
    {
        /// <summary>
        /// The size of the structure.
        /// </summary>
        public uint cb;

        /// <summary>
        /// Reserved; must be NULL.
        /// </summary>
        public IntPtr lpReserved;

        /// <summary>
        /// The name of the desktop, or the name of both the desktop and window station for this process.
        /// <br>A backslash in the string indicates that the string includes both the desktop and window station names.</br>
        /// </summary>
        public IntPtr lpDesktop;

        /// <summary>
        /// For console processes, this is the title displayed in the title bar if a new console window is created.
        /// <br>If NULL, the name of the executable file is used as the window title instead.</br>
        /// <br>This parameter must be NULL for GUI or console processes that do not create a new console window.</br>
        /// </summary>
        public IntPtr lpTitle;

        /// <summary>
        /// If dwFlags specifies STARTF_USEPOSITION, this member is the x offset of the upper left corner of a window if a new
        /// <br>window is created, in pixels. Otherwise, this member is ignored.</br>
        /// <para>
        /// The offset is from the upper left corner of the screen. For GUI processes, the specified position is used the first time the
        /// <br>new process calls CreateWindow to create an overlapped window if the x parameter of CreateWindow is CW_USEDEFAULT.</br>
        /// </para>
        /// </summary>
        public uint dwX;

        /// <summary>
        /// If dwFlags specifies STARTF_USEPOSITION, this member is the y offset of the upper left corner of a window if a new
        /// <br>window is created, in pixels. Otherwise, this member is ignored.</br>
        /// <para>
        /// The offset is from the upper left corner of the screen. For GUI processes, the specified position is used the first time the
        /// <br>new process calls CreateWindow to create an overlapped window if the y parameter of CreateWindow is CW_USEDEFAULT.</br>
        /// </para>
        /// </summary>
        public uint dwY;

        /// <summary>
        /// If dwFlags specifies STARTF_USESIZE, this member is the height of the window if a new window is created, in pixels. 
        /// <br>Otherwise, this member is ignored.</br>
        /// <para>
        /// For GUI processes, this is used only the first time the new process calls CreateWindow to create an overlapped window if the
        /// <br>nWidth parameter of CreateWindow is CW_USEDEFAULT.</br>
        /// </para>
        /// </summary>
        public uint dwXSize;

        /// <summary>
        /// If dwFlags specifies STARTF_USESIZE, this member is the height of the window if a new window is created, in pixels. 
        /// <br>Otherwise, this member is ignored.</br>
        /// <para>
        /// For GUI processes, this is used only the first time the new process calls CreateWindow to create an overlapped window if the
        /// <br>nHeight parameter of CreateWindow is CW_USEDEFAULT.</br>
        /// </para>
        /// </summary>
        public uint dwYSize;

        /// <summary>
        /// If dwFlags specifies STARTF_USECOUNTCHARS, if a new console window is created in a console process,
        /// <br>this member specifies the screen buffer width, in character columns. Otherwise, this member is ignored.</br>
        /// </summary>
        public uint dwXCountChars;

        /// <summary>
        /// If dwFlags specifies STARTF_USECOUNTCHARS, if a new console window is created in a console process,
        /// <br>this member specifies the screen buffer height, in character rows. Otherwise, this member is ignored.</br>
        /// </summary>
        public uint dwYCountChars;

        /// <summary>
        /// If dwFlags specifies STARTF_USEFILLATTRIBUTE, this member is the initial text and background colors if
        /// <br>a new console window is created in a console application. Otherwise, this member is ignored.</br>
        /// </summary>
        public uint dwFillAttribute;

        /// <summary>
        /// A bitfield that determines whether certain STARTUPINFO members are used when the process creates a window.
        /// <br>This member can be one or more of the following values.</br>
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// If dwFlags specifies STARTF_USESHOWWINDOW, this member can be any of the values that can be specified in the nCmdShow parameter
        /// <br>for the ShowWindow function, except for SW_SHOWDEFAULT. Otherwise, this member is ignored.</br>
        /// </summary>
        public ushort wShowWindow;

        /// <summary>
        /// Reserved for use by the C Run-time; must be zero.
        /// </summary>
        public ushort cbReserved2;

        /// <summary>
        /// Reserved for use by the C Run-time; must be NULL.
        /// </summary>
        public IntPtr lpReserved2;

        /// <summary>
        /// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard input handle for the process.
        /// <br>If STARTF_USESTDHANDLES is not specified, the default for standard input is the keyboard buffer.</br>
        /// </summary>
        public IntPtr hStdInput;

        /// <summary>
        /// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard output handle for the process.
        /// <br>Otherwise, this member is ignored and the default for standard output is the console window's buffer.</br>
        /// </summary>
        public IntPtr hStdOutput;

        /// <summary>
        /// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard error handle for the process.
        /// <br>Otherwise, this member is ignored and the default for standard error is the console window's buffer.</br>
        /// </summary>
        public IntPtr hStdError;
    }
}
