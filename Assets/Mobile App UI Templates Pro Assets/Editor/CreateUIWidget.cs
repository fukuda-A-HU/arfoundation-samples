/*
 * 
 * Developed by Olusola Olaoye, 2023
 * 
 * To only be used by those who purchased from the Unity asset store
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// editor script to create UI prefabs
public class CreateUIWidget
{


    [MenuItem("GameObject/Mobile App UI/Slider Toggle", false, -1)]
    private static void createSliderToggle()
    {
        createPrefabResource("Slider Toggle");

    }

    
    [MenuItem("GameObject/Mobile App UI/Slider", false, -1)]
    private static void createSlider()
    {
        createPrefabResource("Slider");

    }

    

    private static void createPrefabResource(string object_name)
    {

        Object ui_object = Resources.Load("prefabs/" + object_name); // find prefab in resources

        GameObject ui_game_object = (GameObject)GameObject.Instantiate(ui_object, Vector3.zero, Quaternion.identity); // instantiate ui 

        ui_game_object.name = object_name; // name object



        GameObject selected_object = Selection.activeGameObject; // current selected game object


        if (selected_object) // if selected object is not null
        {

            if (selected_object.GetComponent<RectTransform>()) // if there is a current seleted game object and that game object has a RectTransform component
            {
                ui_game_object.transform.SetParent(selected_object.transform, false);
            }

            else// if there is a current seleted game object and that game object does not have a RectTransform component
            {
                GameObject canvas = createCustomCanvasObject(); // create a canvas object

                canvas.transform.SetParent(selected_object.transform); // set canvas object to child of selected game object

                ui_game_object.transform.SetParent(canvas.transform, false); // set the UI to the child of the canvas
            }

        }
        else // if selected object is null
        {
            // create a canvas object if none exists
            GameObject canvas = GameObject.FindObjectOfType<Canvas>() ? GameObject.FindObjectOfType<Canvas>().gameObject : createCustomCanvasObject();

            // set the UI object to the child of the canvas
            ui_game_object.transform.SetParent(canvas.transform, false);
        }


       

        Selection.activeGameObject = ui_game_object;
    }


    private static GameObject createCustomCanvasObject()
    {
        // create a gameobject with all the canvas components attached 
        GameObject canvas_object = new GameObject("Canvas", new System.Type[] { typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster) });

        canvas_object.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay; // set canvas to screen space mode

        canvas_object.layer = 5; // Ui layer

        return canvas_object;
    }
}