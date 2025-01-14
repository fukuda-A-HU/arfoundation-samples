using UnityEngine;
using R3;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public class Login : MonoBehaviour
{
    public SerializableReactiveProperty<bool> isLogin = new SerializableReactiveProperty<bool>();
    [SerializeField] private string serverUrl = "https://shielded-ravine-76397-ae9d6ff640dd.herokuapp.com"; 
    public SerializableReactiveProperty<string> message = new SerializableReactiveProperty<string>();

    private void Start()
    {
        isLogin.Subscribe(x =>
        {
            if (x)
            {
                Debug.Log("Login Success");
            }
            else
            {
                Debug.Log("Login Failed");
            }
        }).AddTo(this);

        TryLogin(PlayerPrefs.GetString("userName"), PlayerPrefs.GetString("password"));
        isLogin.Publish();
        message.Value = "";
    }

    public async UniTask TryLogin(string userName, string password)
    {
        var url = serverUrl + "/login/" + userName + "/" + password;

        var request = UnityWebRequest.Get(url);
        await request.SendWebRequest();

        var result = JsonUtility.FromJson<LoginResult>(request.downloadHandler.text);

        if (request.isNetworkError)
        {
            message.Value = "Network Error";
            isLogin.Value = false;
        }
        else if (request.isHttpError)
        {
            message.Value = "Http Error";
            isLogin.Value = false;
        }
        else if (result.result)
        {
            message.Value = "Login Success";
            isLogin.Value = true;
            PlayerPrefs.SetString("userName", userName);
            PlayerPrefs.SetString("password", password);
        }
        else
        {
            message.Value = "Login Failed";
            isLogin.Value = false;
        }
    }
}

[Serializable]
public class LoginResult
{
    public bool result;
}