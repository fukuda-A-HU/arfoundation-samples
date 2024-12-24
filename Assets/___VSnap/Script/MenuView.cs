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
    [Header("Mode Switch")]
    public LeanFingerSwipe leftSwipe;
    public LeanFingerSwipe rightSwipe;
    public LeanFingerSwipe upSwipe;
    public LeanFingerSwipe downSwipe;

    [Header("Mode Lavel & Transform")]
    [SerializeField] private TextMeshProUGUI modeLabel;
    [SerializeField] private RectTransform backGroundTransform;
    [SerializeField] private RectTransform BasicTransform;
    [SerializeField] private RectTransform poseTransform;
    [SerializeField] private RectTransform advancedTransform;

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


    public void SetMode(MenuMode mode)
    {
        // 画面の幅を取得
        int width = Screen.width * 2;
        float duration = 0.1f;

        switch (mode)
        {
            case MenuMode.Basic:
                modeLabel.text = "Basic";
                backGroundTransform.DOAnchorPosY(600, duration);
                poseTransform.DOAnchorPosX(width, duration);
                advancedTransform.DOAnchorPosX(width, duration);
                BasicTransform.DOAnchorPosX(0, duration);
                break;
            case MenuMode.Pose:
                modeLabel.text = "Pose";
                backGroundTransform.DOAnchorPosY(1000, duration);
                poseTransform.DOAnchorPosX(0, duration);
                BasicTransform.DOAnchorPosX(-width, duration);
                advancedTransform.DOAnchorPosX(width, duration);
                break;
            case MenuMode.Advanced:
                modeLabel.text = "Advanced";
                backGroundTransform.DOAnchorPosY(750, duration);
                poseTransform.DOAnchorPosX(-width, duration);
                BasicTransform.DOAnchorPosX(-width, duration);
                advancedTransform.DOAnchorPosX(0, duration);
                break;
        }
    }

    public void SetOpen(bool isOpen)
    {
        int menuHeight = 900;
        float duration = 0.1f;
        int menuYPos = 50;

        if (isOpen)
        {
            BasicTransform.DOAnchorPosY(menuYPos, duration);
            poseTransform.DOAnchorPosY(menuYPos, duration);
            advancedTransform.DOAnchorPosY(menuYPos, duration);
        }
        else
        {
            BasicTransform.DOAnchorPosY(-menuHeight, duration);
            poseTransform.DOAnchorPosY(-menuHeight, duration);
            advancedTransform.DOAnchorPosY(-menuHeight, duration);
        }
    }

    public void SetAdvancedMode(AdvancedMenuMode mode)
    {
        int width = Screen.width * 2;
        float duration = 0.1f;

        switch (mode)
        {
            case AdvancedMenuMode.LightDir:
                lightDirPanel.SetActive(true);
                colorPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                break;
            case AdvancedMenuMode.EnvColor:
                colorPanel.SetActive(true);
                lightDirPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                break;
            case AdvancedMenuMode.DirColor:
                colorPanel.SetActive(true);
                lightDirPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                break;
            case AdvancedMenuMode.CharaShape:
                ShapeKeyPanel.SetActive(true);
                colorPanel.SetActive(false);
                lightDirPanel.SetActive(false);
                gazePanel.SetActive(false);
                break;
            case AdvancedMenuMode.CharaGaze:
                gazePanel.SetActive(true);
                ShapeKeyPanel.SetActive(false);
                colorPanel.SetActive(false);
                lightDirPanel.SetActive(false);
                break;
        }
    }
}
