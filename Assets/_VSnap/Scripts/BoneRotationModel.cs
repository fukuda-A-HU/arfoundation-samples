using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using R3;
using UnityEngine.AI;

public class BoneRotationModel : MonoBehaviour
{
    [SerializeField]
    private LoadVRM loadVRM;
    private Animator anim;

    public SerializableReactiveProperty<HumanBodyBones> bone = new SerializableReactiveProperty<HumanBodyBones>();

    [SerializeField]
    private Slider xSlider;

    [SerializeField]
    private Slider ySlider;

    [SerializeField]
    private Slider zSlider;

    private  Quaternion r;

    private Quaternion rDelta;

    private void Start()
    {
        loadVRM.vrmInstance.Subscribe(x => {

            if(x != null){

                anim = x.gameObject.GetComponent<Animator>();

            }
        }).AddTo(this);

        bone.Subscribe(x =>
        {
            if (anim != null)
            {
                r = anim.GetBoneTransform(x).rotation;

                xSlider.value = 0;
                ySlider.value = 0;
                zSlider.value = 0;
            }
        }).AddTo(this);
    }

    public void RotateChange()
    {
        if (anim != null)
        {
            rDelta = Quaternion.Euler(new Vector3(xSlider.value, ySlider.value, zSlider.value));

            anim.GetBoneTransform(bone.Value).rotation = r * rDelta;
        }
    }

    public void ResetAngle()
    {
        anim.GetBoneTransform(bone.Value).rotation = r;

        xSlider.value = 0;
        ySlider.value = 0;
        zSlider.value = 0;
    }

}
