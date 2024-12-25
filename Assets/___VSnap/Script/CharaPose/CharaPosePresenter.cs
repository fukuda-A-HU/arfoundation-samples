using UnityEngine;
using UnityEngine.UI;
using R3;

public class CharaPosePresenter : MonoBehaviour
{
    // Poseが生成されたらBindする
    public void OnCreatePoseView(CharaPose pose, CharaPoseView poseView)
    {
        // View -> Model
        poseView.button.onClick.AddListener(() =>
        {
            PoseNameInfo poseNameInfo = new PoseNameInfo();
            poseNameInfo.authorName = poseView.authorName;
            poseNameInfo.poseName = poseView.poseName;
            pose.SetPose(poseNameInfo);
        });

        // Model -> View
    }

    public void OnCreateAuthorView(CharaPose pose, AuthorView authorView)
    {
        // View -> Model
        authorView.button.onClick.AddListener(() =>
        {
            pose.SetAuthor(authorView.authorName);
        });

        // Model -> View
    }
}
