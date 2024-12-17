using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UniVRM10;
using R3;
using UnityEngine.XR.ARFoundation;

public class LoadVRM : MonoBehaviour
{
    private string filepath = "Assets/Hakka.vrm";

    [SerializeField]
    private Vrm10Instance initialCharactor;

    public SerializableReactiveProperty<Vrm10Instance> vrmInstance = new SerializableReactiveProperty<Vrm10Instance>();

    private Vrm10Instance instance;

    [SerializeField]
    RuntimeAnimatorController animatorController;

    [SerializeField] MessageController messageController;

    [SerializeField] MessageController messageClosableController;

    [SerializeField] GameObject ShadowPlane;

    [SerializeField] ARSession session;

    private void Start()
    {
        SetInitialCharactor();
    }

    async private UniTask SetInitialCharactor(){

        await UniTask.Yield();

        instance = initialCharactor;

        vrmInstance.Value = instance;

        var anim = instance.gameObject.GetComponent<Animator>();

        anim.runtimeAnimatorController = animatorController;

        anim.enabled = false;

        instance.GetComponent<Vrm10Instance>().enabled = false;

    }

    public void Load()
    {
        Instance();
    }

    public async UniTask Instance()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);

            instance = null;
        }

        // Pick a PDF file
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
                Debug.Log("Operation cancelled");
            else
            {
                if (!path.EndsWith(".vrm"))
                {
                    Debug.Log("Only vrm file can load");

                    filepath = null;
                }
                else
                {
                    filepath = path;

                    loadModel();
                }
            }
        });

        if (permission != NativeFilePicker.Permission.Granted)
        {
            messageClosableController.gameObject.SetActive(true);

            messageClosableController.Text = "Can't Permission Granted.";
        }
        else if (filepath == null)
        {
            messageClosableController.gameObject.SetActive(true);

            messageClosableController.Text = "Can't Load ModelPath.";
        }
        else
        {

        }

    }

    public async UniTask loadModel(){

        messageController.gameObject.SetActive(true);

        messageController.Text = "Model Loading...";

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
                
                messageController.Text = "Model Loading... " + loadCount.ToString();

                instance = await Vrm10.LoadPathAsync(path: filepath, canLoadVrm0X: true, ct: linkedToken);

                // Instantiate(ShadowPlane, instance.gameObject.transform);

                timeoutController.Reset();

            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (timeoutController.IsTimeout())
                {
                    Debug.LogError("Timeoutによるキャンセルです");
                }
            }

            if (instance != null){
                break;
            }

            loadCount -= 1;
        }

        if (instance == null)
        {
            messageController.gameObject.SetActive(false);

            messageClosableController.gameObject.SetActive(true);

            messageClosableController.Text = "Model Loading Failed";
        }
        else
        {
            vrmInstance.Value = instance;

            var anim = instance.gameObject.GetComponent<Animator>();

            anim.runtimeAnimatorController = animatorController;

            anim.enabled = false;

            instance.GetComponent<Vrm10Instance>().enabled = false;

            messageController.gameObject.SetActive(false);

            session.Reset();
        }

    }
}
