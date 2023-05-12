namespace UDK.API.Features.Entity.Stats.SyncedStats
{
    using UnityEngine;
    using System.Collections.Generic;
    using UDK.API.Features.Core.Framework;

    /// <summary>
    /// Defines a stat module which implements an artificial health layer over the <see cref="HealthStat"/> module.
    /// </summary>
    public sealed class ArtificialHealthStat : SyncedStatBase
    {
        /// <summary>
        /// The class to define artificial health processes.
        /// </summary>
        public class ArtificialHealthProcess
        {
            /// <summary>
            /// The kill code AI.
            /// </summary>
            public static int KILL_CODE_AI;

            /// <summary>
            /// Initializes a new instance of the <see cref="ArtificialHealthProcess"/> class.
            /// </summary>
            /// <param name="amount">The amount of artificial health.</param>
            /// <param name="limit">The process cap.</param>
            /// <param name="decay">The decay amount.</param>
            /// <param name="efficacy">The efficacy of the process.</param>
            /// <param name="sustain">The sustain of the process.</param>
            /// <param name="persistant">Whether the process persists during the game.</param>
            public ArtificialHealthProcess(float amount, float limit, float decay, float efficacy, float sustain, bool persistant)
            {
                KILL_CODE_AI++;
                CurrentAmount = amount;
                Limit = limit;
                DecayRate = decay;
                Efficacy = efficacy;
                SustainTime = sustain;
                Persistant = persistant;
                KillCode = KILL_CODE_AI;
            }

            /// <summary>
            /// Gets or sets the current artificial health amount.
            /// </summary>
            public float CurrentAmount { get; set; }

            /// <summary>
            /// Gets or sets the artificial health limit amount.
            /// </summary>
            public float Limit { get; set; }

            /// <summary>
            /// Gets or sets the decay rate.
            /// </summary>
            public float DecayRate { get; set; }

            /// <summary>
            /// Gets or sets the efficacy.
            /// </summary>
            public float Efficacy { get; set; }

            /// <summary>
            /// Gets or sets the sustain time.
            /// </summary>
            public float SustainTime { get; set; }

            /// <summary>
            /// Gets a value indicating whether the process is persistant.
            /// </summary>
            public bool Persistant { get; }

            /// <summary>
            /// Gets the kill code.
            /// </summary>
            public int KillCode { get; }
        }

        /// <summary>
        /// The default maximum limit value.
        /// </summary>
        public const float DEFAULT_MAX = 75f;

        /// <summary>
        /// The default maximum efficacy value.
        /// </summary>
        public const float DEFAULT_EFFICACY = 0.7f;

        /// <summary>
        /// The default maximum decay value.
        /// </summary>
        public const float DEFAULT_DECAY = 1.2f;

        private float _maxValue;
        private readonly List<ArtificialHealthProcess> _activeProcesses = new();

        /// <inheritdoc/>
        public override float MinValue => 0f;

        /// <inheritdoc/>
        public override float MaxValue => CustomMaxValue > 0 ? CustomMaxValue : _maxValue;

        /// <summary>
        /// Gets all the active processes.
        /// </summary>
        public IEnumerable<ArtificialHealthProcess> ActiveProcesses => _activeProcesses;

        /// <inheritdoc/>
        public override void Init(Pawn pawn)
        {
            base.Init(pawn);

            _maxValue = DEFAULT_MAX;
        }

        /// <inheritdoc/>
        protected override void OnValueChanged(float prevValue, float newValue)
        {
            base.OnValueChanged(prevValue, newValue);

            if (CustomMaxValue > 0f)
            {
                CustomMaxValue = Mathf.Max(CustomMaxValue, newValue);
                return;
            }

            _maxValue = Mathf.Max(_maxValue, newValue);
        }

        /// <summary>
        /// Adds an <see cref="ArtificialHealthProcess"/> to the active processes.
        /// </summary>
        /// <param name="amount">The amount of artificial health.</param>
        /// <param name="limit">The process cap.</param>
        /// <param name="decay">The decay amount.</param>
        /// <param name="efficacy">The efficacy of the process.</param>
        /// <param name="sustain">The sustain of the process.</param>
        /// <param name="persistant">Whether the process persists during the game.</param>
        /// <returns>The added <see cref="ArtificialHealthProcess"/>.</returns>
        public ArtificialHealthProcess AddProcess(float amount, float limit, float decay, float efficacy, float sustain, bool persistant)
        {
            float localAmount = 0f;
            float localLimit = limit;

            foreach (ArtificialHealthProcess process in _activeProcesses)
            {
                localAmount += process.CurrentAmount;
                localLimit += Mathf.Min(localLimit, process.Limit);
            }

            float partialAmount = localAmount + amount - localLimit;
            if (partialAmount > 0f)
                amount = Mathf.Max(0f, amount - partialAmount);

            ArtificialHealthProcess ahpProcess = new(amount, limit, decay, efficacy, sustain, persistant);
            for (int i = 0; i < _activeProcesses.Count; i++)
            {
                if (efficacy >= _activeProcesses[i].Efficacy)
                {
                    _activeProcesses.Insert(i, ahpProcess);
                    return ahpProcess;
                }
            }

            _activeProcesses.Add(ahpProcess);
            return ahpProcess;
        }

        /// <summary>
        /// Adds an <see cref="ArtificialHealthProcess"/> to the active processes.
        /// </summary>
        /// <param name="artificialHealthAmount">The amount of the artificial health.</param>
        /// <returns>The added <see cref="ArtificialHealthProcess"/>.</returns>
        public ArtificialHealthProcess AddProcess(float amount) => AddProcess(amount, DEFAULT_MAX, DEFAULT_DECAY, DEFAULT_EFFICACY, 0f, false);

        /// <summary>
        /// Tries to get a process.
        /// </summary>
        /// <param name="killCode">The kill code.</param>
        /// <param name="process">The found process.</param>
        /// <returns><see langword="true"/> if the process was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetProcess(int killCode, out ArtificialHealthProcess process)
        {
            foreach (ArtificialHealthProcess ahpProcess in _activeProcesses)
            {
                if (ahpProcess.KillCode == killCode)
                {
                    process = ahpProcess;
                    return true;
                }
            }

            process = null;
            return false;
        }

        /// <summary>
        /// Kills a process.
        /// </summary>
        /// <param name="killCode">The kill code.</param>
        /// <returns><see langword="true"/> if the process was killed; otherwise, <see langword="false"/>.</returns>
        public bool KillProcess(int killCode) => TryGetProcess(killCode, out ArtificialHealthProcess process) && _activeProcesses.Remove(process);

        /// <summary>
        /// Processes the specified amount of damage.
        /// </summary>
        /// <param name="damage">The damage to process.</param>
        /// <returns>The processed damage.</returns>
        public float ProcessDamage(float damage)
        {
            if (damage <= 0f)
                return damage;

            foreach (ArtificialHealthProcess process in _activeProcesses)
            {
                float amount = damage * process.Efficacy;
                if (amount < process.CurrentAmount)
                {
                    process.CurrentAmount -= amount;
                    return damage - amount;
                }

                damage -= process.CurrentAmount;
                process.CurrentAmount = 0f;
            }

            return damage;
        }

        /// <summary>
        /// Updates all the processes.
        /// </summary>
        public void UpdateProcesses()
        {
            float amount = 0f;
            List<ArtificialHealthProcess> processes = new();
            for (int i = 0; i < _activeProcesses.Count; i++)
            {
                ArtificialHealthProcess ahpProcess = _activeProcesses[i];
                amount += ahpProcess.CurrentAmount;
                if (ahpProcess.SustainTime > 0f)
                {
                    ahpProcess.SustainTime -= Time.deltaTime;
                    continue;
                }

                ahpProcess.CurrentAmount = Mathf.Clamp(ahpProcess.CurrentAmount - (ahpProcess.DecayRate * Time.deltaTime), 0f, ahpProcess.Limit);
                if (ahpProcess.CurrentAmount == 0f && !ahpProcess.Persistant)
                    processes.Add(ahpProcess);
            }

            foreach (ArtificialHealthProcess process in processes)
                _activeProcesses.Remove(process);

            CurrentValue = Mathf.Max(MinValue, amount);
        }
    }
}
