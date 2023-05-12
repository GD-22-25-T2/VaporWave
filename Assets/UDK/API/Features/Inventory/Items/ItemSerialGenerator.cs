namespace UDK.API.Features.Inventory.Items
{
    /// <summary>
    /// The item serial generator.
    /// </summary>
    public static class ItemSerialGenerator
    {
        private static ushort AI;

        /// <summary>
        /// Generates a new serial number.
        /// </summary>
        /// <returns>The generated serial number.</returns>
        public static ushort GenerateNext() => AI > 65000 ? AI = 0 : AI += 1;
    }
}
