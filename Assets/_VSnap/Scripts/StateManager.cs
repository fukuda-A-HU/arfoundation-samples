using UnityEngine;
using R3;

public class StateManager : MonoBehaviour
{
    public ReactiveProperty<UIState> state = new ReactiveProperty<UIState>();

    public enum UIState
    {
        Default,
        Recordig
    }
}
