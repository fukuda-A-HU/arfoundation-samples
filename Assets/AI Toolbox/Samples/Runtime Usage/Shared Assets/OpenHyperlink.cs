using UnityEngine;

namespace AiToolboxRuntimeSample {
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OpenHyperlink : MonoBehaviour {
    [SerializeField, Tooltip("The hyperlink to open when the function is called.")]
    private string _hyperlink = "https://ai-toolbox.dustyroom.com";

    public virtual void OpenLink() {
        Application.OpenURL(_hyperlink);
    }
}
}