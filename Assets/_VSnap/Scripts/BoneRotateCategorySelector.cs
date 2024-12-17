using UnityEngine;
using R3;

public class BoneRotateCategorySelector : MonoBehaviour
{
    [SerializeField] GameObject middleView;

    [SerializeField] GameObject rightView;

    [SerializeField] GameObject leftView;

    public enum Side
    {
        middle,right,left
    }

    SerializableReactiveProperty<Side> side = new SerializableReactiveProperty<Side>();

    private void Start()
    {
        side.Subscribe(x =>
        {
            if (x == Side.middle)
            {
                middleView.SetActive(true);

                rightView.SetActive(false);

                leftView.SetActive(false);
            }
            else if (x == Side.right)
            {
                middleView.SetActive(false);

                rightView.SetActive(true);

                leftView.SetActive(false);
            }
            else if (x == Side.left)
            {
                middleView.SetActive(false);

                rightView.SetActive(true);

                leftView.SetActive(false);
            }
        }).AddTo(this);

        side.Value = Side.middle;
    }

    public void SetSideMiddle()
    {
        side.Value = Side.middle;
    }

    public void SetSideRight()
    {
        side.Value = Side.right;
    }

    public void SetSideLeft()
    {
        side.Value = Side.left;
    }
}
