namespace UDK.API.Features.Entity.Roles.Attributes
{
    using System;

    /// <summary>
    /// An attribute to easily initialize playable roles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RoleSerializableAttribute : Attribute
    {
    }
}
