using RootMotion.Demos;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharaPoseView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] private Color basicColor;
    [SerializeField] private Color SelectedColor;

    public Button button;

    public string authorName;

    public string poseName;

    void Start()
    {
        button = GetComponent<Button>();
    }

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
    
    public void SetName(string authorName, string name)
    {
        this.authorName = authorName;
        this.poseName = name;
        label.text = name;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
