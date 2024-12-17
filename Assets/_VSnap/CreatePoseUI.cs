using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;
using R3;

public class CreatePoseUI : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] LoadVRM loadVRM;

    [Header("To create pose UI, Make sure no elements are in the root objects and PoseButtons List")]
    [SerializeField]
    private PoseObject poseObject;

    [SerializeField]
    private GameObject categoryButtonRoot;

    [SerializeField]
    private GameObject poseScrollViewRoot;

    [SerializeField]
    private GameObject categoryButtonPrefab;

    [SerializeField]
    private GameObject poseScrollViewPrefab;

    [SerializeField]
    private GameObject poseButtonPrefab;

    private List<GameObject> poseScrollViews = new List<GameObject>();

    [SerializeField]
    private List<GameObject> poseButtons;

    [ContextMenu("CreatePoseUI")]
    public void Create()
    {
        poseScrollViews.Clear();

        foreach (Transform n in categoryButtonRoot.transform)
        {
            var obj = n.gameObject;
            Destroy(obj);
        }

        foreach (Transform n in poseScrollViewRoot.transform)
        {
            var obj = n.gameObject;
            Destroy(obj);
        }

        for (int i = 0; i < poseObject.animationClipGroup.Length; i++)
        {
            var categoryButtonInstance = Instantiate(categoryButtonPrefab, categoryButtonRoot.transform);

            var poseScrollViewInstance = Instantiate(poseScrollViewPrefab, poseScrollViewRoot.transform);

            poseScrollViews.Add(poseScrollViewInstance);

            poseScrollViewInstance.SetActive(false);

            categoryButtonInstance.name = poseObject.animationClipGroup[i].name;

            poseScrollViewInstance.name = poseObject.animationClipGroup[i].name;

            categoryButtonInstance.GetComponent<SelectCategory>().scrollView = poseScrollViewInstance;

            categoryButtonInstance.GetComponent<HoldObject>().gameObject[1].GetComponent<TextMeshProUGUI>().text = poseObject.animationClipGroup[i].name;

            for (int j = 0; j < poseObject.animationClipGroup[i].animationClip.Length; j++)
            {
                var poseButtonInstance = Instantiate(poseButtonPrefab, poseScrollViewInstance.GetComponent<HoldObject>().gameObject[0].transform);

                poseButtonInstance.name = poseObject.animationClipGroup[i].animationClip[j].name;

                poseButtonInstance.GetComponent<HoldObject>().gameObject[1].GetComponent<TextMeshProUGUI>().text = poseObject.animationClipGroup[i].animationClip[j].name;

                poseButtonInstance.GetComponent<AnimationParameterController>().superIndex = i;

                poseButtonInstance.GetComponent<AnimationParameterController>().index = j;

                poseButtons.Add(poseButtonInstance);
            }

        }
    }

    void Start(){

        loadVRM.vrmInstance.Subscribe(x =>{

            var animator = x.gameObject.GetComponent<Animator>();

            foreach (var button in poseButtons)
            {
                button.GetComponent<AnimationParameterController>().animator = animator;
            }

        }).AddTo(this);
    }
}
