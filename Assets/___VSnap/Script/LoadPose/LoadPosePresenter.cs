using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;

public class LoadPosePresenter : MonoBehaviour
{
    [SerializeField] private LoadPoseView loadPoseView;
    [SerializeField] private LoadPose loadPose;
    [SerializeField] private CharaPoseManager charaPoseManager;

    private async UniTask Start()
    {
        loadPoseView.loadAssetButton.onClick.AddListener(() =>
        {
            loadPose.LoadPoseFromBundle();
        });

        loadPose.poseObject.Subscribe(x =>
        {
            if (x != null)
            {
            charaPoseManager.poseObject.Value = x;
            }
        }).AddTo(this);

        await UniTask.WaitUntil(() => charaPoseManager.isInitialized);
        await UniTask.WaitUntil(() => loadPose.isInitialized);

        Debug.Log("LoadPosePresenter is initialized");

        loadPose.LoadDefaultPose();
    }
}
