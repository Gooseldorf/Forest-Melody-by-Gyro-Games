using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EaseTester : MonoBehaviour
{
    [SerializeField]
    private Ease easeIn;
    [SerializeField]
    private Ease easeOut;

    [SerializeField]
    private float easeInDuration = .5f;
    [SerializeField]
    private float easeOutDuration = .5f;

    [SerializeField]
    private RectTransform stageVisuals;
    [SerializeField]
    private int birdSizeDeltaChange = 10;

    private Vector2 defaultSizeDelta;

    void Start()
    {
        defaultSizeDelta = stageVisuals.sizeDelta;
        StartSeq();
    }


    private void StartSeq()
    {
        Sequence TapSeq = DOTween.Sequence();
        stageVisuals.sizeDelta = defaultSizeDelta;
        TapSeq.Append(stageVisuals.DOSizeDelta(stageVisuals.sizeDelta + new Vector2(0, birdSizeDeltaChange), easeInDuration).SetEase(easeIn));
        TapSeq.Append(stageVisuals.DOSizeDelta(defaultSizeDelta, easeOutDuration).SetEase(easeOut));
        if (easeInDuration + easeOutDuration < 1)
            TapSeq.AppendInterval(1 - easeInDuration - easeOutDuration);
        TapSeq.OnComplete(() => StartSeq());
    }
}
