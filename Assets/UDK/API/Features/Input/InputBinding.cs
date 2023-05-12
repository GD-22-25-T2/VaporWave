namespace UDK.API.Features.Input
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an input binding with all their respective properties.
    /// </summary>
    public readonly struct InputBinding
    {
        private static readonly List<InputBinding> _bindings = new();

        private InputBinding(string name, IEnumerable<KeyCode> keys)
        {
            Name = name;
            Keys = keys;
            _bindings.Add(this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="InputBinding"/> containing all the defined bindings.
        /// </summary>
        public static IEnumerable<InputBinding> List => _bindings;

        /// <summary>
        /// Gets the name of the binding.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the keys associated to the binding.
        /// </summary>
        public IEnumerable<KeyCode> Keys { get; }

        /// <summary>
        /// Creates a new <see cref="InputBinding"/>.
        /// </summary>
        /// <param name="name">The name of the binding.</param>
        /// <returns>The new <see cref="InputBinding"/>.</returns>
        public static InputBinding Create(string name)
        {
            InputBinding binding = Get(name);
            return binding.Name == name ? binding : new(name, Enumerable.Empty<KeyCode>());
        }

        /// <summary>
        /// Creates a new <see cref="InputBinding"/>.
        /// </summary>
        /// <param name="name">The name of the binding.</param>
        /// <param name="keys">The keys associated to the binding.</param>
        /// <returns>The new <see cref="InputBinding"/>.</returns>
        public static InputBinding Create(string name, IEnumerable<KeyCode> keys)
        {
            InputBinding binding = Get(name);
            return binding.Name == name ? binding : new(name, keys);
        }

        /// <summary>
        /// Creates a new <see cref="InputBinding"/>.
        /// </summary>
        /// <param name="name">The name of the binding.</param>
        /// <param name="key">The key associated to the binding.</param>
        /// <returns>The new <see cref="InputBinding"/>.</returns>
        public static InputBinding Create(string name, KeyCode key)
        {
            InputBinding binding = Get(name);
            return binding.Name == name ? binding : new(name, new KeyCode[] { key });
        }

        /// <summary>
        /// Gets a <see cref="InputBinding"/> from the specified key name.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The corresponding <see cref="InputBinding"/>.</returns>
        public static InputBinding Get(string name) => _bindings.FirstOrDefault(binding => binding.Name == name);

        /// <summary>
        /// Gets a <see cref="InputBinding"/> from the specified <see cref="KeyCode"/>.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The corresponding <see cref="InputBinding"/>.</returns>
        public static InputBinding Get(KeyCode key) => _bindings.FirstOrDefault(binding => binding.Keys.Contains(key));

        /// <summary>
        /// Tries to get a <see cref="InputBinding"/> given the specified <see cref="KeyCode"/>.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <param name="result">The corresponding <see cref="InputBinding"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="InputBinding"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(KeyCode key, out InputBinding result)
        {
            result = Get(key);
            return _bindings.Any(binding => binding.Keys.Contains(key));
        }

        /// <summary>
        /// Tries to get a <see cref="InputBinding"/> given the specified name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <param name="result">The corresponding <see cref="InputBinding"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="InputBinding"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out InputBinding result)
        {
            result = Get(name);
            return _bindings.Any(binding => binding.Name == name);
        }

        /// <inheritdoc cref="Input.GetKeyDown(KeyCode)"/>
        public static bool GetKeyDown(KeyCode key) => Input.GetKeyDown(key);

        /// <inheritdoc cref="Input.GetKeyUp(KeyCode)"/>
        public static bool GetKeyUp(KeyCode key) => Input.GetKeyUp(key);

        /// <inheritdoc cref="Input.GetKey(KeyCode)"/>
        public static bool GetKey(KeyCode key) => Input.GetKey(key);

        /// <inheritdoc cref="Input.GetKeyDown(string)"/>
        public static bool GetKeyDown(string name) => Input.GetKeyDown(name);

        /// <inheritdoc cref="Input.GetKeyUp(string)"/>
        public static bool GetKeyUp(string name) => Input.GetKeyUp(name);

        /// <inheritdoc cref="Input.GetKey(string)"/>
        public static bool GetKey(string name) => Input.GetKey(name);

        /// <inheritdoc cref="Input.GetAxis(string)"/>
        public static float GetAxis(string name) => Input.GetAxis(name);
    }
}
