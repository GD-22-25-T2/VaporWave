namespace UDK.API.Features.Core.Framework.Navigation
{
    using UnityEngine;

    /// <summary>
    /// A navigation component used to define a player start spot.
    /// </summary>
    [AddComponentMenu("UDK/Core/Framework/Navigation/PlayerStart")]
    [DisallowMultipleComponent]
    public sealed class PlayerStart : NavigationObjectBase
    {
        [SerializeField]
        private string _controllerTag;
        
        /// <summary>
        /// Gets or sets the controller tag.
        /// </summary>
        public string ControllerTag
        {
            get => _controllerTag;
            set => _controllerTag = value;
        }
    }
}
