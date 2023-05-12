namespace UDK.API.Features.Core
{
    using System.Reflection;
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using UDK.API.Features.Core.Generic;
    using System.Linq;
    using UDK.API.Features.Core.Framework.Pools;

    /// <summary>
    /// The base class of all unhandled objects.
    /// </summary>
    public abstract class UObject : TypeCastObject<UObject>
    {
        private static readonly Dictionary<Type, List<string>> RegisteredTypesValue = new();

        private bool destroyedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="UObject"/> class.
        /// </summary>
        protected UObject()
            : base() => InternalObjects.Add(this);

        /// <summary>
        /// Initializes a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        protected UObject(GameObject gameObject = null)
            : this()
        {
            if (gameObject)
                Base = gameObject;
        }

        /// <summary>
        /// Gets all the registered <see cref="UObject"/> types.
        /// </summary>
        public static IReadOnlyDictionary<Type, List<string>> RegisteredTypes => RegisteredTypesValue;

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the handled <see cref="GameObject"/>.
        /// </summary>
        public GameObject Base { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="UObject"/> values can be edited.
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Gets all the active <see cref="UObject"/> instances.
        /// </summary>
        internal static List<UObject> InternalObjects { get; } = new();

        /// <summary>
        /// Implicitly converts the given <see cref="UObject"/> instance to a <see cref="bool"/>.
        /// </summary>
        /// <param name="object">The <see cref="UObject"/> to convert.</param>
        /// <returns>Whether the <see cref="UObject"/> instance exists.</returns>
        public static implicit operator bool(UObject @object) => @object != null;

        /// <summary>
        /// Implicitly converts the given <see cref="UObject"/> instance to a <see cref="string"/>.
        /// </summary>
        /// <param name="object">The <see cref="UObject"/> to convert.</param>
        /// <returns>The <see cref="UObject"/> instance's name.</returns>
        public static implicit operator string(UObject @object) => @object.Name;

        /// <summary>
        /// Gets a <see cref="Type"/> from a given type name.
        /// </summary>
        /// <param name="typeName">The type name to look for.</param>
        /// <returns>A <see cref="Type"/> matching the type name or <see langword="null"/> if not found.</returns>
        public static Type GetObjectTypeByName(string typeName)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Name != typeName || type.BaseType != typeof(UObject))
                    continue;

                return type;
            }

            return null;
        }

        /// <summary>
        /// Registers the specified <see cref="UObject"/> type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="name">The name of the registered type.</param>
        /// <returns>The registered <see cref="Type"/>.</returns>
        public static Type RegisterObjectType<T>(string name)
            where T : UObject
        {
            Type matching = GetObjectTypeFromRegisteredTypes<T>(name);
            if (matching is not null)
                return matching;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.Name != typeof(T).Name)
                    continue;

                if (RegisteredTypesValue[t] is not null)
                {
                    RegisteredTypesValue[t].Add(name);
                }
                else
                {
                    List<string> values = new() { name };
                    RegisteredTypesValue.Add(t, values);
                }

                return typeof(T);
            }

            throw new NullReferenceException($"Couldn't find a defined UObject type for {name}");
        }

        /// <summary>
        /// Registers the specified <see cref="UObject"/> <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="name">The name of the registered type.</param>
        /// <returns>The registered <see cref="Type"/>.</returns>
        public static Type RegisterObjectType(Type type, string name)
        {
            Type matching = GetObjectTypeFromRegisteredTypes(type, name);
            if (matching is not null)
                return matching;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes()
                .Where(item => item.BaseType == typeof(UObject) || item.IsSubclassOf(typeof(UObject))))
            {
                if (t.Name != type.Name)
                    continue;

                if (RegisteredTypesValue.ContainsKey(t))
                {
                    RegisteredTypesValue[t].Add(name);
                }
                else
                {
                    List<string> values = new() { name };
                    RegisteredTypesValue.Add(t, values);
                }

                return t;
            }

            throw new NullReferenceException($"Couldn't find a defined UObject type for {name}");
        }

        /// <summary>
        /// Registers the specified <see cref="UObject"/> type.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <returns><see langword="true"/> if the type was unregistered successfully; otherwise, <see langword="false"/>.</returns>
        public static bool UnregisterObjectType(Type type) => RegisteredTypesValue.Remove(type);

        /// <summary>
        /// Unregisters the specified <see cref="UObject"/> type.
        /// </summary>
        /// <param name="name">The name of the type to unregister.</param>
        /// <returns><see langword="true"/> if the type was unregistered successfully; otherwise, <see langword="false"/>.</returns>
        public static bool UnregisterObjectType(string name)
        {
            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Value.Contains(name))
                    continue;

                RegisteredTypesValue.Remove(kvp.Key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the most accurate <see cref="Type"/> matching the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <param name="ignoreAbstractTypes">A value indicating whether abstract types should be ignored.</param>
        /// <returns>The <see cref="Type"/> with the name that matches the given name.</returns>
        public static Type FindObjectDefinedTypeByName(string name, bool ignoreAbstractTypes = true)
        {
            Type[] assemblyTypes =
                ignoreAbstractTypes ?
                Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract).ToArray() :
                Assembly.GetExecutingAssembly().GetTypes();

            List<int> matches = new();
            matches.AddRange(assemblyTypes.Select(type => LevenshteinDistance(type.Name, name)));
            return assemblyTypes[matches.IndexOf(matches.Min())];
        }

        /// <summary>
        /// Gets a <see cref="UObject"/> type from all the registered types.
        /// </summary>
        /// <typeparam name="T">The <see cref="UObject"/> type.</typeparam>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type is not the same as the specified type.</exception>
        public static Type GetObjectTypeFromRegisteredTypes<T>()
            where T : UObject
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Key != typeof(T))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="UObject"/> type from all the registered types.
        /// </summary>
        /// <typeparam name="T">The <see cref="UObject"/> type.</typeparam>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetObjectTypeFromRegisteredTypes<T>(string name)
            where T : UObject
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Key != typeof(T) || !kvp.Value.Contains(name))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="UObject"/> type from all the registered types.
        /// </summary>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type is not the same as the specified type.</exception>
        public static Type GetObjectTypeFromRegisteredTypes(Type type)
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Key != type)
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="UObject"/> type from all the registered types.
        /// </summary>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetObjectTypeFromRegisteredTypes(Type type, string name)
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Key != type || !kvp.Value.Contains(name))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="UObject"/> type from all the registered types.
        /// </summary>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetObjectTypeFromRegisteredTypesByName(string name) => RegisteredTypesValue.FirstOrDefault(kvp => kvp.Value.Contains(name)).Key;

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static UObject CreateDefaultSubobject(Type type, params object[] parameters)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            UObject @object = Activator.CreateInstance(type, flags, null, parameters, null) is not UObject outer ? null : outer;
            return !@object ? throw new NullReferenceException($"Couldn't create an UObject instance of type {type.Name}.") : @object;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="UObject"/> type.</typeparam>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type)
            where T : UObject => CreateDefaultSubobject(type, null).Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="UObject"/> type.</typeparam>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>()
            where T : UObject => CreateDefaultSubobject(typeof(T), null).Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="UObject"/> type to cast.</typeparam>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(params object[] parameters)
            where T : UObject => CreateDefaultSubobject(typeof(T), parameters).Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="UObject"/> type.</typeparam>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="UObject"/> instance.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(GameObject gameObject, string name)
            where T : UObject
        {
            if (CreateDefaultSubobject<T>() is not UObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer.Cast<T>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="UObject"/> type.</typeparam>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="UObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(GameObject gameObject, string name, params object[] parameters)
            where T : UObject
        {
            object newObj = CreateDefaultSubobject<T>(parameters);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="UObject"/> type.</typeparam>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="UObject"/> instance.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type, GameObject gameObject, string name)
            where T : UObject
        {
            object newObj = CreateDefaultSubobject<T>(type);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="UObject"/> type.</typeparam>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="UObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type, GameObject gameObject, string name, params object[] parameters)
            where T : UObject
        {
            object newObj = CreateDefaultSubobject<T>(type, parameters);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="UObject"/> instance.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static UObject CreateDefaultSubobject(Type type, GameObject gameObject, string name)
        {
            if (CreateDefaultSubobject(type) is not UObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="UObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="UObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="UObject"/> instance.</returns>
        public static UObject CreateDefaultSubobject(Type type, GameObject gameObject, string name, params object[] parameters)
        {
            if (CreateDefaultSubobject(type, parameters) is not UObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Destroys all the active <see cref="UObject"/> instances.
        /// </summary>
        public static void DestroyAllObjects()
        {
            List<UObject> objects = ListPool<UObject>.Pool.Get(InternalObjects);
            foreach (UObject @object in objects)
                @object.Destroy();

            objects.Clear();
            ListPool<UObject>.Pool.SharedReturn(objects);
        }

        /// <summary>
        /// Destroys all the active <typeparamref name="T"/> <see cref="UObject"/> instances.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        public static void DestroyAllObjectsOfType<T>()
            where T : UObject
        {
            List<UObject> objects = ListPool<UObject>.Pool.Get(InternalObjects);
            foreach (UObject @object in objects)
            {
                if (@object.Cast(out T obj))
                    obj.Destroy();
            }

            objects.Clear();
            ListPool<UObject>.Pool.SharedReturn(objects);
        }

        /// <summary>
        /// Finds the active <see cref="UObject"/> instances of type <typeparamref name="T"/> filtered based on a predicate.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>The corresponding active <typeparamref name="T"/> <see cref="UObject"/>.</returns>
        public static T FindActiveObjectOfType<T>(Func<UObject, bool> predicate)
            where T : UObject
        {
            foreach (UObject @object in InternalObjects.Where(predicate))
            {
                if (@object.Cast(out T obj))
                    return obj;
            }

            return null;
        }

        /// <summary>
        /// Finds all the active <see cref="UObject"/> instances of type <typeparamref name="T"/> filtered based on a predicate.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(Func<UObject, bool> predicate)
            where T : UObject
        {
            List<T> objects = ListPool<T>.Shared.Rent();
            foreach (UObject @object in InternalObjects.Where(predicate))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return ListPool<T>.Pool.ToArrayReturn(objects);
        }

        /// <summary>
        /// Finds all the active <see cref="UObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>()
            where T : UObject
        {
            List<T> objects = ListPool<T>.Pool.Get();
            foreach (UObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return ListPool<T>.Pool.ToArrayReturn(objects);
        }

        /// <summary>
        /// Finds all the active <see cref="UObject"/> instances of type <typeparamref name="T"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(string name)
            where T : UObject
        {
            List<T> objects = ListPool<T>.Pool.Get();
            foreach (UObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj) && (obj.Name == name))
                    objects.Add(obj);
            }

            return ListPool<T>.Pool.ToArrayReturn(objects);
        }

        /// <summary>
        /// Finds all the active <see cref="UObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(Type type)
            where T : UObject
        {
            List<T> objects = ListPool<T>.Pool.Get();
            foreach (UObject @object in InternalObjects.Where(obj => obj.GetType() == type))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return ListPool<T>.Pool.ToArrayReturn(objects);
        }

        /// <summary>
        /// Finds all the active <see cref="UObject"/> instances of type <typeparamref name="T"/> filtered based on a predicate.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="type">The <see cref="UObject"/> type.</param>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(Type type, Func<UObject, bool> predicate)
            where T : UObject
        {
            List<T> objects = ListPool<T>.Pool.Get();
            foreach (UObject @object in InternalObjects
                .Where(obj => obj.GetType() == type)
                .Where(predicate))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return ListPool<T>.Pool.ToArrayReturn(objects);
        }

        /// <summary>
        /// Finds all the active <see cref="UObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the elements that satify the condition.</returns>
        public static T[] FindActiveObjectsOfType<T>(Func<object, bool> predicate)
            where T : UObject
        {
            List<T> objects = ListPool<T>.Pool.Get();
            foreach (UObject @object in InternalObjects.Where(predicate).Cast<UObject>())
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return ListPool<T>.Pool.ToArrayReturn(objects);
        }

        /// <summary>
        /// Finds all the active <see cref="UObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the elements that satify the condition.</returns>
        public static T[] FindActiveObjectsOfType<T>(Func<T, bool> predicate)
            where T : UObject
        {
            List<T> objects = ListPool<T>.Pool.Get();
            foreach (UObject @object in InternalObjects
                .Where(obj => obj.Cast(out T _))
                .Select(obj => obj.Cast<T>())
                .Where(predicate))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return ListPool<T>.Pool.ToArrayReturn(objects);
        }

        /// <summary>
        /// Destroys all the active <see cref="UObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        public static void DestroyActiveObjectsOfType<T>()
            where T : UObject
        {
            foreach (UObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj))
                    obj.Destroy();
            }
        }

        /// <summary>
        /// Destroys an active <see cref="UObject"/> instance of type <typeparamref name="T"/> given the specified <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="gameObject">The <see cref="GameObject"/> belonging to the <see cref="UObject"/> instance to be destroyed.</param>
        /// <returns><see langword="true"/> if the object was destroyed; otherwise, <see langword="false"/>.</returns>
        public static bool DestroyActiveObject<T>(GameObject gameObject)
            where T : UObject
        {
            foreach (UObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj) && (obj.Base == gameObject))
                {
                    obj.Destroy();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Destroys an active <see cref="UObject"/> instance given the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <param name="gameObject">The <see cref="GameObject"/> belonging to the object.</param>
        /// <returns><see langword="true"/> if the object was destroyed; otherwise, <see langword="false"/>.</returns>
        public static bool DestroyActiveObject(Type type, GameObject gameObject)
        {
            foreach (UObject @object in InternalObjects)
            {
                if ((@object.GetType() == type) && (@object.Base == gameObject))
                {
                    @object.Destroy();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds the most appropriate entry.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="name">The name to pair.</param>
        /// <param name="source">The source on which iterate on.</param>
        /// <returns>The corresponding entry or <see langword="default"/> if not found.</returns>
        public static T FindMostAppropriateEntry<T>(string name, IEnumerable<T> source)
        {
            List<int> matches = new();
            matches.AddRange(source.Select(type => LevenshteinDistance(type.GetType().Name, name)));
            return source.ElementAt(matches.IndexOf(matches.Min()));
        }

        /// <summary>
        /// Destroys the current <see cref="UObject"/> instance.
        /// </summary>
        public void Destroy()
        {
            Destroy(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 23;
                hash = (hash * 29) + Base.GetHashCode();
                hash = (hash * 29) + Name.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="object"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="object"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the <see cref="object"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object other) => other is not null && other is UObject && other == this;

        /// <inheritdoc cref="Destroy()"/>
        protected virtual void Destroy(bool destroying)
        {
            if (!destroyedValue)
            {
                if (destroying)
                {
                    OnBeginDestroy();
                    InternalObjects.Remove(this);
                }

                OnDestroyed();
                destroyedValue = true;
            }
        }

        /// <summary>
        /// Fired before the current <see cref="UObject"/> instance is destroyed.
        /// </summary>
        protected virtual void OnBeginDestroy()
        {
        }

        /// <summary>
        /// Fired when the current <see cref="UObject"/> instance has been explicitly destroyed.
        /// </summary>
        protected virtual void OnDestroyed()
        {
        }

        private static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(target) ? 0 : target.Length;

            if (string.IsNullOrEmpty(target))
                return source.Length;

            if (source.Length > target.Length)
                (source, target) = (target, source);

            int m = target.Length;
            int n = source.Length;
            int[,] distance = new int[2, m + 1];

            for (int j = 1; j <= m; j++)
                distance[0, j] = j;

            int currentRow = 0;
            for (int i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                int previousRow = currentRow ^ 1;
                for (int j = 1; j <= m; j++)
                {
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] = Math.Min(
                        Math.Min(
                            distance[previousRow, j] + 1,
                            distance[currentRow, j - 1] + 1),
                        distance[previousRow, j - 1] + cost);
                }
            }

            return distance[currentRow, m];
        }
    }
}
