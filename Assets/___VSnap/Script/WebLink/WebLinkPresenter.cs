using UnityEngine;

public class WebLinkPresenter : MonoBehaviour
{
    [SerializeField] private WebLink webLink;
    [SerializeField] private WebLinkView[] webLinkView;

    private void Start()
    {
        foreach (var view in webLinkView)
        {
            view.button.onClick.AddListener(() => webLink.OpenLink(view.linkID));
        }
    }
}
