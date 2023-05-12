namespace UDK.API.Features.Core.Framework.StateLib
{
    using UDK.API.Features.Core.Framework;

    /// <summary>
    /// The base controller which handles pawns using in-context states.
    /// <br>A <see cref="Pawn"/> will be automatically found, if possible, and assigned to the corresponding <see cref="Owner"/>.</br>
    /// </summary>
    public abstract class PawnStateController : StateController
    {
        /// <inheritdoc/>
        public new Pawn Owner { get; protected set; }

        /// <inheritdoc/>
        public new T GetOwner<T>()
            where T : Pawn => Owner.Cast<T>();

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            Owner = GetComponentSafe<Pawn>();
        }
    }
}
