using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;

public class ShapeKeyView : MonoBehaviour
{
    public Button button;

    [SerializeField]
    private TextMeshProUGUI text;

    public int rendererInstanceID;
    public int shapeKeyIndex;

    public void SetName(string rendererName, string shapeKeyName)
    {
        text.text = $"{rendererName} - {shapeKeyName}";
    }
}
