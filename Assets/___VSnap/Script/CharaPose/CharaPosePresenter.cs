using UnityEngine;
using UnityEngine.UI;
using R3;
using Cysharp.Threading.Tasks;

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
        pose.poseNameInfo.Subscribe(x =>
        {
            if(x == null) { return; }

            if (x.authorName == poseView.authorName && x.poseName == poseView.poseName)
            {
                poseView.SetSelected(true);
            }
            else
            {
                poseView.SetSelected(false);
            }
        }).AddTo(poseView.gameObject);
    }

    public void OnCreateAuthorView(CharaPose pose, AuthorView authorView)
    {
        // View -> Model
        authorView.button.onClick.AddListener(() =>
        {
            pose.SetAuthor(authorView.authorName);
        });

        // Model -> View
        pose.selectedAuthor.Subscribe(x =>
        {
            if (x == authorView.authorName)
            {
                authorView.SetSelected(true);
            }
            else
            {
                authorView.SetSelected(false);
            }
        }).AddTo(authorView.gameObject);
    }
}
