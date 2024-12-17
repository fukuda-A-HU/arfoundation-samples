using UnityEngine;

public class CameraLayerController : MonoBehaviour
{
    public void EnableLayer(Camera cam, int layer)
    {
        cam.cullingMask |= (1 << layer);
    }

    public void DisableLayer(Camera cam, int layer)
    {
        cam.cullingMask &= ~(1 << layer);
    }

    public void EnableAllLayers(Camera cam)
    {
        cam.cullingMask = ~0; // 全てのビットを1にセット（全てのレイヤーを有効化）
    }

    public void DisableAllLayers(Camera cam)
    {
        cam.cullingMask = 0; // 全てのビットを0にセット（全てのレイヤーを無効化）
    }

}
