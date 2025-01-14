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
    [SerializeField] private RectTransform topBackGroundTransform;
    [SerializeField] private RectTransform BasicTransform;
    [SerializeField] private RectTransform poseTransform;
    [SerializeField] private RectTransform advancedTransform;
    [SerializeField] private RectTransform gridViewTransform;
    [Header("Poses Credit")]
    [SerializeField] private RectTransform posesCreditTransform;
    [SerializeField] private Image poseCreditBackGround;
    public Button posesCreditButton;
    [SerializeField] private Button poseLoadButton;

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
        int width = Screen.width + 100;
        float duration = 0.3f;
        Ease ease = Ease.OutCubic;

        //画面全体の上辺とセーフエリアの上辺の差分を取得
        float safeAreaTop = Screen.safeArea.yMax;
        float screenTop = Screen.height;
        float topMargin = screenTop - safeAreaTop;
        // topBackGroundTransformの高さをセーフエリアの上辺までに設定するY座標を計算
        float topBackGroundY = 500;

        switch (mode)
        {
            case MenuMode.PlaceChara:
                modeLabel.text = "Place Character";
                backGroundTransform.DOAnchorPosY(0, duration).SetEase(ease);
                topBackGroundTransform.DOAnchorPosY(topBackGroundY, duration).SetEase(ease);

                poseTransform.DOAnchorPosX(width, duration).SetEase(ease).OnComplete(() => {
                    poseTransform.gameObject.SetActive(false);
                });

                posesCreditButton.gameObject.SetActive(false);
                poseLoadButton.gameObject.SetActive(false);

                BasicTransform.DOAnchorPosX(width, duration).SetEase(ease).OnComplete(() => {
                    BasicTransform.gameObject.SetActive(false);
                });

                advancedTransform.DOAnchorPosX(width, duration).SetEase(ease).OnComplete(() => {
                    advancedTransform.gameObject.SetActive(false);
                });

                gridViewTransform.DOAnchorPosX(width, duration).SetEase(ease);
                shutterButton.gameObject.SetActive(false);
                break;
            case MenuMode.Basic:
                modeLabel.text = "Charactor Change & Transform";
                backGroundTransform.DOAnchorPosY(700, duration).SetEase(ease);
                topBackGroundTransform.DOAnchorPosY(topBackGroundY, duration).SetEase(ease);

                poseTransform.DOAnchorPosX(width, duration).SetEase(ease).OnComplete(() => {
                    poseTransform.gameObject.SetActive(false);
                });

                posesCreditButton.gameObject.SetActive(false);
                poseLoadButton.gameObject.SetActive(false);

                advancedTransform.DOAnchorPosX(width, duration).SetEase(ease).OnComplete(() => {
                    advancedTransform.gameObject.SetActive(false);
                });

                BasicTransform.gameObject.SetActive(true);
                BasicTransform.DOAnchorPosX(0, duration).SetEase(ease);

                gridViewTransform.DOAnchorPosX(width, duration).SetEase(ease);
                shutterButton.gameObject.SetActive(false);
                break;
            case MenuMode.Pose:
                modeLabel.text = "Pose Change";
                backGroundTransform.DOAnchorPosY(700, duration).SetEase(ease);
                topBackGroundTransform.DOAnchorPosY(topBackGroundY, duration).SetEase(ease);
                
                poseTransform.gameObject.SetActive(true);
                poseTransform.DOAnchorPosX(0, duration).SetEase(ease);

                posesCreditButton.gameObject.SetActive(true);
                poseLoadButton.gameObject.SetActive(true);

                BasicTransform.DOAnchorPosX(-width, duration).SetEase(ease).OnComplete(() => {
                    BasicTransform.gameObject.SetActive(false);
                });

                advancedTransform.DOAnchorPosX(width, duration).SetEase(ease).OnComplete(() => {
                    advancedTransform.gameObject.SetActive(false);
                });

                gridViewTransform.DOAnchorPosX(width, duration).SetEase(ease);
                shutterButton.gameObject.SetActive(false);
                break;
            case MenuMode.Advanced:
                modeLabel.text = "Light & Other Settings";
                topBackGroundTransform.DOAnchorPosY(topBackGroundY, duration).SetEase(ease);

                poseTransform.DOAnchorPosX(-width, duration).SetEase(ease).OnComplete(() => {
                    poseTransform.gameObject.SetActive(false);
                });

                posesCreditButton.gameObject.SetActive(false);
                poseLoadButton.gameObject.SetActive(false);

                BasicTransform.DOAnchorPosX(-width, duration).SetEase(ease).OnComplete(() => {
                    BasicTransform.gameObject.SetActive(false);
                });

                advancedTransform.gameObject.SetActive(true);
                advancedTransform.DOAnchorPosX(0, duration).SetEase(ease);

                gridViewTransform.DOAnchorPosX(width, duration).SetEase(ease);
                shutterButton.gameObject.SetActive(false);
                SetAdvancedMode(MenuMode.Advanced, AdvancedMenuMode.LightDir);
                break;
            case MenuMode.GridView:
                backGroundTransform.DOAnchorPosY(0, duration).SetEase(ease);
                topBackGroundTransform.DOAnchorPosY(600 + topMargin, duration).SetEase(ease).OnComplete(() => {
                    modeLabel.text = "";
                });

                poseTransform.DOAnchorPosX(-width, duration).SetEase(ease).OnComplete(() => {
                    poseTransform.gameObject.SetActive(false);
                });

                posesCreditButton.gameObject.SetActive(false);
                poseLoadButton.gameObject.SetActive(false);

                BasicTransform.DOAnchorPosX(-width, duration).SetEase(ease).OnComplete(() => {
                    BasicTransform.gameObject.SetActive(false);
                });

                advancedTransform.DOAnchorPosX(-width, duration).SetEase(ease).OnComplete(() => {
                    advancedTransform.gameObject.SetActive(false);
                });

                gridViewTransform.DOAnchorPosX(0, duration);
                shutterButton.gameObject.SetActive(true);
                break;
        }
    }

    public void SetAdvancedMode(MenuMode menuMode, AdvancedMenuMode mode)
    {
        float duration = 0.3f;
        Ease ease = Ease.OutCubic;

        if (menuMode != MenuMode.Advanced)
        {
            return;
        }

        switch (mode)
        {
            case AdvancedMenuMode.LightDir:
                lightDirPanel.SetActive(true);
                colorPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(700, duration).SetEase(ease);
                break;
            case AdvancedMenuMode.EnvColor:
                colorPanel.SetActive(true);
                lightDirPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(700, duration).SetEase(ease);
                break;
            case AdvancedMenuMode.DirColor:
                colorPanel.SetActive(true);
                lightDirPanel.SetActive(false);
                ShapeKeyPanel.SetActive(false);
                gazePanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(700, duration).SetEase(ease);
                break;
            case AdvancedMenuMode.CharaShape:
                ShapeKeyPanel.SetActive(true);
                colorPanel.SetActive(false);
                lightDirPanel.SetActive(false);
                gazePanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(850, duration).SetEase(ease);
                break;
            case AdvancedMenuMode.CharaGaze:
                gazePanel.SetActive(true);
                ShapeKeyPanel.SetActive(false);
                colorPanel.SetActive(false);
                lightDirPanel.SetActive(false);
                backGroundTransform.DOAnchorPosY(700, duration).SetEase(ease);
                break;
        }
    }

    public void SetPoseCredit(bool isPoseCreditOpen, MenuMode mode)
    {
        int height = Screen.height + 300;

        if (isPoseCreditOpen)
        {
            posesCreditTransform.gameObject.SetActive(true);
            posesCreditTransform.DOAnchorPosY(50, 0.3f).SetEase(Ease.OutCubic);
            poseCreditBackGround.DOFade(0.9f, 0.3f);
        }
        else
        {
            posesCreditTransform.DOAnchorPosY(height, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                posesCreditTransform.gameObject.SetActive(false);
            });
            poseCreditBackGround.DOFade(0, 0.3f);
        }
    }

}
