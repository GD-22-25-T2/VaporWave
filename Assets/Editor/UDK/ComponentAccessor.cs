namespace UDK.API.Core
{
    using UnityEngine;
    using UnityEditor;
    using UDK.API.Features.Core;

    /// <summary>
    /// The editor component accessor.
    /// </summary>
    public class ComponentAccessor : MonoBehaviour
    {
        [MenuItem("UDK/ComponentBase/Reset")]
        private static void ReplaceReset()
        {
        }

        [MenuItem("UDK/ComponentBase/Reset", true)]
        private static bool ValidateReplacedReset() => false;

        [MenuItem("UDK/ComponentBase/Remove Component")]
        private static void RemoveComponent(MenuCommand command)
        {
            Actor target = (Actor)command.context;

            if (PrefabUtility.IsPartOfPrefabAsset(target))
            {
                string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
                GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);

                target.OnPrefabAssetRemoveComponentDel?.Invoke(root);

                DestroyImmediate(root.GetComponent<Actor>());

                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                PrefabUtility.UnloadPrefabContents(root);

                Undo.ClearAll();
                return;
            }

            Undo.DestroyObjectImmediate(target);
        }
    }
}
