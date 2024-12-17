using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneName;
    // シーンを変更するメソッド
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
