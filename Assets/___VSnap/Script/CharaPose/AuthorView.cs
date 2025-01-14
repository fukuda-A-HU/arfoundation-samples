using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;

public class AuthorView : MonoBehaviour
{
    private TextMeshProUGUI label;
    [SerializeField] private Color basicColor;
    [SerializeField] private Color SelectedColor;

    public string authorName;

    public RectTransform poseButtonParent;
    public Button button;

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            button.GetComponent<Image>().color = SelectedColor;
        }
        else
        {
            button.GetComponent<Image>().color = basicColor;
        }
    }

    public void SetButton(Button button, TextMeshProUGUI label, string name)
    {
        this.button = button;
        this.label = label;
        label.text = name;
        authorName = name;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
