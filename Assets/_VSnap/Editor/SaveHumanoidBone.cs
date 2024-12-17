
using UnityEngine;
using UnityEditor;

public class SaveHumanoidBone : MonoBehaviour
{
    public Animator animator; // �A�j���[�^�[�R���|�[�l���g
    public string animationClipName = "NewAnimationClip"; // �ۑ�����A�j���[�V�����̖��O

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            return;
        }

        AnimationClip clip = new AnimationClip();
        clip.name = animationClipName;

        // �A�j���[�V�����̋L�^
        RecordBoneTransform(animator, HumanBodyBones.Hips, clip);

        // �A�Z�b�g�Ƃ��ĕۑ�
        AssetDatabase.CreateAsset(clip, "Assets/" + animationClipName + ".anim");
        AssetDatabase.SaveAssets();
    }

    void RecordBoneTransform(Animator animator, HumanBodyBones bone, AnimationClip clip)
    {
        Transform boneTransform = animator.GetBoneTransform(bone);
        if (boneTransform == null)
        {
            Debug.LogWarning(bone + " not found.");
            return;
        }

        string bonePath = AnimationUtility.CalculateTransformPath(boneTransform, animator.transform);
        AnimationCurve curveX = AnimationCurve.Linear(0, boneTransform.localPosition.x, 1, boneTransform.localPosition.x);
        AnimationCurve curveY = AnimationCurve.Linear(0, boneTransform.localPosition.y, 1, boneTransform.localPosition.y);
        AnimationCurve curveZ = AnimationCurve.Linear(0, boneTransform.localPosition.z, 1, boneTransform.localPosition.z);

        clip.SetCurve(bonePath, typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve(bonePath, typeof(Transform), "localPosition.y", curveY);
        clip.SetCurve(bonePath, typeof(Transform), "localPosition.z", curveZ);
    }
}
