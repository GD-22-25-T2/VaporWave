namespace UDK.API.Features.Core
{
    using UDK.API.Features.Core.Interfaces;
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// A class that provides object type safety.
    /// <para>
    /// This class is intended to be used to prevent an object's type from being used in certain cases.
    /// <br>A <see cref="TSubclassOf{T}"/> handles the object's type in a way it'll always be a derived type from the selected type.</br>
    /// </para>
    /// </summary>
    [AddComponentMenu("UDK/Core/NavigationObjectBase")]
    [DisallowMultipleComponent]
    public abstract class NavigationObjectBase : Actor, INavAgent
    {
        protected static readonly List<NavigationObjectBase> navigationObjects = new();

        /// <summary>
        /// Gets all <see cref="NavigationObjectBase"/> objects.
        /// </summary>
        public static IEnumerable<NavigationObjectBase> List => navigationObjects;

        /// <summary>
        /// Gets or sets the capsule component.
        /// </summary>
        public CapsuleCollider CapsuleComponent { get; protected set; }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            navigationObjects.Add(this);

            CapsuleComponent = GetComponent<CapsuleCollider>() ?? AddComponent<CapsuleCollider>();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            navigationObjects.Remove(this);
        }
    }
}
