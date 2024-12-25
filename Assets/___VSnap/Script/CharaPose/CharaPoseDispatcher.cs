using ObservableCollections;
using UnityEngine;
using R3;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CharaPoseDispatcher : MonoBehaviour
{
    // Modelを提供するManager
    [SerializeField] private GameObject authorButtonPrefab;
    [SerializeField] private GameObject poseButtonPrefab;
    [SerializeField] private GameObject posePanelPrefab;
    [SerializeField] private RectTransform authorButtonParent;
    [SerializeField] private RectTransform posePanelParent;

    [SerializeField] private CharaPose pose;

    // PlayerのPresenter
    [SerializeField] private CharaPosePresenter posePresenter;

    private void Start()
    {
        pose.poseDictionary.ObserveAdd().Subscribe(x => 
        {
            var button = Instantiate(authorButtonPrefab, authorButtonParent).GetComponent<Button>();
            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            var authorView = Instantiate(posePanelPrefab, posePanelParent).GetComponent<AuthorView>();
            authorView.SetButton(button, label, x.Value.Key);

            DispatchAuthor(authorView);

            x.Value.Value.button = button;
            x.Value.Value.panel = authorView.gameObject.GetComponent<RectTransform>();
            x.Value.Value.poseButtonParent = authorView.poseButtonParent;

            foreach (var poseInfo in x.Value.Value.poseInfo)
            {
                var poseView = Instantiate(poseButtonPrefab, authorView.poseButtonParent).GetComponent<CharaPoseView>();
                poseView.SetName(x.Value.Key, poseInfo.Key);

                DispatchPose(poseView);

                poseInfo.Value.button = poseView.button;
            }

        }).AddTo(this);
    }

    private void DispatchPose(CharaPoseView poseView)
    {
       posePresenter.OnCreatePoseView(pose, poseView);
    }

    private void DispatchAuthor(AuthorView authorView)
    {
        posePresenter.OnCreateAuthorView(pose, authorView);
    }
}
