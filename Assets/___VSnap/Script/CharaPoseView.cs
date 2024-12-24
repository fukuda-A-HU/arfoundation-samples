using RootMotion.Demos;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharaPoseView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;

    public Button button;

    public string authorName;

    public string poseName;

    void Start()
    {
        button = GetComponent<Button>();
    }
    
    public void SetName(string authorName, string name)
    {
        this.authorName = authorName;
        this.poseName = name;
        label.text = name;
    }
}
