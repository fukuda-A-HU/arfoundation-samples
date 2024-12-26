using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TMPro.TMP_Text value_text;


    // Start is called before the first frame update
    void Start()
    {
        updateValueText();

        slider.onValueChanged.AddListener(delegate 
        {
            updateValueText();
        });
    }

    // Update is called once per frame
    void updateValueText()
    {
        value_text.text = slider.value.ToString("n1");
    }
}
