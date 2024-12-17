using ObservableCollections;
using UnityEngine;
using R3;

public class PoseDispatcher : MonoBehaviour
{
    // Modelを提供するManager
    [SerializeField] private PoseManager poseManager;

    [SerializeField] private Pose pose;

    // PlayerのPresenter
    [SerializeField] private PosePresenter posePresenter;
    [SerializeField] private Menu menu;

    private void Start()
    {
        // 今リストにあるやつをDispatch
        foreach (var p in poseManager.poseViews)
        {
            DispatchPose(p);
        }

        poseManager.poseViews.ObserveAdd().Subscribe(p => DispatchPose(p.Value));

        foreach (var a in poseManager.authorViews)
        {
            DispatchAuthor(a);
        }

        poseManager.authorViews.ObserveAdd().Subscribe(a => DispatchAuthor(a.Value));
    }

    private void DispatchPose(PoseView poseView)
    {
       posePresenter.OnCreatePoseView(pose, poseView);
    }

    private void DispatchAuthor(AuthorView authorView)
    {
        posePresenter.OnCreateAuthorView(menu, authorView);
    }
}
