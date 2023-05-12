namespace UDK.API.Features.Entity.Stats
{
    using UnityEngine;
    using System.Collections.Generic;
    using DamageHandlers;
    using System;
    using UDK.API.Features.Entity.Stats.SyncedStats;
    using UDK.API.Features.Entity.Roles;
    using UDK.API.Features.Entity.Roles.Interfaces;
    using UDK.API.Features.Core;

    /// <summary>
    /// The base component to manage stat modules.
    /// <para>
    /// It's strictly dependent on the <see cref="Entity"/> as it supports entities derived from the <see cref="EntityBase"/> class.
    /// </para>
    /// <para>
    /// The current implementation is not network replicated.
    /// </para>
    /// </summary>
    [AddComponentMenu("UDK/Entity/Stats/StatsBase")]
    [DisallowMultipleComponent]
    public class StatsBase : PawnComponent
    {
        private readonly Dictionary<Type, SyncedStatBase> _dictionarizedTypes = new();

        static StatsBase()
        {
            OnAnyEntityHurt = (EntityBase target, DamageHandlerBase usedHandler) => { };
            OnAnyEntityDied = (EntityBase taget, DamageHandlerBase usedHandler) => { };
        }

        /// <summary>
        /// Invoked when an entity was hurt.
        /// </summary>
        public static event Action<EntityBase, DamageHandlerBase> OnAnyEntityHurt;

        /// <summary>
        /// Invoked when an entity died.
        /// </summary>
        public static event Action<EntityBase, DamageHandlerBase> OnAnyEntityDied;

        /// <summary>
        /// Invoked when the entity was hurt.
        /// </summary>
        public event Action<DamageHandlerBase> OnEntityHurt = (DamageHandlerBase damageHandlerBase) => { };

        /// <summary>
        /// Invoked when the entity died.
        /// </summary>
        public event Action<DamageHandlerBase> OnEntityDied = (DamageHandlerBase damageHandlerBase) => { };

        /// <summary>
        /// Gets or sets a value indicating whether the damage should be ignored.
        /// </summary>
        public bool IsDamageIgnored { get; set; }

        /// <summary>
        /// Gets a value indicating whether the owner is alive.
        /// </summary>
        public virtual bool IsAlive => GetModule<HealthStat>().CurrentValue > 0f && Owner.Cast<EntityBase>().Role.Id != RoleManager.DEFAULT_ROLE;

        /// <summary>
        /// The active modules.
        /// </summary>
        public SyncedStatBase[] Modules { get; } = new SyncedStatBase[]
        {
            new HealthStat(),
            new ArtificialHealthStat(),
        };

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            foreach (SyncedStatBase statBase in Modules)
                _dictionarizedTypes.Add(statBase.GetType(), statBase);
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            foreach (SyncedStatBase statBase in Modules)
                statBase.Init(Owner.Cast<EntityBase>());
        }

        /// <inheritdoc/>
        protected override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            foreach (SyncedStatBase module in Modules)
                module.Update();
        }

        /// <summary>
        /// Gets a module.
        /// </summary>
        /// <typeparam name="T">The type of the module.</typeparam>
        /// <returns>The corresponding module.</returns>
        public T GetModule<T>()
            where T : SyncedStatBase => _dictionarizedTypes[typeof(T)] as T;

        /// <summary>
        /// Tries to get a module given the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the module.</typeparam>
        /// <param name="module">The corresponding module.</param>
        /// <returns><see langword="true"/> if the module was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetModule<T>(out T module)
            where T : SyncedStatBase
        {
            T t;
            if (_dictionarizedTypes.TryGetValue(typeof(T), out SyncedStatBase statBase) && (t = statBase as T) is not null)
            {
                module = t;
                return true;
            }

            module = default;
            return false;
        }

        /// <summary>
        /// Deals damage to the owner.
        /// </summary>
        /// <param name="damageHandler">The damage handler.</param>
        /// <returns><see langword="true"/> if the damage was dealt; otherwise, <see langword="false"/>.</returns>
        public virtual bool DealDamage(DamageHandlerBase damageHandler)
        {
            if (Owner.Cast<EntityBase>().Stats.IsDamageIgnored)
                return false;

            if (Owner.Cast<EntityBase>().RoleManager.CurrentRole is IDamageHandlerProcessorRole damageHandlerProcessorRole)
                damageHandler = damageHandlerProcessorRole.ProcessDamageHandler(damageHandler);

            DamageHandlerBase handlerBase = damageHandler;
            DamageHandlerBase.HandlerOutput handlerOutput = damageHandler.ApplyDamage(Owner.Cast<EntityBase>());
            if (handlerOutput is DamageHandlerBase.HandlerOutput.None)
                return false;

            if (OnAnyEntityHurt is not null)
                OnAnyEntityHurt(Owner.Cast<EntityBase>(), damageHandler);

            if (OnEntityHurt is not null)
                OnEntityHurt(damageHandler);

            if (handlerOutput is DamageHandlerBase.HandlerOutput.Dead)
            {
                if (OnAnyEntityDied is not null)
                    OnAnyEntityDied(Owner.Cast<EntityBase>(), damageHandler);

                if (OnEntityDied is not null)
                    OnEntityDied(damageHandler);

                Kill(damageHandler);
            }

            return true;
        }

        /// <summary>
        /// Kills the owner.
        /// </summary>
        /// <param name="_">The damage handler.</param>
        public virtual void Kill(DamageHandlerBase damageHandler)
        {
        }
    }
}
