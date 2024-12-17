using UnityEngine;
using Timeline;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] PlayableDirector playableDirector;

    [SerializeField] PlayableAsset playableAsset;

    public void PlayTimeline()
    {
        playableDirector.Play(playableAsset);
    }
}
