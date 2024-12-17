using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class Pose : MonoBehaviour
{
    public SerializableReactiveProperty<PoseInfo> poseInfo;
    [SerializeField] PoseObject poseObject;
    [SerializeField] Animator animator;

    [SerializeField] private string authorName;
    [SerializeField] private string poseName;

    private void Start()
    {
        poseInfo = new SerializableReactiveProperty<PoseInfo>();

        poseInfo.Subscribe(x => {
            SetPoseAsync(poseInfo.Value.authorName, poseInfo.Value.poseName).Forget();
        }).AddTo(this);
    }

    public void SetPose(PoseInfo poseInfo)
    {
        this.poseInfo.Value = poseInfo;
        authorName = poseInfo.authorName;
        poseName = poseInfo.poseName;
    }

    private async UniTask SetPoseAsync(string authorName, string poseName)
    {
        // poseObject.animationClipGroup.name == author　となるもののindexを取得
        int superIndex = 0;
        int poseIndex = 0;

        for (int i = 0; i < poseObject.animationClipGroup.Length; i++)
        {
            if (poseObject.animationClipGroup[i].name == authorName)
            {
                superIndex = i;
                break;
            }
        }

        for (int j = 0; j < poseObject.animationClipGroup[superIndex].animationClip.Length; j++)
        {
            if (poseObject.animationClipGroup[superIndex].animationClip[j].name == poseName)
            {
                poseIndex = j;
                break;
            }
        }

        if (animator != null)
        {
            animator.enabled = true;

            animator.SetInteger("Index", poseIndex);
            animator.SetInteger("SuperIndex", superIndex);

            await UniTask.Yield();
            await UniTask.WaitUntil(() => !animator.IsInTransition(0));

            await UniTask.Yield();

            animator.enabled = false;
        }
        else
        {
            Debug.LogError("Animatorがアタッチされていません");
        }
    }
}

public class PoseInfo
{
    public string authorName;
    public string poseName;
}
