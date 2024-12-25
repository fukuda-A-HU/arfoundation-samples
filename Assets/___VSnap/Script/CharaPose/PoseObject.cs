using UnityEngine;

[CreateAssetMenu(fileName = "PoseObject", menuName = "Scriptable Objects/PoseObject")]
public class PoseObject : ScriptableObject
{
    public PoseGroup[] animationClipGroup;
}

[System.Serializable]
public class PoseGroup
{
    public string name;
    public AnimationClip[] animationClip;
}