using UnityEngine;
using UnityEngine.UI;
using R3;

public class PosePresenter : MonoBehaviour
{
    // Poseが生成されたらBindする
    public void OnCreatePoseView(Pose pose, PoseView poseView)
    {
        // View -> Model
        poseView.button.onClick.AddListener(() =>
        {
            PoseInfo poseInfo = new PoseInfo();
            poseInfo.authorName = poseView.authorName;
            poseInfo.poseName = poseView.poseName;
            pose.SetPose(poseInfo);
        });

        // Model -> View
    }

    public void OnCreateAuthorView(Menu menu, AuthorView authorView)
    {
        // View -> Model
        authorView.button.onClick.AddListener(() =>
        {
            menu.SetAuthor(authorView.authorName);
        });

        // Model -> View
    }
}
