using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;

public class AuthorView : MonoBehaviour
{
    private TextMeshProUGUI label;

    public string authorName;

    public RectTransform poseButtonParent;
    public Button button;

    public void SetButton(Button button, TextMeshProUGUI label, string name)
    {
        this.button = button;
        this.label = label;
        label.text = name;
        authorName = name;
    }
}
