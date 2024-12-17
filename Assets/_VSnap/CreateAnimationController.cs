using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif


public class CreateAnimationController : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    AnimatorController animatorController;

    [SerializeField]
    PoseObject poseObject;

    AnimatorStateMachine[] stateMachines;

    [ContextMenu("PoseAnimatorController????????")]
    public void CreatePoseController()
    {
        // animatorController.AddParameter("Index", AnimatorControllerParameterType.Int);

        var rootStateMachine = animatorController.layers[0].stateMachine;

        while (rootStateMachine.states.Length > 0)
        {
            rootStateMachine.RemoveState(rootStateMachine.states[0].state);
        }

        for (int i = 0; i < poseObject.animationClipGroup.Length; i++)
        {

            for (int j = 0; j < poseObject.animationClipGroup[i].animationClip.Length ; j++)
            {
                var state = rootStateMachine.AddState(poseObject.animationClipGroup[i].animationClip[j].name);

                state.motion = poseObject.animationClipGroup[i].animationClip[j];

                var transitionEntry = rootStateMachine.AddEntryTransition(state);

                transitionEntry.AddCondition(AnimatorConditionMode.Equals, i, "SuperIndex");
                transitionEntry.AddCondition(AnimatorConditionMode.Equals, j, "Index");

                var transitionExit1 = state.AddExitTransition();
                var transitionExit2 = state.AddExitTransition();

                transitionExit1.AddCondition(AnimatorConditionMode.NotEqual, i, "SuperIndex");
                transitionExit2.AddCondition(AnimatorConditionMode.NotEqual, j, "Index");
            }

        }
    }
#endif
}