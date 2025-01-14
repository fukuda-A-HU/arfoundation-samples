
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPLink : MonoBehaviour,IPointerClickHandler
{
    private TextMeshProUGUI tmpText = default;

    private void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, eventData.position, eventData.pressEventCamera);
        if(linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}