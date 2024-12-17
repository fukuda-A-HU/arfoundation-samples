using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using R3;

[RequireComponent(typeof(Toggle))]
public class PlaneVisualizerModel : MonoBehaviour
{
    private Toggle toggle;

    public SerializableReactiveProperty<bool> isOn = new SerializableReactiveProperty<bool>();

    private void Start()
    {
        toggle = GetComponent<Toggle>();
    }

    public void CangeValue()
    {
        isOn.Value = toggle.isOn;
    }


}
