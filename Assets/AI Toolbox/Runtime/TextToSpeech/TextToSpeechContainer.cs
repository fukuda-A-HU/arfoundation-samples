using UnityEngine;

namespace AiToolbox {
public static partial class TextToSpeech {
    private class TextToSpeechContainer : MonoBehaviour {
        private static TextToSpeechContainer _instance;
        internal static TextToSpeechContainer instance {
            get {
                if (_instance == null) {
                    var container = new GameObject("TextToSpeech");
                    DontDestroyOnLoad(container);
                    container.hideFlags = HideFlags.HideInHierarchy;
                    _instance = container.AddComponent<TextToSpeechContainer>();
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