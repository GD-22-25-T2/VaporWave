namespace UDK.API.Features.Input.Models
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the key input action.
    /// </summary>
    public class InputAction : InputBase<bool>
    {
        private InputAction(string name, Action<bool> inputDelegate, IEnumerable<KeyCode> keys)
            : base(name, inputDelegate) => Keys = keys;

        private InputAction(InputBinding binding, Action<bool> inputDelegate)
            : this(binding.Name, inputDelegate, binding.Keys)
        {
        }

        /// <summary>
        /// Gets the bound keys.
        /// </summary>
        public IEnumerable<KeyCode> Keys { get; }

        /// <summary>
        /// Defines a new <see cref="InputAction"/>.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="inputDelegate">The delegate of the action.</param>
        /// <param name="keys">The keys to be bound.</param>
        /// <returns>The defined <see cref="InputAction"/>.</returns>
        public static InputAction Define(string name, Action<bool> inputDelegate, IEnumerable<KeyCode> keys) => new(name, inputDelegate, keys);

        /// <summary>
        /// Defines a new <see cref="InputAction"/>.
        /// </summary>
        /// <param name="binding">The <see cref="InputBinding"/>.</param>
        /// <param name="inputDelegate">The delegate of the action.</param>
        /// <returns>The defined <see cref="InputAction"/>.</returns>
        public static InputAction Define(InputBinding binding, Action<bool> inputDelegate) => new(binding, inputDelegate);
    }
}
