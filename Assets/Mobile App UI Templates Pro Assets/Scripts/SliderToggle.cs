/*
 * 
 * Developed by Olusola Olaoye, 2022
 * 
 * To only be used by those who purchased from the Unity asset store
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderToggle : MonoBehaviour
{

    [SerializeField]
    private Button button; // the button we will click on for toggle slide animation to happen

    [SerializeField]
    private Image knob; // the knob that would be sliding


    [SerializeField]
    private TMPro.TMP_Text toggle_slider_text; // for displaying if tiggle is on or off


    [SerializeField]
    private Color on_color_for_background; // background color when toggle is on

    [SerializeField]
    private Color off_color_for_background; // background color when toggle is off


    [Space]

    [SerializeField]
    private Color on_color_for_knob; // knob color when toggle is on

    [SerializeField]
    private Color off_color_for_knob; // knob color when toggle is off


    [SerializeField]
    private string on_text;

    [SerializeField]
    private string off_text;




    [SerializeField]
    private float animate_speed = 10; // speed to animate toggle

    public bool is_on // if toggle is on
    {
        get;
        set;
    }
    private bool animate_to_the_right; // animate to the right to switch toggle on

    private bool animate_to_the_left; // animate to the left to switch toggle off

    private float off_position; // position of the knob in the off state

    private void Start()
    {
        off_position = (button.image.rectTransform.rect.width / 2) - (knob.rectTransform.rect.width / 2);


        animate_to_the_left = true; // start by being at the left

        button.onClick.AddListener(() => updateKnobPosition());
    }
    
    private void updateKnobPosition()
    {
        
        if (is_on)
        {

            animate_to_the_left = true;
        }
        else
        {
            animate_to_the_right = true;

        }

    }

    private void Update()
    {
         
        if (animate_to_the_left)
        {
            knob.rectTransform.anchoredPosition = Vector2.Lerp(knob.rectTransform.anchoredPosition, new Vector2(-off_position, 0), animate_speed * Time.deltaTime);

            if(Vector2.Distance(knob.rectTransform.anchoredPosition, new Vector2(-off_position, 0)) < 1) // if knob is really close to on position
            {
                animate_to_the_left = false;
                button.image.color = off_color_for_background;

                knob.color = off_color_for_knob;
                is_on = false;
            }
        }

        if(animate_to_the_right)
        {
            knob.rectTransform.anchoredPosition = Vector2.Lerp(knob.rectTransform.anchoredPosition, new Vector2(off_position, 0), animate_speed * Time.deltaTime);

            if (Vector2.Distance(knob.rectTransform.anchoredPosition, new Vector2(off_position, 0)) < 1) // if knob is really close to off position
            {
                animate_to_the_right = false;
                button.image.color = on_color_for_background;

                knob.color = on_color_for_knob;
                is_on = true;
            }
        }

        toggle_slider_text.text = is_on ? on_text : off_text;
    }
}
