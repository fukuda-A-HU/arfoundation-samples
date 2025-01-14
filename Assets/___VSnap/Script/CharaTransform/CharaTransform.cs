using UnityEngine;
using R3;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class CharaTransform : MonoBehaviour
{
    [SerializeField] private Transform CharaRoot;
    [SerializeField] private Transform CharaOffset;
    [SerializeField] private Menu menu;
    [SerializeField] private ARRaycastManager raycastManager;
    public SerializableReactiveProperty<float> height = new SerializableReactiveProperty<float>();
    public SerializableReactiveProperty<float> rotation = new SerializableReactiveProperty<float>();
    public SerializableReactiveProperty<float> scale = new SerializableReactiveProperty<float>();
    [SerializeField] private Vector3 prevPosition;

    public void SetHeight(float value)
    {
        height.Value = value;
    }

    public void SetRotation(float value)
    {
        rotation.Value = value;
    }

    public void SetScale(float value)
    {
        scale.Value = value;
    }

    private void Start()
    {
        height.Subscribe(value =>
        {
            CharaOffset.localPosition = new Vector3(CharaOffset.localPosition.x, value, CharaOffset.localPosition.z);
        }).AddTo(this);

        rotation.Subscribe(value =>
        {
            CharaOffset.rotation = Quaternion.Euler(0, value, 0);
        }).AddTo(this);

        scale.Subscribe(value =>
        {
            CharaOffset.localScale = new Vector3(value, value, value);
        }).AddTo(this);
    }

    const float k_PrefabHalfSize = 0.025f;

    [SerializeField]
    [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
    ARRaycastHitEventAsset m_RaycastHitEvent;


    void OnEnable()
    {
        if (m_RaycastHitEvent != null)
            m_RaycastHitEvent.eventRaised += PlaceObjectAt;
    }

    void OnDisable()
    {
        if (m_RaycastHitEvent != null)
            m_RaycastHitEvent.eventRaised -= PlaceObjectAt;
    }

    void PlaceObjectAt(object sender, ARRaycastHit hitPose)
    {
        PlaceObjectAtAsync(hitPose).Forget();
    }

    async UniTask PlaceObjectAtAsync(ARRaycastHit hitPose)
    {
        // UI上でタッチイベントが発生した場合は処理を中断
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (menu.mode.Value != MenuMode.PlaceChara)
        {
            return;
        }

        // prevPosition = CharaRoot.transform.position;

        // 3フレームまつ
        await UniTask.DelayFrame(3);

        var forward = hitPose.pose.rotation * Vector3.up;       
        var offset = forward * k_PrefabHalfSize;
        CharaRoot.transform.position = hitPose.pose.position + offset;
        CharaRoot.transform.parent = hitPose.trackable.transform.parent;
    }

    public async UniTask SetPrevPosition()
    {
        CharaRoot.transform.DOMove(prevPosition, 0.0001f);
    }

    public void RecordPrevPosition()
    {
        prevPosition = CharaRoot.transform.position;
    }

}