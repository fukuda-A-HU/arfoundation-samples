using UnityEngine;
using ObservableCollections;
using Cysharp.Threading.Tasks;

public class CharaPoseManager : MonoBehaviour
{
    [SerializeField] private PoseObject poseObject;
    [SerializeField] private CharaPose pose;

    private void Start()
    {
        ReloadPoseDelay().Forget();
    }

    private async UniTask ReloadPoseDelay()
    {
        await UniTask.Yield();
        ReloadPoseButton();
    }
    
    public void ReloadPoseButton()
    {
        pose.poseDictionary.Clear();

        foreach (var clipGroup in poseObject.animationClipGroup)
        {
            Debug.Log("Add PoseGroup: " + clipGroup.name);

            // clipGroup.nameがすでにposeDictionaryに存在する場合はスキップ
            if (pose.poseDictionary.ContainsKey(clipGroup.name))
            {
                Debug.LogWarning("PoseGroup: " + clipGroup.name + " is already exist.");
                continue;
            }

            var poseGroup = new PoseGroupInfo();

            foreach (var clip in clipGroup.animationClip)
            {
                Debug.Log("Add Pose: " + clip.name);

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
