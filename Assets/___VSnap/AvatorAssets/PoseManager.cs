using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using ObservableCollections;
using TMPro;

public class PoseManager : MonoBehaviour
{
    public ObservableList<AuthorView> authorViews = new ObservableList<AuthorView>();
    public ObservableList<PoseView> poseViews = new ObservableList<PoseView>();
    [SerializeField] private GameObject authorButtonPrefab;
    [SerializeField] private GameObject poseButtonPrefab;
    [SerializeField] private RectTransform authorButtonParent;
    [SerializeField] private RectTransform poseButtonParent;
    [SerializeField] private PoseObject poseObject;

    private void Start()
    {
        ReloadPoseButton();
    }

    public void ReloadPoseButton()
    {
        authorViews.Clear();
        poseViews.Clear();

        foreach (var clipGroup in poseObject.animationClipGroup)
        {
            var author = CreateAuthorButton(clipGroup.name);
            authorViews.Add(author);

            foreach (var clip in clipGroup.animationClip)
            {
                var pose = CreatePoseButton(clipGroup.name, clip.name);
                poseViews.Add(pose);
            }
        }
    }

    private AuthorView CreateAuthorButton(string authorName)
    {
        var authorView = Instantiate(authorButtonPrefab, authorButtonParent).GetComponent<AuthorView>();
        authorView.SetName(authorName);
        return authorView;
    }

    private PoseView CreatePoseButton(string authorName, string poseName)
    {
        var poseView = Instantiate(poseButtonPrefab, poseButtonParent).GetComponent<PoseView>();
        poseView.SetName(authorName, poseName);
        return poseView;
    }
}
