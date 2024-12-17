using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.Playables;

public class PreviewScreenShot : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    [SerializeField] 
    private RawImage rawImage;

    [SerializeField]
    float waitSeconds = 3f;


    private TimelinePlayer player;

    void Start(){

        // panel.SetActive(false);

        player = GetComponent<TimelinePlayer>();

    }
    public async UniTask Preview(Texture2D tex){

        Debug.Log(tex.height + " " + tex.width);

        panel.SetActive(true);

        rawImage.texture = tex;

        if (player != null){

            player.PlayTimeline();

        }
        else{
    
        await UniTask.WaitForSeconds(waitSeconds);
        
        }

        // panel.SetActive(false);
    }
}
