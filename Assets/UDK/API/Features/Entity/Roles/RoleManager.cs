namespace UDK.API.Features.Entity.Roles
{
    using UDK.API.Features.Core;
    using UnityEngine;

    /// <summary>
    /// The base component which handles in-game roles.
    /// </summary>
    [AddComponentMenu("UDK/Entity/Roles/RoleManager")]
    [DisallowMultipleComponent]
    public class RoleManager : PawnComponent
    {
        /// <summary>
        /// Invoked when the role is changed.
        /// </summary>
        public static event RoleChanged OnRoleChanged;

        /// <summary>
        /// Represents the default role.
        /// </summary>
        public const uint DEFAULT_ROLE = 0;

        private bool _anySet;
        private RoleBase _curRole;

        /// <summary>
        /// Called when the role is changed.
        /// </summary>
        /// <param name="entity">The owner.</param>
        /// <param name="prevRole">The previous role.</param>
        /// <param name="newRole">The new role.</param>
        public delegate void RoleChanged(EntityBase entity, RoleBase prevRole, RoleBase newRole);

        /// <summary>
        /// Gets the current role.
        /// </summary>
        public RoleBase CurrentRole => _curRole;

        /// <summary>
        /// Initializes a new role.
        /// </summary>
        /// <param name="id">The role to initialize.</param>
        public void InitializeNewRole(uint id)
        {
            RoleBase prevRole = null;

            bool wasAnySet = _anySet;
            if (wasAnySet)
            {
                prevRole = CurrentRole;
                prevRole.DisableRole(id);
            }

            RoleBase roleBase = RoleBase.Get(id);
            _curRole = roleBase;
            _curRole.Init(Owner.Cast<EntityBase>());

            if (wasAnySet && OnRoleChanged is not null)
                OnRoleChanged(Owner.Cast<EntityBase>(), prevRole, CurrentRole);

            _anySet = true;
        }

        /// <summary>
        /// Initialized a new role.
        /// </summary>
        /// <param name="obj">The role to initialize.</param>
        public void InitializeNewRole(object obj)
        {
            try
            {
                InitializeNewRole(RoleBase.Get((string)obj).Id);
            }
            catch
            {
            }

            try
            {
                InitializeNewRole(RoleBase.Get((uint)obj).Id);
            }
            catch
            {
            }
        }
    }
}
