namespace UDK.API.Features.StatusEffects
{
    using System;
    using System.Collections.Generic;
    using UDK.API.Features.Core;
    using UnityEngine;

    /// <summary>
    /// The effects controller which handles in-game effects.
    /// </summary>
    [AddComponentMenu("UDK/StatusEffects/EffectsController")]
    [DisallowMultipleComponent]
    public class EffectsController : PawnComponent
    {
        #region Editor

        [SerializeField]
        private GameObject loadedEffects;

        #endregion

        #region BackingFields

        private readonly Dictionary<Type, StatusEffectBase> _effectsByType = new();

        #endregion

        /// <summary>
        /// Gets all the paired types to their relative effect.
        /// </summary>
        public IReadOnlyDictionary<Type, StatusEffectBase> EffectsByType => _effectsByType;

        /// <summary>
        /// Gets or sets all the available effects.
        /// </summary>
        public StatusEffectBase[] AllEffects { get; protected set; }

        /// <summary>
        /// Gets or sets the effects length.
        /// </summary>
        public int EffectsLength { get; protected set; }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            AllEffects = loadedEffects.GetComponentsInChildren<StatusEffectBase>();
            EffectsLength = AllEffects.Length;

            foreach (StatusEffectBase statusEffectBase in AllEffects)
                _effectsByType.Add(statusEffectBase.GetType(), statusEffectBase);
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            loadedEffects.SetActive(true);
        }

        /// <summary>
        /// Tries to get an effect given the specified name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <param name="playerEffect">The found <see cref="StatusEffectBase"/>.</param>
        /// <returns><see langword="true"/> if the effect was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetEffect(string name, out StatusEffectBase playerEffect)
        {
            foreach (StatusEffectBase statusEffectBase in AllEffects)
            {
                if (statusEffectBase.ToString().StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    playerEffect = statusEffectBase;
                    return true;
                }
            }

            playerEffect = null;
            return false;
        }

        /// <summary>
        /// Tries to get an effect given the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="playerEffect">The found <see cref="StatusEffectBase"/>.</param>
        /// <returns><see langword="true"/> if the effect was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetEffect<T>(out T playerEffect)
            where T : StatusEffectBase
        {
            playerEffect = default(T);

            StatusEffectBase statusEffectBase;
            if (!_effectsByType.TryGetValue(typeof(T), out statusEffectBase))
                return false;

            T effect = statusEffectBase.Cast<T>();
            if (effect == null)
                return false;

            playerEffect = effect;
            return true;
        }

        /// <summary>
        /// Changes the state of the specified effect.
        /// </summary>
        /// <param name="name">The name of the effect to look for.</param>
        /// <param name="intensity">The new intensity.</param>
        /// <param name="duration">The new duration.</param>
        /// <param name="addDuration">Whether duration should be added.</param>
        /// <returns>The modified <see cref="StatusEffectBase"/>, or <see langword="null"/> if not found.</returns>
        public StatusEffectBase ChangeState(string name, byte intensity, float duration = 0f, bool addDuration = false)
        {
            if (TryGetEffect(name, out StatusEffectBase statusEffectBase))
                statusEffectBase.SetState(intensity, duration, addDuration);

            return statusEffectBase;
        }

        /// <summary>
        /// Changes the state of the specified effect.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="intensity">The new intensity.</param>
        /// <param name="duration">The new duration.</param>
        /// <param name="addDuration">Whether duration should be added.</param>
        /// <returns>The modified <see cref="StatusEffectBase"/>, or <see langword="null"/> if not found.</returns>
        public T ChangeState<T>(byte intensity, float duration = 0f, bool addDuration = false)
            where T : StatusEffectBase
        {
            if (TryGetEffect(out T effect))
                effect.SetState(intensity, duration, addDuration);

            return effect;
        }

        /// <summary>
        /// Enables the specified effect.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="duration">The new duration.</param>
        /// <param name="addDuration">Whether duration should be added.</param>
        /// <returns>The enabled <see cref="StatusEffectBase"/>, or <see langword="null"/> if not found.</returns>
        public T EnableEffect<T>(float duration = 0f, bool addDuration = false)
            where T : StatusEffectBase => ChangeState<T>(1, duration, addDuration);

        /// <summary>
        /// Disables the specified effect.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <returns>The enabled <see cref="StatusEffectBase"/>, or <see langword="null"/> if not found.</returns>
        public T DisableEffect<T>()
            where T : StatusEffectBase => ChangeState<T>(0, 0f, false);

        /// <summary>
        /// Disables all effects.
        /// </summary>
        public void DisableAllEffects()
        {
            StatusEffectBase[] allEffects = AllEffects;
            for (int i = 0; i < allEffects.Length; i++)
                AllEffects[i].DisableEffect();
        }

        /// <summary>
        /// Gets an effect given the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <returns>The found <see cref="StatusEffectBase"/>, or <see langword="null"/> if not found.</returns>
        public T GetEffect<T>()
            where T : StatusEffectBase => TryGetEffect(out T effect) ? effect : default(T);
    }
}
