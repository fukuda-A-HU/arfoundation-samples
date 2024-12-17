using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
[ExecuteAlways]
public class CopyColorFromParent : MonoBehaviour {
    private Graphic _graphicSelf;
    private Graphic _graphicParent;

    private void Start() {
        _graphicSelf = GetComponent<Graphic>();
        _graphicParent = transform.parent.GetComponent<Graphic>();
        if (_graphicParent == null) {
            Debug.LogError($"Parent of {name} does not have a Graphic component.", this);
            enabled = false;
        }
    }

    private void Update() {
        _graphicSelf.color = _graphicParent.canvasRenderer.GetColor();
    }
}