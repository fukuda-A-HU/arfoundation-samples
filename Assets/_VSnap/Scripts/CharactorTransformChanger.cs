using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARFoundation;
using DG.Tweening;
using R3;

public class CharactorTransformChanger : MonoBehaviour
{
    [SerializeField]
    LoadVRM loadVRM;

    [SerializeField]
    private TransformChangeByUI transformChangeByUI;

    private Vector3 defaultPosition = new Vector3(0,0,0);

    [SerializeField]
    [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
    ARRaycastHitEventAsset m_RaycastHitEvent;

    GameObject m_SpawnedObject;

    /// <summary>
    /// The spawned prefab instance.
    /// </summary>
    public GameObject spawnedObject
    {
        get => m_SpawnedObject;
        set => m_SpawnedObject = value;
    }

    void OnEnable()
    {
        if (m_RaycastHitEvent == null)
        {
            Debug.LogWarning($"{nameof(ARPlaceObject)} component on {name} has null inputs and will have no effect in this scene.", this);
            return;
        }

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
        bool isOnUI = IsOnUI();
        if (isOnUI)
        {
            return;
        }

        if (m_SpawnedObject == null)
        {
            Debug.Log("VRMModel Not Loaded");
        }

        else
        {

            m_SpawnedObject.transform.parent = hitPose.trackable.transform.parent;

            defaultPosition = hitPose.pose.position;

            m_SpawnedObject.transform.position = defaultPosition;
        }
    }

    private void Start()
    {
        loadVRM.vrmInstance.Subscribe(x => {

            if (x != null){
                m_SpawnedObject = x.gameObject;
            }
            

        }).AddTo(this);

        transformChangeByUI.offsetPosition.Subscribe(x =>
        {
            if (m_SpawnedObject != null)
            {
                m_SpawnedObject.transform.position = defaultPosition + x;
            }

        }).AddTo(this);

        transformChangeByUI.offsetAngle.Subscribe(x =>
        {
            if (m_SpawnedObject != null)
            {
                m_SpawnedObject.transform.rotation = Quaternion.Euler(x);
            }
        }).AddTo(this);
    }

    public bool IsOnUI()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, result);
        return result.Count > 0;
    }
}
