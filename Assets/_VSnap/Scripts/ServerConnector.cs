using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniGLTF;

public class ServerConnector : MonoBehaviour
{
    [Tooltip("URL of the server to request text from.")]
    [SerializeField] private string serverUrl;

    private string receivedJsonText;
    private Texture receivedTexture;
    private AssetBundle receivedAssetBundle;

    public async UniTask<string> RequestText(string path)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverUrl + path))
        {
            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                receivedJsonText = webRequest.downloadHandler.text;
            }
        }

        return receivedJsonText;
    }

    public async UniTask<Texture> RequestTexture(string path) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(serverUrl + path);
        await www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
         
        else {
            receivedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }

        return receivedTexture;
    }

    public async UniTask<AssetBundle> RequestAssetBundle(string path) {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(serverUrl + path);
        await www.SendWebRequest();
  
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            receivedAssetBundle = DownloadHandlerAssetBundle.GetContent(www);
        }

        return receivedAssetBundle;
    }

    public async UniTask SendFiles(string path, string data)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection(data, "myfile.txt"));
 
        UnityWebRequest www = UnityWebRequest.Post(serverUrl + path, formData);
        await www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }

    public async UniTask SendBytes(string path, byte[] myData) {
        UnityWebRequest www = UnityWebRequest.Put(serverUrl + path, myData);
        await www.SendWebRequest();
  
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Upload complete!");
        }
    }
}