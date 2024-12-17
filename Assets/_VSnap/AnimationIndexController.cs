using UnityEngine;

public class AnimationIndexController : MonoBehaviour
{
    private Animator animator;
    private CreateAnimationController createAnimation;
    [SerializeField] PoseObject poseObejct;
    private int index = 0;
    private int superIndex = 0;

    void Start()
    {
        createAnimation = GetComponent<CreateAnimationController>();
        // Animator?R???|?[?l???g??????
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ?????L?[?????????`?F?b?N
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            index++;
            CheckMaxIndex();
            animator.SetInteger("Index", index);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            index--;
            CheckMaxIndex();
            animator.SetInteger("Index", index);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            superIndex++;
            CheckMaxIndex();
            animator.SetInteger("SuperIndex", superIndex);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            superIndex--;
            CheckMaxIndex();
            animator.SetInteger("SuperIndex", superIndex);
        }
    }

    private void CheckMaxIndex()
    {
        if (superIndex < 0)
        {
            superIndex = 0;
        }
        if (superIndex > poseObejct.animationClipGroup.Length - 1)
        {
            superIndex = poseObejct.animationClipGroup.Length - 1;
        }

        if (index < 0)
        {
            index = 0;
        }
        if (index > poseObejct.animationClipGroup[superIndex].animationClip.Length - 1)
        {
            index = poseObejct.animationClipGroup[superIndex].animationClip.Length - 1;
        }
    }
}
