using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapeKeySliderView : MonoBehaviour
{
    public ExtendedSlider extendedSlider;
    [SerializeField] private TextMeshProUGUI text;

    public void SetName(string rendererName, string shapeKeyName)
    {
        text.text = $"{rendererName} - {shapeKeyName}";
    }
}
