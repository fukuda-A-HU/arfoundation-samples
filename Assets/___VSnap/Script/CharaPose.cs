using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;

public class CharaPose : MonoBehaviour
{
    public SerializableReactiveProperty<PoseInfo> poseInfo = new SerializableReactiveProperty<PoseInfo>();
    [SerializeField] PoseObject poseObject;
    public SerializableReactiveProperty<Animator> animator = new SerializableReactiveProperty<Animator>();
    [SerializeField] private RuntimeAnimatorController poseController;
    public SerializableReactiveProperty<string> showingAuthorName = new SerializableReactiveProperty<string>();

    // =========
    public ObservableDictionary<string, ObservableDictionary<string, int[]>> poseIndex = new ObservableDictionary<string, ObservableDictionary<string, int[]>>();


    private void Start()
    {
        poseInfo = new SerializableReactiveProperty<PoseInfo>();

        poseInfo.Subscribe(x => {
            if (x == null || x.authorName == null || x.poseName == null) return;
            SetPoseAsyncByName(x.authorName, x.poseName).Forget();
        }).AddTo(this);
    }

    public void SetPose(PoseInfo poseInfo)
    {
        this.poseInfo.Value = poseInfo;
    }

    public void SetAnimator(Animator _animator)
    {
        _animator.runtimeAnimatorController = poseController;
        animator.Value = _animator;
    }

    public void SetShowingAuthorName(string authorName)
    {
        showingAuthorName.Value = authorName;
    }

    private async UniTask SetPoseAsyncByName(string authorName, string poseName)
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

        Debug.Log("superIndex: " + superIndex + " poseIndex: " + poseIndex);

        if (animator != null)
        {
            animator.Value.enabled = true;

            animator.Value.SetInteger("Index", poseIndex);
            animator.Value.SetInteger("SuperIndex", superIndex);

            await UniTask.Yield();
            await UniTask.WaitUntil(() => !animator.Value.IsInTransition(0));

            await UniTask.Yield();

            animator.Value.enabled = false;
        }
        else
        {
            Debug.LogError("Animatorがアタッチされていません");
        }
    }

    private async UniTask SetPoseAsync(int superIndex, int poseIndex)
    {
        if (animator != null)
        {
            animator.Value.enabled = true;

            animator.Value.SetInteger("Index", poseIndex);
            animator.Value.SetInteger("SuperIndex", superIndex);

            await UniTask.Yield();
            await UniTask.WaitUntil(() => !animator.Value.IsInTransition(0));

            await UniTask.Yield();

            animator.Value.enabled = false;
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
