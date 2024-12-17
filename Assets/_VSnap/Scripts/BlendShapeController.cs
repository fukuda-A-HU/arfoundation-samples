using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using R3;

public class  BlendShapeController : MonoBehaviour
{
    [SerializeField]
    LoadVRM loadVRM;

    [SerializeField]
    private GameObject sliderPrefab;

    [SerializeField]
    private GameObject sliderRoot;

    private GameObject charactorRoot;

    void Start(){

        loadVRM.vrmInstance.Subscribe(x => {
            
            if (x != null){
                charactorRoot = x.gameObject;

                foreach (Transform t in sliderRoot.transform)
                {
                    Destroy(t.gameObject);
                }

                foreach (Transform t in GetChildrenRecursive(charactorRoot.transform, false))
                {
                var renderer = t.gameObject.GetComponent<SkinnedMeshRenderer>();

                if (renderer != null)
                {
                    for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                    {
                            var sliderObject = Instantiate(sliderPrefab, sliderRoot.transform);

                            var weightChanger = sliderObject.GetComponent<BlendShapeWeightChanger>();

                            weightChanger.blendShapeIndex = i;

                            weightChanger.renderer = renderer;

                            weightChanger.currentValue = renderer.GetBlendShapeWeight(i);

                            sliderObject.GetComponent<HoldObject>().gameObject[0].GetComponent<TextMeshProUGUI>().text = renderer.sharedMesh.GetBlendShapeName(i);
                        }
                    }
                };
            }

        }).AddTo(this);
    }

    // parent直下の子オブジェクトを再帰的に取得する
    private static Transform[] GetChildrenRecursive(Transform parent, bool includeParent = true)
    {
        // 親を含む子オブジェクトを再帰的に取得
        // trueを指定しないと非アクティブなオブジェクトを取得できないことに注意
        var parentAndChildren = parent.GetComponentsInChildren<Transform>(true);

        if (includeParent)
        {
            // 親を含む場合はそのまま返す
            return parentAndChildren;
        }

        // 子オブジェクトの格納用配列作成
        var children = new Transform[parentAndChildren.Length - 1];

        // 親を除く子オブジェクトを結果にコピー
        Array.Copy(parentAndChildren, 1, children, 0, children.Length);

        // 子オブジェクトが再帰的に格納された配列
        return children;
    }

}
