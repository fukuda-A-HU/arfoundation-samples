using UnityEngine;

namespace AiToolbox {
public static partial class Moderation {
    private class ModerationContainer : MonoBehaviour {
        private static ModerationContainer _instance;
        internal static ModerationContainer instance {
            get {
                if (_instance == null) {
                    var container = new GameObject("Moderation");
                    DontDestroyOnLoad(container);
                    container.hideFlags = HideFlags.HideInHierarchy;
                    _instance = container.AddComponent<ModerationContainer>();
                }

                return _instance;
            }
        }

        private void OnApplicationQuit() {
            CancelAllRequests();
        }
    }
}
}