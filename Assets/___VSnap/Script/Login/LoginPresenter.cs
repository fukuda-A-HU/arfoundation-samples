using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using R3;

public class LoginPresenter : MonoBehaviour
{
    [SerializeField] private Login login;
    [SerializeField] private LoginView loginView;

    private void Start()
    {
        loginView.userLoginButton.onClick.AddListener(() =>
        {
            login.TryLogin(loginView.userNameInputField.text, loginView.passwordInputField.text);
        });

        login.isLogin.Subscribe(x =>
        {
            if (x)
            {
                Debug.Log("Login Success");
                loginView.loginPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Login Failed");
                loginView.loginPanel.SetActive(true);
            }
        }).AddTo(this);

        login.message.Subscribe(x =>
        {
            loginView.messageText.text = x;
        }).AddTo(this);
    }
}
