using UnityEngine;

public class ARPresenter : MonoBehaviour
{
    [SerializeField] AR ar;
    [SerializeField] MenuView menuView;

    private void Start()
    {
        menuView.resetARButton.onClick.AddListener(() =>
        {
            ar.Reset();
        });

        menuView.checkPlaneButton.onClick.AddListener(() =>
        {
            Debug.Log("Check Plane Button Clicked");
            ar.isOnChange();
        });
    }

    private void OnDestroy()
    {
        menuView.resetARButton.onClick.RemoveAllListeners();

        menuView.checkPlaneButton.onClick.RemoveAllListeners();
    }
}
