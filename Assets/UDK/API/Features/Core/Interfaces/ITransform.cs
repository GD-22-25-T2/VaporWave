namespace UDK.API.Features.Core.Interfaces
{
    using UnityEngine;

    /// <summary>
    /// The base contract for objects handling <see cref="UnityEngine.Transform"/> data.
    /// </summary>
    public interface ITransform
    {
        /// <summary>
        /// The base <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public abstract Transform Transform { get; }

        /// <summary>
        /// The position of the <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// The rotation of the <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public abstract Quaternion Rotation { get; set; }

        /// <summary>
        /// The scale of the <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public abstract Vector3 Scale { get; set; }
    }
}
