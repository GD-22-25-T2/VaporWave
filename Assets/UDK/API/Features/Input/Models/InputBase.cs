namespace UDK.API.Features.Input.Models
{
    using System;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

    /// <summary>
    /// Represents a basic user input.
    /// </summary>
    /// <typeparam name="T">The <see cref="ValueType"/> to handle.</typeparam>
    public abstract class InputBase<T>
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputBase{T}"/> class. 
        /// </summary>
        /// <param name="name">The input name.</param>
        /// <param name="inputDelegate">The input delegate.</param>
        protected InputBase(string name, Action<T> inputDelegate)
        {
            Name = name;
            InputDelegate = inputDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBase{T}"/> class.
        /// </summary>
        /// <param name="binding">The input binding.</param>
        /// <param name="inputDelegate">The input delegate.</param>
        protected InputBase(InputBinding binding, Action<T> inputDelegate)
            : this(binding.Name, inputDelegate)
        {
        }

        /// <summary>
        /// Gets the input game.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Gets or sets the input delegate.
        /// </summary>
        public virtual Action<T> InputDelegate { get; set; }

        /// <summary>
        /// Implicitly converts a <see cref="InputBase{T}"/> to a <see cref="bool"/>.
        /// </summary>
        /// <param name="inputAxis">The <see cref="InputBase{T}"/> to convert.</param>

        public static implicit operator bool(InputBase<T> inputAxis) => inputAxis is not null;

        /// <summary>
        /// Implicitly converts a <see cref="InputBase{T}"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="inputAxis">The <see cref="InputBase{T}"/> to convert.</param>

        public static implicit operator string(InputBase<T> inputAxis) => inputAxis.Name;

        /// <summary>
        /// Compares two operands: <see cref="InputBase{T}"/> and <see cref="InputBase{T}"/>.
        /// </summary>
        /// <param name="left">The right-hand <see cref="InputBase{T}"/> to compare.</param>
        /// <param name="right">The left-hand <see cref="InputBase{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(InputBase<T> left, InputBase<T> right) => Equals(left, right);

        /// <summary>
        /// Compares two operands: <see cref="InputBase{T}"/> and <see cref="InputBase{T}"/>.
        /// </summary>
        /// <param name="left">The right-hand <see cref="InputBase{T}"/> to compare.</param>
        /// <param name="right">The left-hand <see cref="InputBase{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(InputBase<T> left, InputBase<T> right) => !Equals(left, right);

        /// <summary>
        /// Compares two operands: <see cref="InputBase{T}"/> and <see cref="string"/>.
        /// </summary>
        /// <param name="left">The right <see cref="InputBase{T}"/> to compare.</param>
        /// <param name="right">The left <see cref="string"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(InputBase<T> left, string right) => left && left.Name == right;

        /// <summary>
        /// Compares two operands: <see cref="InputBase{T}"/> and <see cref="string"/>.
        /// </summary>
        /// <param name="left">The right <see cref="InputBase{T}"/> to compare.</param>
        /// <param name="right">The left <see cref="string"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(InputBase<T> left, string right) => left || left.Name != right;

        /// <summary>
        /// Compares two operands: <see cref="InputBase{T}"/> and <see cref="string"/>.
        /// </summary>
        /// <param name="left">The left <see cref="string"/> to compare.</param>
        /// <param name="right">The right <see cref="InputBase{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(string left, InputBase<T> right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="InputBase{T}"/> and <see cref="string"/>.
        /// </summary>
        /// <param name="left">The left <see cref="string"/> to compare.</param>
        /// <param name="right">The right <see cref="InputBase{T}"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(string left, InputBase<T> right) => right != left;
    }
}
