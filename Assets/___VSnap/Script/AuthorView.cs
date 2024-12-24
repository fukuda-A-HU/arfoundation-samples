using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;

public class AuthorView : MonoBehaviour
{
    private TextMeshProUGUI label;

    public RectTransform poseButtonParent;

    public string authorName;

    public Button button;

    public void SetButton(Button button, TextMeshProUGUI label, string name)
    {
        this.button = button;
        this.label = label;
        label.text = name;
        authorName = name;
    }

    public void SetShowingAuthorName(string name)
    {
        label.text = name;
    }

    public void JudgeShowButton(string _authorName)
    {
        if (authorName == _authorName)
        {
            poseButtonParent.gameObject.SetActive(true);
        }
        else
        {
            poseButtonParent.gameObject.SetActive(false);
        }
    }
}
