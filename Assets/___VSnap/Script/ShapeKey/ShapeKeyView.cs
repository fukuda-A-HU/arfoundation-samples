using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;

public class ShapeKeyView : MonoBehaviour
{
    public Button button;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField] private Color maxColor;
    [SerializeField] private Color minColor;

    public int rendererInstanceID;
    public int shapeKeyIndex;

    public void SetName(string rendererName, string shapeKeyName)
    {
        text.text = $"{rendererName} - {shapeKeyName}";
    }

    public void SetColor(float value)
    {
        button.GetComponent<Image>().color = Color.Lerp(minColor, maxColor, value/100);
    }
}
