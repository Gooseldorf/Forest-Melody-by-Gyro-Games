using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DecorAddTimeToOfflineAdditionalAnimation : AdditionalDecorEffect
{
    [SerializeField] private Image [] noteImages;
    [SerializeField] private RectTransform [] noteRootRects;

    private bool isPlaying = false;
    private Sequence fadeSeq;
    private Sequence scaleSeq;
    private Sequence moveSeq;
    
    private Vector3[] noteWaypoints;

    
    private void Awake()
    {
        noteWaypoints = new Vector3[]
        {
            new(0, 0),
            new(110, 60),
            new(110, 160)
        };
    }

    private void Update()
    {
        if(!isPlaying) return;
        for (int i = 0; i < noteImages.Length; i++)
        {
            noteImages[i].rectTransform.position = noteRootRects[i].position;
        }
    }

    public override void Play()
    {
        isPlaying = true;
        scaleSeq = DOTween.Sequence();
        moveSeq = DOTween.Sequence();
        fadeSeq = DOTween.Sequence();

        scaleSeq.Insert(1.173f, noteImages[0].rectTransform.DOScale(Vector3.one, 0.5f))
            .Insert(1.9333f, noteImages[1].rectTransform.DOScale(Vector3.one, 0.5f))
            .Insert(2.7667f, noteImages[2].rectTransform.DOScale(Vector3.one, 0.5f));

        moveSeq.Insert(1.073f, noteRootRects[0].DOLocalPath(noteWaypoints, 1))
            .Insert(1.8333f, noteRootRects[1].DOLocalPath(noteWaypoints, 1))
            .Insert(2.6667f, noteRootRects[2].DOLocalPath(noteWaypoints, 1));

        fadeSeq.Insert(1.573f, noteImages[0].DOFade(0, 0.4f))
            .Insert(2.3333f, noteImages[1].DOFade(0, 0.4f))
            .Insert(3.1667f, noteImages[2].DOFade(0, 0.4f));

        moveSeq.OnComplete(Reset);
        
        scaleSeq.Play();
        moveSeq.Play();
        fadeSeq.Play();
    }

    private void Reset()
    {
        for (int i = 0; i < noteImages.Length; i++)
        {
            noteRootRects[i].localPosition = Vector3.zero;
            noteImages[i].rectTransform.localScale = Vector3.zero;
            noteImages[i].color = Color.white;
        }
    }
}
