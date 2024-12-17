using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthorView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;

    public string authorName;

    public Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }
    
    public void SetName(string name)
    {
        label.text = name;
        authorName = name;
    }
}
