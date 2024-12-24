using UnityEngine;
using R3;

public class AuthorPresenter : MonoBehaviour
{
    [SerializeField] private AuthorView authorView;
    [SerializeField] private CharaPose pose;

    private void Start()
    {
        authorView.button.onClick.AddListener(() =>
        {
           pose.SetShowingAuthorName(authorView.authorName);
        });

        pose.showingAuthorName.Subscribe(value =>
        {
            authorView.SetShowingAuthorName(value);
        }).AddTo(this);
    }
}
