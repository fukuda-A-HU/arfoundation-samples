using UnityEngine;
using R3;
using UnityEngine.XR.ARFoundation;

public class PlaneVisualizeController : MonoBehaviour
{
    // [SerializeField] private ARPlaneMeshVisualizer meshVisualizer;

    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Material spetialPlaneMat;

    [SerializeField] private Material shadowPlane;

    private AR ar;

    private Color c;

    private void Start()
    {
        ar = FindFirstObjectByType<AR>();

        ar.isOn.Subscribe(x =>
        {
            if (x == true){
                meshRenderer.material = spetialPlaneMat;

                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
            }
            else{
                meshRenderer.material = shadowPlane;

                lineRenderer.startWidth = 0;
                lineRenderer.endWidth = 0;
            }

        }).AddTo(this);
    }
}
