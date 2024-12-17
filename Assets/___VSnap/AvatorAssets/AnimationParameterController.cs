using UnityEngine;
using UnityEngine.Animations;
using Cysharp.Threading.Tasks;
public class AnimationParameterController : MonoBehaviour
{
    public int superIndex;

    public int index;

    public Animator animator;

    public void SetIndex()
    {
        SetIndexAsync();
    }

    async UniTask SetIndexAsync()
    {
        animator.enabled = true;

        animator.SetInteger("Index", index);
        animator.SetInteger("SuperIndex", superIndex);

        await UniTask.Yield();
        await UniTask.WaitUntil(() => !animator.IsInTransition(0));

        await UniTask.Yield();

        animator.enabled = false;
    }
}
