using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapeKeySliderView : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    public void SetName(string rendererName, string shapeKeyName)
    {
        text.text = $"{rendererName} - {shapeKeyName}";
    }
}
