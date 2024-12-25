using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;

public class ShapeKeyView : MonoBehaviour
{
    public SerializableReactiveProperty<float> currentValue;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TextMeshProUGUI nameText;

    public int rendererInstanceID;
    public int shapeKeyIndex;

    private void Start()
    {
        slider.onValueChanged.AddListener((value) =>
        {
            currentValue.Value = value;
        });
    }

    public void SetName(string rendererName, string shapeKeyName)
    {
        nameText.text = $"{rendererName} - {shapeKeyName}";
    }

    public float MaxValue
    {
        set { slider.maxValue = value; }
    }

    public float MinValue
    {
        set { slider.minValue = value; }
    }
}
