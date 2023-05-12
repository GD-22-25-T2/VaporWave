namespace UDK.API.Extensions
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Helper class with methods to easily interact with components.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Gets the <see cref="Component"/> of type <typeparamref name="T"/> in the active <see cref="GameObject"/> or any of its immmediate children.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Component"/> to retrieve.</typeparam>
        /// <param name="component">The parent component.</param>
        /// <returns>A <see cref="Component"/> matching the type <typeparamref name="T"/>, or <see langword="null"/> if not found.</returns>
        public static T GetComponentInImmediateChildren<T>(this Component component) where T : Component
        {
            foreach (Transform child in component.transform)
            {
                T comp = child.GetComponent<T>();
                if (comp != null)
                {
                    return comp;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all components of type <typeparamref name="T"/> in any of its immediate children. 
        /// <br>Unlike <see cref="GameObject.GetComponentsInChildren{T}"/>, this method doesn't return any components in the <see cref="GameObject"/>.</br>
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Component"/> to retrieve.</typeparam>
        /// <param name="component">The parent component.</param>
        /// <returns>An array containing all the found components matching the specified type <typeparamref name="T"/>.</returns>
        public static T[] GetComponentsInImmediateChildren<T>(this Component component) where T : Component
        {
            List<T> comps = new List<T>();
            foreach (Transform child in component.transform)
            {
                T comp = child.GetComponent<T>();
                if (comp != null)
                {
                    comps.Add(comp);
                }
            }
            return comps.ToArray();
        }
    }
}
