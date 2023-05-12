namespace UDK.API.Features.Input.Models
{
    using System;

    /// <summary>
    /// Represents the input axis.
    /// </summary>
    public sealed class InputAxis : InputBase<float>
    {
        private InputAxis(string name, Action<float> inputDelegate)
            : base(name, inputDelegate) => InputDelegate += delegate (float value) { Value = value; };

        private InputAxis(InputBinding binding, Action<float> inputDelegate)
            : this(binding.Name, inputDelegate)
        {
        }

        /// <summary>
        /// The axis value.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Defines a new <see cref="InputAxis"/>.
        /// </summary>
        /// <param name="name">The name of the axis.</param>
        /// <param name="inputDelegate">The delegate of the action.</param>
        /// <returns>The defined <see cref="InputAxis"/>.</returns>
        public static InputAxis Define(string name, Action<float> inputDelegate) => new(name, inputDelegate);

        /// <summary>
        /// Defines a new <see cref="InputAxis"/>.
        /// </summary>
        /// <param name="binding">The <see cref="InputBinding"/>.</param>
        /// <param name="inputDelegate">The delegate of the action.</param>
        /// <returns>The defined <see cref="InputAxis"/>.</returns>
        public static InputAxis Define(InputBinding binding, Action<float> inputDelegate) => new(binding, inputDelegate);
    }
}
