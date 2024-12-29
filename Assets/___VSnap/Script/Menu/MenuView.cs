using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;
using R3;
using Lean.Touch;
using ObservableCollections;
using TMPro;

public class MenuView : MonoBehaviour
{
    [Header("UI Color")]
    [SerializeField] private Color basicColor;
    [SerializeField] private Color SelectedColor;
    [Header("Mode Switch Swipe")]
    public LeanFingerSwipe leftSwipe;
    public LeanFingerSwipe rightSwipe;
    public LeanFingerSwipe upSwipe;
    public LeanFingerSwipe downSwipe;

    [Header("Mode Switch Button")]
    public Button modeLeftButton;
    public Button modeRightButton;
    [Header("Shooting Button")]
    public RectTransform shutterButton;

    [Header("Mode Lavel & Transform")]
    [SerializeField] private TextMeshProUGUI modeLabel;
    [SerializeField] private RectTransform backGroundTransform;
    [SerializeField] private RectTransform BasicTransform;
    [SerializeField] private RectTransform poseTransform;
    [SerializeField] private RectTransform advancedTransform;
    [SerializeField] private RectTransform gridViewTransform;

    [Header("MainMenu Basic")]
    public Button resetARButton;
    public Button checkPlaneButton;
    public Button charaChangeButton;

    [Header("MainMenu Advanced")]
    public Button lightDirButton;
    public Button envColorButton;
    public Button dirColorButton;
    public Button charaShapeButton;
    public Button charaGazeButton;

    [Header("SubMenu Advanced")]
    public GameObject lightDirPanel;
    public GameObject colorPanel;
    public GameObject ShapeKeyPanel;
    public GameObject gazePanel;

    public void SetCheckPlane(bool isCheck)
    {
        if (isCheck)
        {
            checkPlaneButton.GetComponent<Image>().color = SelectedColor;
        }
        else
        {
            checkPlaneButton.GetComponent<Image>().color = basicColor;
        }
    }


    public void SetMode(MenuMode mode)
    {
        // 画面の幅を取得
        int width = Screen.width * 2;
        float duration = 0.1f;

        switch (mode)
        {
            case MenuMode.Basic:
                modeLabel.text = "Basic";
                backGroundTransform.DOAnchorPosY(700, duration);
                poseTransform.DOAnchorPosX(width, duration);
                advancedTransform.DOAnchorPosX(width, duration);
                BasicTransform.DOAnchorPosX(0, duration);
                gridViewTransform.DOAnchorPosX(width, duration);
                shutterButton.gameObject.SetActive(false);
                break;
            case MenuMode.Pose:
                modeLabel.text = "Pose";
                backGroundTransform.DOAnchorPosY(850, duration);
                poseTransform.DOAnchorPosX(0, duration);
                BasicTransform.DOAnchorPosX(-width, duration);
                advancedTransform.DOAnchorPosX(width, duration);
                gridViewTransform.DOAnchorPosX(width, duration);
                shutterButton.gameObject.SetActive(false);
                break;
            case MenuMode.Advanced:
                modeLabel.text = "Advanced";
                // backGroundTransform.DOAnchorPosY(700, duration);
                poseTransform.DOAnchorPosX(-width, duration);
                BasicTransform.DOAnchorPosX(-width, duration);
                advancedTransform.DOAnchorPosX(0, duration);
                gridViewTransform.DOAnchorPosX(width, duration);
                shutterButton.gameObject.SetActive(false);
                break;
            case MenuMode.GridView:
                modeLabel.text = "GridView";
                backGroundTransform.DOAnchorPosY(0, duration);
                poseTransform.DOAnchorPosX(-width, duration);
                BasicTransform.DOAnchorPosX(-width, duration);
                advancedTransform.DOAnchorPosX(-width, duration);
                gridViewTransform.DOAnchorPosX(0, duration);
                shutterButton.gameObject.SetActive(true);
                break;
            case MenuMode.PlaceChara:
                modeLabel.text = "PlaceChara";
                backGroundTransform.DOAnchorPosY(0, duration);
                poseTransform.DOAnchorPosX(width, duration);
                BasicTransform.DOAnchorPosX(width, duration);
                advancedTransform.DOAnchorPosX(width, duration);
                gridViewTransform.DOAnchorPosX(width, duration);
                shutterButton.gameObject.SetActive(false);
                break;
        }
    }

    public void SetAdvancedMode(AdvancedMenuMode mode)
    {
        float duration = 0.1f;

        switch (mode)
        {
            case AdvancedMenuMode.LightDir:
                lightDirPanel.SetActive(true);
                colorPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(700, duration);
                break;
            case AdvancedMenuMode.EnvColor:
                colorPanel.SetActive(true);
                lightDirPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(700, duration);
                break;
            case AdvancedMenuMode.DirColor:
                colorPanel.SetActive(true);
                lightDirPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(700, duration);
                break;
            case AdvancedMenuMode.CharaShape:
                ShapeKeyPanel.SetActive(true);
                colorPanel.SetActive(false);
                lightDirPanel.SetActive(false);
                gazePanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(850, duration);
                break;
            case AdvancedMenuMode.CharaGaze:
                gazePanel.SetActive(true);
                ShapeKeyPanel.SetActive(false);
                colorPanel.SetActive(false);
                lightDirPanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(700, duration);
                break;
        }
    }
}
