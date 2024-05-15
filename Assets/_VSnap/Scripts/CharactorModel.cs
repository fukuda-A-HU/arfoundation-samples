using UnityEngine;
using R3;

public class CharactorModel : MonoBehaviour
{
    public ReactiveProperty<float> rotate = new ReactiveProperty<float>();
    public ReactiveProperty<float> scale = new ReactiveProperty<float>();
    public ReactiveProperty<int> poseIndex = new ReactiveProperty<int>();
    [SerializeField] int maxPoseIndex = 3;

    private void Start()
    {
        poseIndex.Subscribe(x => Debug.Log(x));
    }

    public void SetRotate(float value)
    {
        rotate.Value = value;
    }

    public void SetScale(float value)
    {
        scale.Value = value;
    }

    public void PoseIndexUp()
    {
        if (poseIndex.Value < maxPoseIndex)
        {
            poseIndex.Value += 1;
        }
        else
        {
            poseIndex.Value = 0;
        }
    }

    public void PoseIndexDown()
    {
        if (poseIndex.Value > 0)
        {
            poseIndex.Value -= 1;
        }
        else
        {
            poseIndex.Value = 3;
        }
    }
}
