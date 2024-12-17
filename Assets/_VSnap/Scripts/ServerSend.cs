using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.ComponentModel.DataAnnotations;
using System;
using UnityEngine.Rendering.Universal;
using System.Threading;
using TMPro;

public class ServerSend : MonoBehaviour
{
    [SerializeField] MessageController messageClosableController;

    [SerializeField] string BaseURL;

    [SerializeField] private Shutter shutter;

    [SerializeField] private TMP_InputField inputField;

    private string requestURL;

    private GPS gps;

    void Start(){
        gps = GetComponent<GPS>();
    }

    [Serializable]
    public class Data
    {
        public float latitude = 40;
        public float longitude = 135;
        public float altitude = 10;
        public double timestamp = 1000000000000000;

        public Texture2D texture;
    }

    public void Send(){
        SendAsync(shutter.ScreenShot);
    }

    public async UniTask SendAsync(Texture2D tex){
        byte[] bytes = tex.EncodeToPNG();

        LocationService service =  await gps.GetLocation();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        // Can't Upload Image without text
        if (inputField.text == null){
            inputField.text = "no review";
        }

        if (service != null){

            formData.Add(new MultipartFormDataSection("latitude", service.lastData.latitude.ToString()));

            formData.Add(new MultipartFormDataSection("longitude", service.lastData.longitude.ToString()));

            formData.Add(new MultipartFormDataSection("altitude", service.lastData.altitude.ToString()));

            formData.Add(new MultipartFormDataSection("timestamp", TimeUtil.GetUnixTime(DateTime.Now).ToString()));

            formData.Add(new MultipartFormDataSection("review", inputField.text));

            formData.Add(new MultipartFormFileSection(name: "image", data: bytes, fileName: "picture.png", contentType: "image/png"));

            requestURL = BaseURL + "/image/with_location";
        }
        else{

            formData.Add(new MultipartFormDataSection("timestamp", TimeUtil.GetUnixTime(DateTime.Now).ToString()));

            formData.Add(new MultipartFormDataSection("review", inputField.text));

            formData.Add(new MultipartFormFileSection(name: "image", data: bytes, fileName: "picture.png", contentType: "image/png"));;
            
            requestURL = BaseURL + "/image/without_location";
        }

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        await www.SendWebRequest();

        if (www.isNetworkError)
        {
            messageClosableController.gameObject.SetActive(true);

            messageClosableController.Text = www.error;
        }
        else if (www.isHttpError)
        {
            messageClosableController.gameObject.SetActive(true);

            messageClosableController.Text = www.error;
        }
        else
        {
            // messageClosableController.gameObject.SetActive(true);

            // messageClosableController.Text = www.downloadHandler.text;
        }

    }
}
