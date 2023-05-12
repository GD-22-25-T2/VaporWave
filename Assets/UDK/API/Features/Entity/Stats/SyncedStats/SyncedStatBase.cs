namespace UDK.API.Features.Entity.Stats.SyncedStats
{
    using UDK.API.Features.Core.Framework;
    using UnityEngine;

    /// <summary>
    /// The base implementation of stat modules.
    /// <para>
    /// A stat module is a component which handles everything regarding in-game stats.
    /// </para>
    /// <br>It can be used to implement many important modules, from the basic ones to the most advanced ones.</br>
    /// <br>It's strictly dependent on the <see cref="Entity"/> as it supports entities derived from the <see cref="EntityBase"/> class.</br>
    /// <para>
    /// The current implementation is not network replicated.
    /// </para>
    /// </summary>
    public abstract class SyncedStatBase
    {
        protected float lastValue;

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public virtual float CurrentValue
        {
            get => lastValue;
            set
            {
                float previousLastValue = lastValue;
                lastValue = value;

                if (previousLastValue != value)
                    OnValueChanged(previousLastValue, value);
            }
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public abstract float MinValue { get; }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public abstract float MaxValue { get; }

        /// <summary>
        /// Gets or sets the custom maximum value.
        /// </summary>
        public virtual float CustomMaxValue { get; set; }

        /// <summary>
        /// Gets the normalized value.
        /// </summary>
        public float NormalizedValue
        {
            get
            {
                float maxValue = Mathf.Max(MaxValue, CustomMaxValue);
                return MinValue != maxValue ? (CurrentValue - MinValue) / (maxValue - MinValue) : 0f;
            }
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        public Pawn Owner { get; set; }

        /// <summary>
        /// Fired when the value is changed.
        /// </summary>
        /// <param name="prevValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnValueChanged(float prevValue, float newValue)
        {
        }

        /// <summary>
        /// Initializes the stat on the given entity.
        /// </summary>
        /// <param name="pawn">The entity to initialize the stat on.</param>
        public virtual void Init(Pawn pawn) => Owner = pawn;

        /// <summary>
        /// Updates the <see cref="SyncedStatBase"/>.
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// Restores <see cref="CurrentValue"/> to the given processed value without exceeding the <see cref="MaxValue"/>.
        /// </summary>
        /// <param name="value">The value to be processed.</param>
        public virtual void Restore(float value) => CurrentValue = Mathf.Min(CurrentValue + Mathf.Abs(value), Mathf.Max(MaxValue, CustomMaxValue));
    }
}
