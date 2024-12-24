using UnityEngine;
using UnityEngine.XR.ARFoundation;
using R3;

public class AR : MonoBehaviour
{
    [SerializeField] private ARSession session;
    public SerializableReactiveProperty<bool> isOn = new SerializableReactiveProperty<bool>();
    
    public void Reset()
    {
        session.Reset();
    }

    public void SetActive(bool value)
    {
        session.gameObject.SetActive(value);
    }
}
