using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

public class GPS : MonoBehaviour
{


    [SerializeField]
    int TimeoutSeconds;

    [SerializeField]
    MessageController messageController;

    void Start(){

        Input.location.Start();

    }
    public async UniTask<LocationService> GetLocation(){

        if (!Input.location.isEnabledByUser)
        {
            messageController.gameObject.SetActive(true);

            messageController.Text = "Can't Access GPS";

            return null;
        }
        else
        {
            var timeoutController = new TimeoutController();

            try{
                // TimeoutControllerから指定時間後にキャンセルされるCancellationTokenを生成
                var timeoutToken = timeoutController.Timeout(TimeSpan.FromSeconds(TimeoutSeconds));

                // このGameObjectが破棄されたらキャンセルされるCancellationTokenを生成
                var destroyToken = this.GetCancellationTokenOnDestroy();

                // タイムアウトとDestroyのどちらもでキャンセルするようにTokenを生成
                var token = CancellationTokenSource
                    .CreateLinkedTokenSource(timeoutToken, destroyToken)
                    .Token;

                await UniTask.WaitUntil(() => Input.location.status == LocationServiceStatus.Running, cancellationToken: token);

                timeoutController.Reset();

                return Input.location;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (timeoutController.IsTimeout())
                {
                    Debug.LogError("Timeoutによるキャンセルです");
                }
                return null;
            }
        }
    }
}
