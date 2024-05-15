using UnityEngine;
using R3;

public class Record : MonoBehaviour
{
    [SerializeField] StateManager manager;
    [SerializeField] bool onRecording = false;

    private void Start()
    {
        manager.state.Subscribe(x => {
            if(x == StateManager.UIState.Recordig)
            {
                onRecording = true;
                RecordStart();
            }
            else
            {
                
                if (onRecording == true)
                {
                    onRecording = false;
                    RecordStop();
                }
                
            }
        }).AddTo(this);
    }

    void RecordStart()
    {
        //SmileSoftScreenRecordController.instance.StartRecording();
    }
    void RecordStop()
    {
        //SmileSoftScreenRecordController.instance.StopRecording();
    }
}
