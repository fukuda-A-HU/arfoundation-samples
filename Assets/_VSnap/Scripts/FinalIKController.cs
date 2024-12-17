using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.UI;
using Unity.Collections;
using R3;

public class FinalIKController : MonoBehaviour
{
    [SerializeField]
    LoadVRM loadVRM;
    private LookAtIK lookAtIK;

    private Animator anim;

    [SerializeField] Transform lookAtTarget;

    [SerializeField] Slider bodySlider;

    [SerializeField] Slider headSlider;

    [SerializeField] Slider eyeSlider;

    void Start(){
        loadVRM.vrmInstance.Subscribe(x => {

            if (x != null){

                lookAtIK = x.gameObject.AddComponent<LookAtIK>();

                anim = x.gameObject.GetComponent<Animator>();

                if (lookAtIK != null  && anim != null){

                    lookAtIK.solver.eyes = new IKSolverLookAt.LookAtBone[2];

                    lookAtIK.solver.eyes[0] = new IKSolverLookAt.LookAtBone(anim.GetBoneTransform(HumanBodyBones.LeftEye));

                    lookAtIK.solver.eyes[1] = new IKSolverLookAt.LookAtBone(anim.GetBoneTransform(HumanBodyBones.RightEye));

                    lookAtIK.solver.head = new IKSolverLookAt.LookAtBone(anim.GetBoneTransform(HumanBodyBones.Head));

                    lookAtIK.solver.spine = new IKSolverLookAt.LookAtBone[1];

                    lookAtIK.solver.spine[0] = new IKSolverLookAt.LookAtBone(anim.GetBoneTransform(HumanBodyBones.Spine));

                    lookAtIK.solver.target = lookAtTarget;

                    lookAtIK.solver.eyesWeight = 0.5f;

                }
                else{
                    Debug.Log("Can't find animator");
                }


            }

        }).AddTo(this);
    }

    public void BodyWeightChange()
    {
        if (lookAtIK != null)
        {
            lookAtIK.solver.bodyWeight = bodySlider.value;
        }
        
    }

    public void HeadWeightChange()
    {
        if (lookAtIK != null)
        {
            lookAtIK.solver.headWeight = headSlider.value;
        }
    }

    public void EyeWeightChange()
    {
        if (lookAtIK != null)
        {
            lookAtIK.solver.eyesWeight = eyeSlider.value;
        }
    }

    public void ResetWeight()
    {
        if (lookAtIK != null)
        {
            lookAtIK.solver.bodyWeight = 0;
            lookAtIK.solver.headWeight = 0;
            lookAtIK.solver.eyesWeight = 0;

            bodySlider.value = 0;
            headSlider.value = 0;
            eyeSlider.value = 0;

        }
    }
}
