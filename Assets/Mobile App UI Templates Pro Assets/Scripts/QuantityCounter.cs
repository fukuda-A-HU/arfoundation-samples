using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuantityCounter : MonoBehaviour
{
    [SerializeField]
    private Button decrement_button;

    [SerializeField]
    private Button increment_button;

    [SerializeField]
    private TMPro.TMP_Text counter_text;

    private int counter=1;

    // Start is called before the first frame update
    void Start()
    {
        counter_text.text = counter.ToString();

        decrement_button.onClick.AddListener(() =>
        {
            counter -= 1;
            counter_text.text = counter.ToString();
        });

        increment_button.onClick.AddListener(() =>
        {
            counter += 1;
            counter_text.text = counter.ToString();
        });


    }
}
