using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System.IO;

public class LoadPose : MonoBehaviour
{
    public SerializableReactiveProperty<PoseObject> poseObject = new SerializableReactiveProperty<PoseObject>();
    public SerializableReactiveProperty<string> filepath = new SerializableReactiveProperty<string>();
    [SerializeField] private string defaultAssetBundlePath = "poseobject";
    private AssetBundle assetBundle;
    public bool isInitialized
    {
        get;
        private set;
    } = false;

    private async void Start()
    {
        filepath.Subscribe(async value =>
        {
            if (value != null)
            {
                await LoadPoseFromBundlePath(value);
            }
        }).AddTo(this);
        isInitialized = true;
    }

    public void LoadDefaultPose()
    {
        string loadPath = PlayerPrefs.GetString("PoseObjectPath");
        //string loadPath = "";
        if (!File.Exists(loadPath))
        {
#if UNITY_ANDROID
            loadPath = Path.Combine(Application.streamingAssetsPath, "Android", defaultAssetBundlePath);
#elif UNITY_IOS
            loadPath = Path.Combine(Application.streamingAssetsPath, "iOS", defaultAssetBundlePath);
#endif
        }

        Debug.Log("LoadDefaultPose: " + filepath.Value);
        filepath.Value = loadPath;
    }

    public void LoadPoseFromBundle()
    {
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
                Debug.Log("Operation cancelled");
            else
            {
                filepath.Value = path;
            }
        });
    }

    private async UniTask LoadPoseFromBundlePath(string path)
    {

        Object[]? assets = await LoadAllAssetsFromPath(path);
        if (assets == null)
        {
            return;
        }

        bool isPoseObjectExist = false;

        foreach (Object asset in assets)
        {
            if (asset.GetType() == typeof(PoseObject))
            {
                poseObject.Value = (PoseObject)asset;
                isPoseObjectExist = true;
                break;
            }
        }

        if (isPoseObjectExist)
        {
            PlayerPrefs.SetString("PoseObjectPath", path);
        }
    }

    private async UniTask<Object[]> LoadAllAssetsFromPath(string path)
    {
        //もしpathが見つからない場合はnullを返す
        if (!File.Exists(path))
        {
            Debug.Log("File is not found");
            return null;
        }
        //もしpathで読み込もうとしているAssetBundleがすでに読み込まれている場合は、読み込まれているAssetBundleを一旦削除する
        if (assetBundle != null)
        {
            await assetBundle.UnloadAsync(true);
        }

        try
        {
            assetBundle = await AssetBundle.LoadFromFileAsync(path);
        }
        catch (System.Exception e)
        {
            Debug.Log("LoadFromFileAsync: " + e.Message);
            return null;
        }

        if (assetBundle == null)
        {
            Debug.Log("AssetBundle is null");
            return null;
        }

        Object[] allAssets = assetBundle.LoadAllAssets();
        return allAssets;
    }
}
