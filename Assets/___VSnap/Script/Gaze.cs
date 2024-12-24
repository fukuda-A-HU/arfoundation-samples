using UnityEngine;
using R3;
using RootMotion.FinalIK;

public class Gaze : MonoBehaviour
{
    private Animator anim;
    private LookAtIK lookAtIK;
    public SerializableReactiveProperty<float> eyeValue;
    public SerializableReactiveProperty<float> headValue;
    public SerializableReactiveProperty<float> bodyValue;
    [SerializeField] Transform lookAtTarget;

    public void SetIK(LookAtIK lookAtIK, Animator anim)
    {
        if (lookAtIK == null || anim == null)
        {
            Debug.Log("Missing component");
            return;
        }

        this.lookAtIK = lookAtIK;
        this.anim = anim;
        this.lookAtIK.solver.eyes = new IKSolverLookAt.LookAtBone[2];

        Transform leftEye = anim.GetBoneTransform(HumanBodyBones.LeftEye);
        Transform rightEye = anim.GetBoneTransform(HumanBodyBones.RightEye);
        Transform head = anim.GetBoneTransform(HumanBodyBones.Head);
        Transform spine = anim.GetBoneTransform(HumanBodyBones.Spine);

        if (leftEye == null || rightEye == null || head == null || spine == null)
        {
            Debug.LogError("Missing bone");
            return;
        }

        this.lookAtIK.solver.eyes[0] = new IKSolverLookAt.LookAtBone(leftEye);
        this.lookAtIK.solver.eyes[1] = new IKSolverLookAt.LookAtBone(rightEye);
        this.lookAtIK.solver.head = new IKSolverLookAt.LookAtBone(head);
        this.lookAtIK.solver.spine = new IKSolverLookAt.LookAtBone[1];
        this.lookAtIK.solver.spine[0] = new IKSolverLookAt.LookAtBone(spine);
        this.lookAtIK.solver.target = lookAtTarget;
        lookAtIK.solver.eyesWeight = 0.8f;

        eyeValue.Subscribe(x =>
        {
            lookAtIK.solver.eyesWeight = x;
        }).AddTo(lookAtIK.gameObject);

        headValue.Subscribe(x =>
        {
            lookAtIK.solver.headWeight = x;
        }).AddTo(lookAtIK.gameObject);

        bodyValue.Subscribe(x =>
        {
            lookAtIK.solver.bodyWeight = x;
        }).AddTo(lookAtIK.gameObject);
    }

    public void SetEyeWeight(float eye)
    {
        this.eyeValue.Value = eye;
    }

    public void SetHeadWeight(float head)
    {
        this.headValue.Value = head;
    }

    public void SetBodyWeight(float body)
    {
        this.bodyValue.Value = body;
    }
}
