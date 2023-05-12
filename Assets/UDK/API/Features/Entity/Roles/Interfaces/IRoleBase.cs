namespace UDK.API.Features.Entity.Roles.Interfaces
{
    /// <summary>
    /// Defines the base contract for roles.
    /// </summary>
    public interface IRoleBase
    {
        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the id of the role.
        /// </summary>
        public uint Id { get; }
    }
}
