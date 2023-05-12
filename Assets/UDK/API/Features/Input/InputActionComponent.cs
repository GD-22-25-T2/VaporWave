namespace UDK.API.Features.Input
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using UDK.API.Features.Core;
    using UDK.API.Features.Input.Models;

    /// <summary>
    /// The base component which handles inputs.
    /// </summary>
    [AddComponentMenu("UDK/Input/InputActionComponent")]
    [DisallowMultipleComponent]
    public class InputActionComponent : PawnComponent
    {
        private readonly HashSet<InputAxis> _boundAxis = new();
        private readonly HashSet<InputAction> _boundActions = new();

        /// <summary>
        /// Gets all the bound input actions.
        /// </summary>
        public IEnumerable<InputAction> BoundActions => _boundActions;

        /// <summary>
        /// Gets all the bound input axis.
        /// </summary>
        public IEnumerable<InputAxis> BoundAxis => _boundAxis;

        /// <summary>
        /// Gets or sets a value indicating whether keys should be detected.
        /// </summary>
        public bool DetectByKeys { get; set; } = true;

        /// <inheritdoc/>
        protected override void Tick(float deltaTime)
        {
            foreach (InputAxis inputAxis in _boundAxis)
                inputAxis.InputDelegate(InputBinding.GetAxis(inputAxis.Name));

            if (DetectByKeys)
            {
                foreach (InputAction inputAction in _boundActions)
                    inputAction.InputDelegate(inputAction.Keys.Any(key => InputBinding.GetKey(key) || InputBinding.GetKeyDown(key)));

                return;
            }

            foreach (InputAction inputAction in _boundActions)
                inputAction.InputDelegate(InputBinding.GetKey(inputAction.Name) || InputBinding.GetKeyDown(inputAction.Name));
        }

        /// <summary>
        /// Binds an input action.
        /// </summary>
        public void BindInputAction(InputAction inputAction) => _boundActions.Add(inputAction);

        /// <inheritdoc cref="BindInputAction(InputAction)"/>
        public void BindInputAction(string name, Action<bool> inputDelegate, IEnumerable<KeyCode> keys)
        {
            if (InputBinding.TryGet(name, out InputBinding binding))
            {
                BindInputAction(InputAction.Define(binding, inputDelegate));
                return;
            }

            BindInputAction(InputAction.Define(InputBinding.Create(name, keys), inputDelegate));
        }

        /// <inheritdoc cref="BindInputAction(InputAction)"/>
        public void BindInputAction(InputBinding binding, Action<bool> inputDelegate) => BindInputAction(InputAction.Define(binding, inputDelegate));

        /// <summary>
        /// Binds an input axis.
        /// </summary>
        public void BindInputAxis(InputAxis inputAxis) => _boundAxis.Add(inputAxis);

        /// <inheritdoc cref="BindInputAxis(InputAxis)"/>
        public void BindInputAxis(string name, Action<float> inputDelegate) =>
            BindInputAxis(InputAxis.Define(name, inputDelegate));

        /// <inheritdoc cref="BindInputAxis(InputAxis)"/>
        public void BindInputAxis(InputBinding binding, Action<float> inputDelegate) =>
            BindInputAxis(InputAxis.Define(binding, inputDelegate));
    }
}
