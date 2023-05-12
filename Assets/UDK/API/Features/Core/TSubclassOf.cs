namespace UDK.API.Features.Core
{
    using System;
    using UnityEngine;
    using UDK.API.Features.Core.Generic;

    /// <summary>
    /// A class that provides object type safety.
    /// <para>
    /// This class is intended to be used to prevent an object's type from being used in certain cases.
    /// <br>A <see cref="TSubclassOf{T}"/> handles the object's type in a way it'll always be a derived type from the selected type.</br>
    /// </para>
    /// </summary>
    /// <typeparam name="T">The subclass type.</typeparam>
    [Serializable]
    public class TSubclassOf<T> : TypeCastObject<TSubclassOf<T>>
    {
        [SerializeField]
        [HideInInspector]
        protected internal string parentTypeQualifiedName;

        [SerializeField]
        [HideInInspector]
        protected internal string selectedTypeQualifiedName = "Null";

        [SerializeField]
        protected internal string subclassType = "None";

        /// <summary>
        /// Initializes a new instance of the <see cref="TSubclassOf{T}"/> class.
        /// </summary>
        public TSubclassOf()
        {
            parentTypeQualifiedName = typeof(T).AssemblyQualifiedName;
            selectedTypeQualifiedName = "Null";
            subclassType = Type != null ? Type.ToString() : "None";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TSubclassOf{T}"/> class.
        /// </summary>
        /// <param name="selectedType">The explicit selected type.</param>
        public TSubclassOf(Type selectedType)
        {
            parentTypeQualifiedName = typeof(T).AssemblyQualifiedName;

            if (selectedType == null)
            {
                selectedTypeQualifiedName = "Null";
                subclassType = "None";
                return;
            }

            if (selectedType.IsSubclassOf(typeof(T)))
            {
                selectedTypeQualifiedName = selectedType.AssemblyQualifiedName;
                subclassType = selectedType.ToString();
                return;
            }

            Log.Error($"The provided type is not subclass of {typeof(T).ToString()}");

        }

        /// <summary>
        /// Gets the subclass type.
        /// </summary>
        public Type Type
        {
            get
            {
                Type selectedType = Type.GetType(selectedTypeQualifiedName);
                if (selectedType != null && (selectedType.IsSubclassOf(typeof(T)) || selectedType == typeof(T)))
                    return selectedType;

                parentTypeQualifiedName = typeof(T).AssemblyQualifiedName;
                selectedTypeQualifiedName = "Null";
                subclassType = "None";
                return null;
            }
            set
            {
                if (value == null)
                {
                    selectedTypeQualifiedName = "Null";
                    subclassType = "None";
                    return;
                }

                if (value.IsSubclassOf(typeof(T)))
                {
                    selectedTypeQualifiedName = value.AssemblyQualifiedName;
                    subclassType = value.ToString();
                    return;
                }

                Log.Error($"Type {value.ToString()} is not child of {typeof(T).ToString()}");
                selectedTypeQualifiedName = "Null";
                subclassType = "None";
                return;
            }
        }

        /// <summary>
        /// Gets the short name of the currently selected type.
        /// </summary>
        public string SelectedTypeName => subclassType;

        /// <summary>
        /// Gets the Assembly Qualified name of the currently selected type.
        /// </summary>
        public string SelectedTypeAssemblyQualifiedName => selectedTypeQualifiedName;

        /// <summary>
        /// Gets the Assembly Qualified name of the T type.
        /// </summary>
        public string ParentTypeAssemblyQualifiedName => parentTypeQualifiedName;

        /// <summary>
        /// Implicitly converts the given <see cref="UObject"/> instance to a <see cref="System.Type"/>.
        /// </summary>
        /// <param name="object">The <see cref="TSubclassOf{T}"/> to convert.</param>
        /// <returns>The <see cref="TSubclassOf{T}.Type"/>.</returns>
        public static implicit operator Type(TSubclassOf<T> @object) => @object.Type;

        /// <summary>
        /// Returns a string that represents the current obje
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"Type {subclassType} (parent: {parentTypeQualifiedName})";

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="object"/> is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            TSubclassOf<T> other = (TSubclassOf<T>)obj;
            return other != null && Type == other.Type;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => $"SubclassOf<{parentTypeQualifiedName}>: {selectedTypeQualifiedName}".GetHashCode();
    }
}
