namespace UDK.API.Features.CharacterMovement
{
    using UDK.API.Features.Core;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The base component which handles footsteps.
    /// <para>
    /// <see cref="GenFootstepComponent"/> provides a basic implementation for footsteps.
    /// <br>The current implementation is intended to be rewritten as needed, as it doesn't really provide any blending solutions nor equalizers.</br>
    /// <br>A new implementation will be eventually added on the next major version (2.0.0).</br>
    /// </para>
    /// <para>
    /// The current implementation is not network replicated.
    /// </para>
    /// <para>
    /// It's not strictly dependent on <see cref="GenMovementComponent"/> as the code base doesn't refer to it.
    /// <br>It's safe to use it along with different movement systems and implementations.</br>
    /// </para>
    /// <para>
    /// The override is recommended, unless footsteps are already handled by a non-CharacterMovement component and there's a possibility to conflict with it,
    /// <br>as the current implementation is a basic one, providing no real solutions for complex cases.</br>
    /// </para>
    /// </summary>
    [AddComponentMenu("UDK/CharacterMovement/GenFootstepComponent")]
    [RequireComponent(typeof(GenMovementComponent))]
    public class GenFootstepComponent : PawnComponent
    {
        #region Editor

        [Space(5)]
        [Header("Volume Settings")]

        [SerializeField]
        protected float minVolume;

        [SerializeField]
        protected float maxVolume;

        [SerializeField]
        protected bool randomizeVolume;

        [Space(5)]
        [Header("Floor Settings")]

        [SerializeField]
        protected string defaultFloor;

        #endregion

        #region BackingFields

        protected readonly Dictionary<string, List<AudioSource>> groundFootsteps = new();

        #endregion

        /// <summary>
        /// Gets a <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing all the audio sources paired to the specified ground.
        /// </summary>
        public IReadOnlyDictionary<string, List<AudioSource>> GroundFootsteps => groundFootsteps;

        /// <summary>
        /// Gets or sets the current floor.
        /// </summary>
        public string CurrentFloor { get; set; }

        /// <summary>
        /// Gets or sets the minimum volume.
        /// </summary>
        public float MinVolume
        {
            get => minVolume;
            set => minVolume = value;
        }

        /// <summary>
        /// Gets or sets the maximum volume.
        /// </summary>
        public float MaxVolume
        {
            get => maxVolume;
            set => maxVolume = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the volume should be randomized.
        /// </summary>
        public bool RandomizeVolume
        {
            get => randomizeVolume;
            set => randomizeVolume = value;
        }

        /// <summary>
        /// Processes the footsteps by evaluating the floor the pawn currently on.
        /// <br>It randomizes the volume whenever the <see cref="AudioSource"/> is played.</br>
        /// </summary>
        protected virtual void ProcessFootsteps()
        {
            foreach (List<AudioSource> value in GroundFootsteps.Values)
                value.ForEach(source => source.Stop());

            if (GroundFootsteps.TryGetValue(CurrentFloor, out List<AudioSource> sources))
            {
                AudioSource source = sources[Random.Range(0, sources.Count)];
                source.PlayOneShot(source.clip, Random.Range(MinVolume, MaxVolume + 1f));
            }
        }

        /// <summary>
        /// Fired when the collider enters the trigger.
        /// </summary>
        /// <param name="other">The entered collider.</param>
        protected void OnTriggerEnter(Collider other)
        {
            foreach (KeyValuePair<string, List<AudioSource>> sources in GroundFootsteps)
            {
                if (other.gameObject.CompareTag(sources.Key))
                {
                    CurrentFloor = sources.Key;
                    break;
                }
            }
        }

        /// <summary>
        /// Fired when the collider exits the trigger.
        /// </summary>
        /// <param name="other">The exited collider.</param>
        protected virtual void OnTriggerExit(Collider other) => CurrentFloor = defaultFloor;
    }
}
