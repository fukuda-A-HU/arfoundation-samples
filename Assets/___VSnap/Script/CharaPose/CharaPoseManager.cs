using UnityEngine;
using ObservableCollections;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using R3;

public class CharaPoseManager : MonoBehaviour
{
    [SerializeField] private CharaPose pose;
    public SerializableReactiveProperty<string> message = new SerializableReactiveProperty<string>();
    public SerializableReactiveProperty<PoseObject> poseObject = new SerializableReactiveProperty<PoseObject>();

    public bool isInitialized
    {
        get;
        private set;
    } = false;

    private void Start()
    {
        poseObject.Subscribe(async x =>
        {
            if (x != null)
            {
                await ReloadPose(x);
            }
        }).AddTo(this);

        isInitialized = true;
    }
    
    public async UniTask ReloadPose(PoseObject _poseObject)
    {
        await UniTask.Yield();

        pose.poseDictionary.Clear();

        if (_poseObject == null)
        {
            Debug.LogError("PoseObject is null");
            return;
        }

        foreach (var clipGroup in _poseObject.animationClipGroup)
        {
            if (pose.poseDictionary.ContainsKey(clipGroup.name))
            {
                Debug.LogWarning("PoseGroup: " + clipGroup.name + " is already exist.");
                continue;
            }

            var poseGroup = new PoseGroupInfo();

            foreach (var clip in clipGroup.animationClip)
            {
                // clip.nameがすでにposeGroupに存在する場合はスキップ
                if (poseGroup.poseInfo.ContainsKey(clip.name))
                {
                    Debug.LogWarning("Pose: " + clip.name + " is already exist.");
                    continue;
                }
                var pose = new PoseInfo();
                pose.clip = clip;
                poseGroup.poseInfo.Add(clip.name, pose);
            }

            pose.poseDictionary.Add(clipGroup.name, poseGroup);
        }
        pose.selectedAuthor.Value = null;
    }
}
