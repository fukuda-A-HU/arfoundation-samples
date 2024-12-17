using UnityEngine;

public class SelectCategory : MonoBehaviour
{
    public GameObject scrollView;

    public void Select()
    {
        foreach (Transform n in scrollView.transform.parent.transform)
        {
            n.gameObject.SetActive(false);
        }

        scrollView.SetActive(true);
    }
}
