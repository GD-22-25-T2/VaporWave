namespace UDK.MEC
{
    public enum Segment
    {
        Invalid = -1,
        Update,
        FixedUpdate,
        LateUpdate,
        SlowUpdate,
        RealtimeUpdate,
        EditorUpdate,
        EditorSlowUpdate,
        EndOfFrame,
        ManualTimeframe
    }
}
