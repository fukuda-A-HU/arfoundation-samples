using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using UnityEngine.UI;

public class CharaPose : MonoBehaviour
{
    public ReactiveProperty<PoseNameInfo> poseNameInfo = new ReactiveProperty<PoseNameInfo>();
    public SerializableReactiveProperty<Animator> animator = new SerializableReactiveProperty<Animator>();
    [SerializeField] private RuntimeAnimatorController poseController;

    public ObservableDictionary<string, PoseGroupInfo> poseDictionary = new ObservableDictionary<string, PoseGroupInfo>();

    public SerializableReactiveProperty<string> selectedAuthor = new SerializableReactiveProperty<string>();

    private void Start()
    {
        poseNameInfo.Subscribe(x => {
            if (x == null || x.authorName == null || x.poseName == null) return;
            SetPoseAsyncByName(x.authorName, x.poseName).Forget();
        }).AddTo(this);

        selectedAuthor.Subscribe(x => {
            
            // dictionaryの全てのpanelがinstanceされていない場合はreturn
            if (poseDictionary.Any(pose => pose.Value.panel == null))
            {
                return;
            }

            if (x == null)
            {
                // dictionaryの最初の要素のみを表示
                var firstAuthor = poseDictionary.First();
                firstAuthor.Value.poseButtonParent.gameObject.SetActive(true);
                // それ以外は非表示
                foreach (var author in poseDictionary)
                {
                    if (author.Key != firstAuthor.Key)
                    {
                        author.Value.poseButtonParent.gameObject.SetActive(false);
                    }
                }
                return;
            }
            foreach (var author in poseDictionary)
            {
                if (author.Key != x)
                {
                    author.Value.poseButtonParent.gameObject.SetActive(false);
                }
                else
                {
                    author.Value.poseButtonParent.gameObject.SetActive(true);
                }
            }
        }).AddTo(this);
    }

    public void SetPose(PoseNameInfo _poseNameInfo)
    {
        poseNameInfo.Value = _poseNameInfo;
    }

    public void SetAnimator(Animator _animator)
    {
        _animator.runtimeAnimatorController = poseController;
        animator.Value = _animator;
    }

    public void SetAuthor(string authorName)
    {
        selectedAuthor.Value = authorName;
    }

    private async UniTask SetPoseAsyncByName(string authorName, string poseName)
    {
        var clip = poseDictionary[authorName].poseInfo[poseName].clip;
        Debug.Log("SetPoseAsyncByName: " + authorName + " " + poseName + " " + clip.name);

        if (animator != null)
        {
            animator.Value.enabled = true;
            
            //animatoroverridecontrollerを生成
            var overrideController = new AnimatorOverrideController(poseController);

            // animationをclipに置き換え
            overrideController["000_0000_Idle"] = clip;

            // 元あるコントローラーをoverridecontrollerに設定
            animator.Value.runtimeAnimatorController = overrideController;
            
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

public class PoseNameInfo
{
    public string authorName;
    public string poseName;
}

public class PoseGroupInfo
{
    public Button button;
    public RectTransform panel;
    public RectTransform poseButtonParent;
    public ObservableDictionary<string, PoseInfo> poseInfo = new ObservableDictionary<string, PoseInfo>();
}
public class PoseInfo
{
    public Button button;
    public AnimationClip clip;
}