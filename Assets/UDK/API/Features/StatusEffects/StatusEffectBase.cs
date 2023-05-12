namespace UDK.API.Features.StatusEffects
{
    using System;
    using UDK.API.Features.Core;
    using UnityEngine;

    /// <summary>
    /// The base status effect component.
    /// </summary>
    [AddComponentMenu("UDK/StatusEffects/StatusEffectBase")]
    public abstract class StatusEffectBase : PawnComponent, IEquatable<StatusEffectBase>
    {
        /// <summary>
        /// All the effect classifications.
        /// </summary>
        public enum EffectClassification
        {
            /// <summary>
            /// Means it's negative.
            /// </summary>
            Negative,

            /// <summary>
            /// Means it's mixed.
            /// </summary>
            Mixed,

            /// <summary>
            /// Means it's positive.
            /// </summary>
            Positive
        }

        /// <summary>
        /// Invoked when the effect is enabled.
        /// </summary>
        public static event Action<StatusEffectBase> OnEnabled;

        /// <summary>
        /// Invoked when the effect is disabled.
        /// </summary>
        public static event Action<StatusEffectBase> OnDisabled;

        /// <summary>
        /// Invoked when the effect's intensity changes.
        /// </summary>
        public static event Action<StatusEffectBase, byte, byte> OnIntensityChanged;

        #region BackingFields

        protected byte intensity;
        protected float duration;
        protected float timeLeft;

        #endregion

        /// <summary>
        /// Gets or sets the effect's intensity.
        /// </summary>
        public byte Intensity
        {
            get => intensity;
            set
            {
                if (intensity == value || !AllowEnabling && value > intensity)
                    return;

                byte prevIntensity = intensity;
                bool flag = prevIntensity == 0 && value > 0;
                intensity = (byte)Mathf.Min(value, MaxIntensity);

                if (flag)
                {
                    Enabled();

                    if (OnEnabled is not null)
                        OnEnabled(this);
                }
                else if (prevIntensity > 0 && value == 0)
                {
                    Disabled();

                    if (OnDisabled is not null)
                        OnDisabled(this);
                }

                IntensityChanged(prevIntensity, value);

                if (OnIntensityChanged is not null)
                    OnIntensityChanged(this, prevIntensity, value);
            }
        }

        /// <summary>
        /// Gets the effect's max intensity.
        /// </summary>
        public virtual byte MaxIntensity { get; } = 255;

        /// <summary>
        /// Gets or sets a value indicating whether the effect is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => Intensity > 0;
            set
            {
                if (value == IsEnabled)
                    return;

                Intensity = (byte)(value ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the effect can be enabled.
        /// </summary>
        public virtual bool AllowEnabling { get; } = true;

        /// <summary>
        /// Gets the <see cref="EffectClassification"/>.
        /// </summary>
        public virtual EffectClassification Classification { get; }

        /// <summary>
        /// Gets or sets the effect's duration.
        /// </summary>
        public float Duration
        {
            get => duration;
            set => duration = Mathf.Max(0f, value);
        }

        /// <summary>
        /// Gets or sets the effect's time left.
        /// </summary>
        public float TimeLeft
        {
            get => timeLeft;
            set
            {
                timeLeft = Mathf.Max(0f, value);
                if (timeLeft == 0f && Duration != 0f)
                    DisableEffect();
            }
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Intensity = 1;
            DisableEffect();
        }

        /// <inheritdoc/>
        protected override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (!IsEnabled)
                return;

            RefreshTime(deltaTime);
        }

        /// <summary>
        /// Sets the effect's state.
        /// </summary>
        /// <param name="intensity">The intensity.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="addDuration">Whether duration should be added to the current one.</param>
        public void SetState(byte intensity, float duration = 0f, bool addDuration = false)
        {
            Intensity = intensity;
            ChangeDuration(duration, addDuration);
        }

        /// <summary>
        /// Changes the effect's duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="addDuration">Whether duration should be added to the current one.</param>
        public void ChangeDuration(float duration, bool addDuration = false)
        {
            if (addDuration && duration > 0f)
            {
                Duration += duration;
                TimeLeft += duration;
                return;
            }

            Duration = duration;
            TimeLeft = duration;
        }

        /// <summary>
        /// Refreshes the effect's time.
        /// </summary>
        /// <param name="deltaTime"></param>
        protected void RefreshTime(float deltaTime)
        {
            if (Duration == 0f)
                return;

            TimeLeft -= deltaTime;
        }

        /// <summary>
        /// Fired when the effect is enabled.
        /// </summary>
        protected virtual void Enabled()
        {
        }

        /// <summary>
        /// Fired when the effect is disabled.
        /// </summary>
        protected virtual void Disabled()
        {
        }

        /// <summary>
        /// Fired every tick.
        /// </summary>
        protected virtual void OnEffectUpdate()
        {
        }

        /// <summary>
        /// Fired when the intensity is changed.
        /// </summary>
        /// <param name="prevState">The previous intensity.</param>
        /// <param name="newState">The new intensity.</param>
        protected virtual void IntensityChanged(byte prevState, byte newState)
        {
        }

        /// <summary>
        /// Disables the effect.
        /// </summary>
        public virtual void DisableEffect() => Intensity = 0;

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="StatusEffectBase"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="StatusEffectBase"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the <see cref="StatusEffectBase"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public bool Equals(StatusEffectBase other) => other && other.GameObject == GameObject;

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="object"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="object"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the <see cref="object"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj != null && (this as object == obj || (obj.GetType() == GetType() && Equals((StatusEffectBase)obj)));

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer representing the hash code for this instance.</returns>
        public override int GetHashCode() => GameObject.GetHashCode();
    }
}
