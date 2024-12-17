using UnityEngine;
using R3;

public class BoneRotationPresenter : MonoBehaviour
{
    public HumanBodyBones bone;

    [SerializeField]
    BoneRotationModel boneRotationModel;

    private Vector3 eular;

    public void SetBone()
    {
        boneRotationModel.bone.Value = bone;
    }
}
