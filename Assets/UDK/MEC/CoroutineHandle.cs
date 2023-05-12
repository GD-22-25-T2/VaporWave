namespace UDK.MEC
{
    using System;

    public struct CoroutineHandle : IEquatable<CoroutineHandle>
    {
        private const byte ReservedSpace = 15;

        private static readonly int[] NextIndex = new int[16]
        {
            16, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0
        };

        private readonly int _id;

        public byte Key => (byte)((uint)_id & 0xFu);

        public string Tag
        {
            get => Timing.GetTag(this);
            set => Timing.SetTag(this, value);
        }

        public int? Layer
        {
            get => Timing.GetLayer(this);
            set
            {
                if (!value.HasValue)
                {
                    Timing.RemoveLayer(this);
                    return;
                }

                Timing.SetLayer(this, value.Value);
            }
        }

        public Segment Segment
        {
            get
            {
                return Timing.GetSegment(this);
            }
            set
            {
                Timing.SetSegment(this, value);
            }
        }

        public bool IsRunning
        {
            get
            {
                return Timing.IsRunning(this);
            }
            set
            {
                if (!value)
                {
                    Timing.KillCoroutines(this);
                }
            }
        }

        public bool IsAliveAndPaused
        {
            get
            {
                return Timing.IsAliveAndPaused(this);
            }
            set
            {
                if (value)
                {
                    Timing.PauseCoroutines(this);
                }
                else
                {
                    Timing.ResumeCoroutines(this);
                }
            }
        }

        public bool IsValid => Key != 0;

        public CoroutineHandle(byte ind)
        {
            if (ind > 15)
            {
                ind = (byte)(ind - 15);
            }

            _id = NextIndex[ind] + ind;
            NextIndex[ind] += 16;
        }

        public CoroutineHandle(CoroutineHandle other) => _id = other._id;

        public bool Equals(CoroutineHandle other) => _id == other._id;

        public override bool Equals(object other) => other is CoroutineHandle && Equals((CoroutineHandle)other);

        public static bool operator ==(CoroutineHandle a, CoroutineHandle b) => a._id == b._id;

        public static bool operator !=(CoroutineHandle a, CoroutineHandle b) => a._id != b._id;

        public override int GetHashCode() => _id;

        public override string ToString()
        {
            if (Timing.GetTag(this) == null)
                return !Timing.GetLayer(this).HasValue ? Timing.GetDebugName(this) : Timing.GetDebugName(this) + " Layer: " + Timing.GetLayer(this);

            return !Timing.GetLayer(this).HasValue ?
                Timing.GetDebugName(this) + " Tag: " + Timing.GetTag(this) :
                Timing.GetDebugName(this) + " Tag: " + Timing.GetTag(this) + " Layer: " + Timing.GetLayer(this);
        }
    }
}
