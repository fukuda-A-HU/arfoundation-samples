using UnityEngine;
using R3;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.EventSystems;

public class CharaTransform : MonoBehaviour
{
    [SerializeField] private Transform CharaRoot;
    [SerializeField] private Transform CharaOffset;
    [SerializeField] private ARRaycastManager raycastManager;
    public SerializableReactiveProperty<bool> isTranformable = new SerializableReactiveProperty<bool>();
    public SerializableReactiveProperty<float> height = new SerializableReactiveProperty<float>();
    public SerializableReactiveProperty<float> rotation = new SerializableReactiveProperty<float>();
    public SerializableReactiveProperty<float> scale = new SerializableReactiveProperty<float>();

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
        // UI上でタッチイベントが発生した場合は処理を中断
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (isTranformable.Value == false)
        {
            return;
        }

        var forward = hitPose.pose.rotation * Vector3.up;
        var offset = forward * k_PrefabHalfSize;
        CharaRoot.transform.position = hitPose.pose.position + offset;
        CharaRoot.transform.parent = hitPose.trackable.transform.parent;
    }

}
