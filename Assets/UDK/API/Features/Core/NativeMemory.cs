namespace UDK.API.Features.Core
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Stores a reference to unmanaged memory allocated with <see cref="Marshal.AllocCoTaskMem"/> and prevents it from leaking.
    /// </summary>
    public sealed unsafe class NativeMemory : IDisposable
    {
        /// <summary>
        /// A pointer to allocated memory.
        /// </summary>
        public readonly IntPtr Data;

        /// <summary>
        /// The allocated memory length.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Frees the allocated memory.
        /// </summary>
        ~NativeMemory() => Free();

        /// <summary>
        /// Creates a <see cref="NativeMemory"/> with requested size.
        /// </summary>
        /// <param name="size">The allocation size.</param>
        public NativeMemory(int size)
        {
            Data = Marshal.AllocCoTaskMem(size);
            Length = size;

            if (Length > 0)
                GC.AddMemoryPressure(Length);
        }

        /// <summary>
        /// Converts the <see cref="IntPtr"/> to specified pointer type.
        /// </summary>
        /// <typeparam name="T">The pointer type.</typeparam>
        /// <returns>A pointer to allocated memory.</returns>
        public T* ToPointer<T>()
            where T : unmanaged => (T*)Data.ToPointer();

        /// <summary>
        /// Frees the allocated memory.
        /// </summary>
        public void Dispose()
        {
            Free();
            GC.SuppressFinalize(this);
        }

        private void Free()
        {
            Marshal.FreeCoTaskMem(Data);

            if (Length > 0)
                GC.RemoveMemoryPressure(Length);
        }
    }
}
