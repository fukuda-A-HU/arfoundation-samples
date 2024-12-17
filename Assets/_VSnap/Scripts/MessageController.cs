using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI tmpro;

    public string Text
    {
        get
        {
            return tmpro.text;
        }

        set
        {
            tmpro.text = value;
        }
    }

}
