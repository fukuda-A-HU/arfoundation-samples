using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using ObservableCollections;
using TMPro;

public class CharaPoseManager : MonoBehaviour
{
    public ObservableList<AuthorView> authorViews = new ObservableList<AuthorView>();
    public ObservableList<CharaPoseView> poseViews = new ObservableList<CharaPoseView>();
    [SerializeField] private GameObject authorButtonPrefab;
    [SerializeField] private GameObject poseButtonPrefab;
    [SerializeField] private GameObject posePanelPrefab;
    [SerializeField] private RectTransform authorButtonParent;
    [SerializeField] private RectTransform posePanelParent;
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
            var button = Instantiate(authorButtonPrefab, authorButtonParent).GetComponent<Button>();
            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            var authorView = Instantiate(posePanelPrefab, posePanelParent).GetComponent<AuthorView>();
            authorView.SetButton(button, label, clipGroup.name);
            authorViews.Add(authorView);

            foreach (var clip in clipGroup.animationClip)
            {
                // var pose = CreatePoseButton(clipGroup.name, clip.name, posePanel);
                var poseView = Instantiate(poseButtonPrefab, authorView.poseButtonParent).GetComponent<CharaPoseView>();
                poseView.SetName(clipGroup.name, clip.name);
                poseViews.Add(poseView);
            }
        }
    }
}
