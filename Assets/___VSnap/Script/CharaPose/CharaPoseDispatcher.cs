using ObservableCollections;
using UnityEngine;
using R3;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
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
        pose.poseDictionary.ObserveAdd().Subscribe(async x => 
        {
            
            var buttonPrefab = await InstantiateAsync(authorButtonPrefab, authorButtonParent);
            var button = buttonPrefab[0].GetComponent<Button>();
            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            var posePanel = await InstantiateAsync(posePanelPrefab, posePanelParent);
            // localのanchorを設定
            posePanel[0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            var authorView = posePanel[0].GetComponent<AuthorView>();
            authorView.SetButton(button, label, x.Value.Key);

            DispatchAuthor(authorView);

            x.Value.Value.button = button;
            x.Value.Value.panel = authorView.gameObject.GetComponent<RectTransform>();
            x.Value.Value.poseButtonParent = authorView.poseButtonParent;

            foreach (var poseInfo in x.Value.Value.poseInfo)
            {
                var poseViewPrefab = await InstantiateAsync(poseButtonPrefab, authorView.poseButtonParent);
                var poseView = poseViewPrefab[0].GetComponent<CharaPoseView>();
                poseView.SetName(x.Value.Key, poseInfo.Key);

                DispatchPose(poseView);

                poseInfo.Value.button = poseView.button;
            }

            if (pose.poseDictionary.Count == 1)
            {
                pose.selectedAuthor.Value = pose.poseDictionary.First().Key;
            }

        }).AddTo(this);

        pose.poseDictionary.ObserveRemove().Subscribe(x =>
        {
            Destroy(x.Value.Value.panel.gameObject);
            Destroy(x.Value.Value.button.gameObject);
        }).AddTo(this);

        pose.poseDictionary.ObserveClear().Subscribe(x =>
        {
            //自分を含めない、自分の子オブジェクトを削除
            foreach (Transform n in authorButtonParent)
            {
                if (n.gameObject == authorButtonParent) continue;
                Destroy(n.gameObject);
            }
            foreach (Transform n in posePanelParent)
            {
                if (n.gameObject == posePanelParent) continue;
                Destroy(n.gameObject);
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
