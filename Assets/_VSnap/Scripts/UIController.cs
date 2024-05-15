using UnityEngine;
using R3;

public class UIController : MonoBehaviour
{
    [SerializeField] StateManager manager;

    private void Start()
    {
        manager.state.Subscribe(x => {
            Debug.Log(x);
        }).AddTo(this);
    }

    public void Record()
    {
        if (manager.state.Value != StateManager.UIState.Recordig)
        {
            manager.state.Value = StateManager.UIState.Recordig;
        }
        else if (manager.state.Value == StateManager.UIState.Recordig)
        {
            manager.state.Value = StateManager.UIState.Default;
        }
    }
}
