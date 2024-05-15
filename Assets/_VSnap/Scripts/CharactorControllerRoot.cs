using UnityEngine;
using R3;

public class CharactorControllerRoot : MonoBehaviour
{
    [SerializeField] CharactorModel model;
    [SerializeField] RuntimeAnimatorController animationController;

    private GameObject obj = null;
    private Animator anim = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        model.rotate.Subscribe(x =>
        {
            if(obj != null)
            {
                obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.x, x, obj.transform.rotation.z);
            };
        });

        model.scale.Subscribe(x =>
        {
            if (obj != null)
            {
                obj.transform.localScale = new Vector3(x, x, x);
            }
        });

        model.poseIndex.Subscribe(x =>
        {
            anim.SetInteger("PoseIndex", x);
        });
    }

    // Update is called once per frame
    void Update()
    {
        FindAvatorRoot();
    }

    void FindAvatorRoot()
    {
        if (obj != null)
        {
            return;
        }
        else
        {
            obj = FindFirstObjectByType<Animator>().gameObject;
            if (obj != null)
            {
                anim = obj.GetComponent<Animator>();
                obj.AddComponent<CharactorController>();
                anim.runtimeAnimatorController = animationController;
            }
        }
    }
}
