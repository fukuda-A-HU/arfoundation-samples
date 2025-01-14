using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using System.Collections.Generic;
using System;
using RootMotion.FinalIK;

public class CharaPresenter : MonoBehaviour
{
    [SerializeField] private Chara chara;
    [SerializeField] private CharaPose pose;
    [SerializeField] private ShapeKey shapekey;
    [SerializeField] private Gaze gaze;

    void Start()
    {
        chara.chara.Subscribe(value =>
        {
            if (value == null) return;
            pose.SetAnimator(value.gameObject.GetComponent<Animator>());
        }).AddTo(this);

        chara.chara.Subscribe(value =>
        {
            shapekey.renderers.Clear();

            var renderers = chara.GetRenderers();
            if (renderers == null) return;
            foreach(var renderer in renderers)
            {
                shapekey.AddRenderer(renderer);
            }
        }).AddTo(this);

        chara.chara.Subscribe(value =>
        {
            if (value == null) return;
            gaze.SetIK(value.gameObject.GetComponent<LookAtIK>(), value.gameObject.GetComponent<Animator>());
        }).AddTo(this);
    }
}
