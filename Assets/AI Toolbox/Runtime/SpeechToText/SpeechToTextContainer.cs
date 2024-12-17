using UnityEngine;

namespace AiToolbox {
public static partial class SpeechToText {
    private class SpeechToTextContainer : MonoBehaviour {
        private static SpeechToTextContainer _instance;
        internal static SpeechToTextContainer instance {
            get {
                if (_instance == null) {
                    var container = new GameObject("SpeechToText");
                    DontDestroyOnLoad(container);
                    container.hideFlags = HideFlags.HideInHierarchy;
                    _instance = container.AddComponent<SpeechToTextContainer>();
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