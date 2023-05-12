namespace UDK.API.Features
{
    /// <summary>
    /// A set of constants values used to evaluate data and perform calculations.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Represents a small number.
        /// </summary>
        public const float KINDA_SMALL_NUMBER = 0.000001f;

        /// <summary>
        /// Represents a big number.
        /// </summary>
        public const float BIG_NUMBER = float.MaxValue;

        /// <summary>
        /// Represents the minimum delta frame update time.
        /// </summary>
        public const float MIN_DELTA_TIME = 0.016f;
    }
}
