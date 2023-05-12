namespace UDK.API.Features.Entity.Roles
{
    using System.Collections.Generic;
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Linq;
    using UDK.API.Features.Entity.Roles.Interfaces;
    using UDK.API.Features.Entity.Roles.Attributes;

    /// <summary>
    /// The base class which defines a playable role.
    /// </summary>
    public abstract class RoleBase : IRoleBase
    {
        private static readonly HashSet<RoleBase> _registered = new();

        /// <summary>
        /// Gets all the registered roles.
        /// </summary>
        public static IEnumerable<RoleBase> Registered => _registered;

        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public abstract uint Id { get; }

        /// <summary>
        /// Gets the last owner of the role.
        /// </summary>
        public EntityBase LastOwner { get; protected set; }

        /// <summary>
        /// Gets a <see cref="Stopwatch"/> counting the role active time.
        /// </summary>
        public Stopwatch ActiveTime { get; } = Stopwatch.StartNew();

        /// <summary>
        /// Invoked when the role gets disabled.
        /// </summary>
        public Action<uint> OnRoleDisabled { get; set; }

        /// <summary>
        /// Gets a role given the specified name.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        /// <returns>A <see cref="RoleBase"/>, or <see langword="null"/> if not found.</returns>
        public static RoleBase Get(string name) => _registered.FirstOrDefault(r => r.Name == name);

        /// <summary>
        /// Gets a role given the specified id.
        /// </summary>
        /// <param name="id">The id of the role.</param>
        /// <returns>A <see cref="RoleBase"/>, or <see langword="null"/> if not found.</returns>
        public static RoleBase Get(uint id) => _registered.FirstOrDefault(r => r.Id == id);

        /// <summary>
        /// Gets a role given the specified value.
        /// </summary>
        /// <param name="id">The value to parse.</param>
        /// <returns>A <see cref="RoleBase"/>, or <see langword="null"/> if not found.</returns>
        public static RoleBase Get(object value)
        {
            try
            {
                return Get((string)value);
            }
            catch
            {
            }

            try
            {
                return Get((uint)value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Registers all the roles in the current assembly.
        /// </summary>
        /// <returns>The registered roles.</returns>
        public static IEnumerable<RoleBase> RegisterRoles()
        {
            List<RoleBase> registered = new();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttribute<RoleSerializableAttribute>() is null ||
                    type.BaseType != typeof(RoleBase) && !type.IsSubclassOf(typeof(RoleBase)))
                    continue;

                RoleBase entityRoleBase = Activator.CreateInstance(type) as RoleBase;
                registered.Add(entityRoleBase);
                entityRoleBase.Register();
            }

            return registered;
        }

        /// <summary>
        /// Unregisters all the registered roles.
        /// </summary>
        /// <returns>The unregistered roles.</returns>
        public static IEnumerable<RoleBase> UnregisterRoles()
        {
            List<RoleBase> unregistered = new();

            foreach (RoleBase role in Registered)
            {
                unregistered.Add(role);
                role.Unregister();
            }

            return unregistered;
        }

        /// <summary>
        /// Registers the role.
        /// </summary>
        protected virtual void Register() => _registered.Add(this);

        /// <summary>
        /// Unregisters the role.
        /// </summary>
        protected virtual void Unregister() => _registered.Remove(this);

        /// <summary>
        /// Initializes the role to the specified <see cref="EntityBase"/>.
        /// </summary>
        /// <param name="entity">The owner.</param>
        public virtual void Init(EntityBase entity)
        {
            LastOwner = entity;
            ActiveTime.Restart();
        }

        /// <summary>
        /// Disables the role.
        /// </summary>
        /// <param name="newRole">The new role.</param>
        public virtual void DisableRole(uint newRole)
        {
            if (OnRoleDisabled is not null)
                OnRoleDisabled(newRole);
        }
    }
}
