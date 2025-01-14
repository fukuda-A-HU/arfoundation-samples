using UnityEngine;
using System.IO;

public class AssetBundleTest : MonoBehaviour
{
    [SerializeField] private string assetBundleName = "myassetBundle";
    [SerializeField] private string assetName = "MyObject";
    [SerializeField] private PoseObject poseObject;

    void Start() {
        var myLoadedAssetBundle 
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, assetBundleName));
        if (myLoadedAssetBundle == null) {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        poseObject = myLoadedAssetBundle.LoadAsset<PoseObject>(assetName);
        if (poseObject == null) {
            Debug.Log("Failed to load PoseObject!");
            return;
        }
        else {
            Debug.Log("PoseObject loaded successfully!");
            // アセットバンドル内の名前を表示
            Debug.Log(poseObject.name);
        }
    }
}
