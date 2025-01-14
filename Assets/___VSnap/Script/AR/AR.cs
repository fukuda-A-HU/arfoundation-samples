using UnityEngine;
using UnityEngine.XR.ARFoundation;
using R3;

public class AR : MonoBehaviour
{
    [SerializeField] private ARSession session;
    [SerializeField] private ARPointCloudManager pointCloudManager;
    [SerializeField] private Transform trackablesRoot;
    public SerializableReactiveProperty<bool> isOn = new SerializableReactiveProperty<bool>();
    
    public void Reset()
    {
        session.Reset();
    }

    private void Start()
    {
        isOn.Subscribe(x =>
        {
            if (x)
            {
                pointCloudManager.enabled = true;
            }
            else
            {
                pointCloudManager.enabled = false;
                var arClouds = trackablesRoot.GetComponentsInChildren<ARPointCloud>();
                foreach (var arCloud in arClouds)
                {
                    Destroy(arCloud.gameObject);
                }
            }
        });
    }

    public void SetActive(bool value)
    {
        Debug.Log("ARSession SetActive: " + value);
        session.gameObject.SetActive(value);
    }

    public void isOnChange()
    {
        isOn.Value = !isOn.Value;
        
    }
}
