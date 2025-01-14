using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using UniVRM10;
using System;
using System.Threading;
using RootMotion.FinalIK;
using System.IO;

public class LoadChara : MonoBehaviour
{
    [SerializeField] private Transform CharaOffset;
    public SerializableReactiveProperty<GameObject> chara = new SerializableReactiveProperty<GameObject>();
    public SerializableReactiveProperty<string> filepath = new SerializableReactiveProperty<string>();
    public SerializableReactiveProperty<string> message = new SerializableReactiveProperty<string>();
    public SerializableReactiveProperty<string> errorMessage = new SerializableReactiveProperty<string>();

    private Vrm10Instance instance = new Vrm10Instance();
    [SerializeField] private string defaultModelPath;

    private void Start()
    {
        filepath.Subscribe(async value =>
        {
            if (value != null && File.Exists(value))
            {
                PlayerPrefs.SetString("ModelPath", value);
                await LoadModel(filepath.Value);
            }
        }).AddTo(this);

        filepath.Value = PlayerPrefs.GetString("ModelPath");
        if (filepath.Value != null && File.Exists(filepath.Value))
        {
            filepath.Value = Application.streamingAssetsPath + defaultModelPath;
        }
    }

    public async UniTask Load()
    {
        // Pick a PDF file
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
                Debug.Log("Operation cancelled");
            else
            {
                if (!path.EndsWith(".vrm"))
                {
                    message.Value = "Only vrm file can load";
                }
                else
                {
                    filepath.Value = path;
                }
            }
        });

        if (permission != NativeFilePicker.Permission.Granted)
        {
            message.Value = "Can't Permission Granted.";
        }
        else if (filepath == null)
        {
            message.Value = "Can't Load ModelPath.";
        }
        else
        {

        }

    }

    public async UniTask LoadModel(string _filepath){

        message.Value = "Model Loading...";

        var loadCount = 3;

        while (loadCount > 0)
        {
            var timeoutController = new TimeoutController();

            try{
                // TimeoutControllerから指定時間後にキャンセルされるCancellationTokenを生成
                var timeoutToken = timeoutController.Timeout(TimeSpan.FromSeconds(5));

                // このGameObjectが破棄されたらキャンセルされるCancellationTokenを生成
                var destroyToken = this.GetCancellationTokenOnDestroy();

                // タイムアウトとDestroyのどちらもでキャンセルするようにTokenを生成
                var linkedToken = CancellationTokenSource
                    .CreateLinkedTokenSource(timeoutToken, destroyToken)
                    .Token;
                
                message.Value = "Model Loading... " + loadCount.ToString();

                instance = await Vrm10.LoadPathAsync(path: _filepath, canLoadVrm0X: true, ct: linkedToken);
                
                if (instance != null && instance.gameObject != null)
                {

                    // charaRootの子に追加
                    instance.gameObject.transform.SetParent(CharaOffset);
                    // localTransformを初期化
                    instance.gameObject.transform.localPosition = Vector3.zero;
                    instance.gameObject.transform.localRotation = Quaternion.identity;

                    // SkinnedMeshRendererのboundsを100倍にする
                    var renderers = instance.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var renderer in renderers)
                    {
                        renderer.localBounds = new Bounds(renderer.localBounds.center, renderer.localBounds.size * 100);
                    }

                    instance.gameObject.AddComponent<LookAtIK>();
                    instance.gameObject.GetComponent<Vrm10Instance>().enabled = false;
                    chara.Value = instance.gameObject;
                    timeoutController.Reset();
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (timeoutController.IsTimeout())
                {
                    errorMessage.Value = "Timeoutによるキャンセルです";
                }
                else
                {
                    errorMessage.Value = "エラーが発生しました " + ex.Message; ;
                }
            }

            loadCount--;
        }
    }
}
