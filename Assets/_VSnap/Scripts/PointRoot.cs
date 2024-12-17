using UnityEngine;
using CesiumForUnity;
using Unity.Mathematics;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections.Generic;
using CesiumForUnity;
using System.Collections.Generic;

public enum Weather{
    Sunny,
    Cloudy,
    Rainy,
    Snowy
}

[System.Serializable]
public class Layer1{

    public List<Layer2> list;

}

[System.Serializable]
public class Layer2{

    public string user_id;
    public string event_id;
    public float latitude;
    public float longitude;
    public float altitude;
    public double timestamp;
    public string review;
    public string imagepath;
    public string nearsides;

    public string weather;

}
public class PointRoot : MonoBehaviour
{
    [SerializeField] 
    private GameObject pointPrefab;

    [SerializeField]
    private Transform parent;

    [SerializeField]
    private string serverUrl;

    [SerializeField]
    private string dataPath;

    [SerializeField]
    private string imagePath;

    [SerializeField]
    private CesiumGlobeAnchor anchor;

    private string receivedJsonText;

    private Texture2D receivedTexture;

    void Start()
    {
        UpdatePoint();
    }

    void Update()
    {
        anchor.height = 0;
    }

    public async UniTask UpdatePoint(){

        string jsonText = await RequestText(dataPath);

        Debug.Log(jsonText);

        Layer1 layer1 = JsonUtility.FromJson<Layer1>(jsonText);

        Debug.Log(layer1.list.Count);

        foreach (Layer2 layer2 in layer1.list){

            GameObject point = Instantiate(pointPrefab, new Vector3(layer2.latitude, layer2.altitude, layer2.longitude), Quaternion.identity, parent);

            PointController pointController = point.GetComponent<PointController>();

            CesiumGlobeAnchor globeAnchor = point.GetComponent<CesiumGlobeAnchor>();

            globeAnchor.longitudeLatitudeHeight = new double3(layer2.longitude, layer2.latitude, layer2.altitude);

            pointController.Review = layer2.review;

            pointController.nearsides = layer2.nearsides;

            if (layer2.weather == "Sunny") pointController.weather = Weather.Sunny;
            else if (layer2.nearsides == "Cloudy") pointController.weather = Weather.Cloudy;
            else if (layer2.nearsides == "Rainy") pointController.weather = Weather.Rainy;
            else if (layer2.nearsides == "Snowy") pointController.weather = Weather.Snowy;

            //天気はランダムに設定
            pointController.Weather = (Weather)UnityEngine.Random.Range(0, 3);

            string requestURL = serverUrl + imagePath + "?path=" + layer2.imagepath;

            Debug.Log(requestURL);

            pointController.Picture = await RequestTexture(requestURL);

        }
    }

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

    public async UniTask<Texture2D> RequestTexture(string path) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
        await www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
         
        else {
            receivedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }

        return receivedTexture;
    }

}
