using System.Collections;
using Coffee.UIExtensions;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Animation = Spine.Animation;

public class BirdVisual : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform stageVisuals;
    [SerializeField] private GameObject nestVisuals;
    [SerializeField] private Image babyInNestImage;
    [SerializeField] private Image babyImage;
    [SerializeField] private NoteVisual noteVisual;
    [SerializeField] private RectTransform noteVisualRoot;
    [SerializeField] private RectTransform babyNoteVisualRoot;
    [SerializeField] private TextMeshProUGUI incubationTimerTMP;
    [SerializeField] private Slider incubationSlider;
    [SerializeField] private Transform tapEffectRoot;
    [SerializeField] private float minVisualEffectInterval;
    [SerializeField] private SkeletonGraphic eggSkeleton;
    [SerializeField] private UIParticle hatchParticles;
    [SerializeField] private SkeletonGraphic birdSkeleton;

    [FoldoutGroup("VisualSettings"), SerializeField] private float stageScaleMultiplier;
    [FoldoutGroup("VisualSettings"), SerializeField] private float notesScaleDuration;
    [FoldoutGroup("VisualSettings"), SerializeField] private float notesMoveYDuration;
    [FoldoutGroup("VisualSettings"), SerializeField] private float notesFadeDuration;

    private int currentBirdLevel;
    private bool isInitialized;
    private float lastNotesTime;
    private float lastTapTime;
    private float shakeTimer;
    private Animation birdSpineAnimation;

    public RectTransform StageVisuals => stageVisuals;

    public void Init(Bird bird)
    {
        isInitialized = false;
        UpdateStage(0, bird.Level);
        isInitialized = true;
        shakeTimer = Random.Range(3, 10);
    }

    public void UpdateStage(int previousLevel, int birdLevel)
    {
        if((birdLevel % 10 == 0 && birdLevel > 1) || birdLevel-previousLevel == 10) ShowNextStageEffects(birdLevel);
        switch (birdLevel)
        {
            case <= 0:
                ShowEgg();
                break;
            case 1:
                ShowHatch();
                ShowBabyInNest();
                break;
            case >= 2 and < 10:
                ShowBabyInNest();
                break;
            case >= 11 and < 20:
                ShowBaby();
                break;
            case >= 20 and < 50:
                ShowBird();
                break;
            case 50:
                ShowBird();
                ShowFinalStage();
                break;
        }
        currentBirdLevel = birdLevel;
        UpdateNoteVisualRoot();
    }

    private void ShowEgg()
    {
        nestVisuals.SetActive(true);
        eggSkeleton.enabled = true;
        birdSkeleton.enabled = false;
        incubationTimerTMP.enabled = true;
        incubationSlider.gameObject.SetActive(true);
    }

    public void DisplayIncubationTime(float timeLeft, float percent)
    {
        incubationTimerTMP.text = Utilities.GetTimeString((int)timeLeft);
        incubationSlider.value = percent;
        AnimateIncubation(percent);
    }

    private void AnimateIncubation(float percent)
    {
        Vector3 test = Vector3.one - eggSkeleton.rectTransform.localScale;
        eggSkeleton.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -30 * (1 - percent)));
        eggSkeleton.rectTransform.localScale = new Vector3(0.7f,0.7f,0.7f) +  test * percent;
        shakeTimer -= Time.deltaTime;
        if (shakeTimer <= 0)
        {
            eggSkeleton.rectTransform.DOPunchPosition(new Vector3(Random.Range(-2, 2), Random.Range(-2, 2)), 2);
            shakeTimer = Random.Range(3, 10);
        }
    }

    private void ShowHatch()
    {
        HideIncubationTime();
        hatchParticles.Play();
        
        Sequence hatchSeq = DOTween.Sequence();
        hatchSeq.AppendInterval(0.7f)
            .Append(eggSkeleton.DOFade(0, 0.5f));
        hatchSeq.OnComplete(ShowBabyInNest);

        eggSkeleton.AnimationState.SetAnimation(1,"Crack", false);
        
        if(isInitialized) SoundManager.Instance.PlaySound("Hatch");
    }
    
    private void HideIncubationTime()
    {
        incubationTimerTMP.enabled = false;
        incubationSlider.gameObject.SetActive(false);
    }
    
    private void ShowBabyInNest()
    {
        birdSkeleton.enabled = false;
        nestVisuals.SetActive(true); 
        babyInNestImage.enabled = true;
        noteVisual.ChangeStartPosition(babyNoteVisualRoot);
    }

    private void ShowBaby()
    {
        birdSkeleton.enabled = false;
        nestVisuals.SetActive(false); 
        babyInNestImage.enabled = false;
        babyImage.enabled = true;
        noteVisual.ChangeStartPosition(babyNoteVisualRoot);
    }
    
    private void ShowBird()
    {
        babyImage.enabled = false;
        birdSkeleton.enabled = true;
        UpdateNoteVisualRoot();        
        transform.parent.SetAsFirstSibling();
        birdSpineAnimation = birdSkeleton.SkeletonData.FindAnimation("GetNotes");
        birdSkeleton.AnimationState.SetAnimation(1, birdSpineAnimation,false);
    }

    public IEnumerator ShowBirdAnimation()
    {
        yield return new WaitForSeconds(0.7f);
        birdSkeleton.AnimationState.AddAnimation(1, birdSpineAnimation,false, 0);
    }

    private void ShowFinalStage(){}

    public void DisplayNotesText(double notes)
    {
        if(TimeManager.Instance.CurrentTime - lastNotesTime < minVisualEffectInterval)
            return;
        lastNotesTime = TimeManager.Instance.CurrentTime;
        NotesText notesText = NotesTextPool.Instance.NotesTextObjPool.Get();
        notesText.TMP.text = "+" + Utilities.GetNotesString(notes);
        notesText.transform.localScale = Vector3.zero;
        notesText.CanvasGroup.alpha = 1;
        notesText.transform.SetParent(transform);
        notesText.transform.localPosition = new Vector3(0, rectTransform.sizeDelta.y / 2, 0);
        Sequence notesSeq = DOTween.Sequence();
        notesSeq.Append(notesText.transform.DOScale(1, notesScaleDuration).SetEase(Ease.OutBack))
            .Insert(0.1f, notesText.transform.DOLocalMoveY(rectTransform.sizeDelta.y * 1, notesMoveYDuration))
            .Insert(0.5f,notesText.CanvasGroup.DOFade(0, notesFadeDuration));
        notesSeq.OnComplete(() =>
        {
            NotesTextPool.Instance.NotesTextObjPool.Release(notesText);
        });
        notesSeq.Play();
    }

    public void ShowNotesVisual()
    {
        noteVisual.Play();
    }

    public void UpdateNoteVisualRoot()
    {
        noteVisual.ChangeStartPosition(currentBirdLevel >= 20 ? noteVisualRoot : babyNoteVisualRoot);
    }

    private void ShowNextStageEffects(int birdLevel)
    {
        if(isInitialized) SoundManager.Instance.PlaySound("BirdNextStage");
        ScaleNextStage(birdLevel);
    }
    
    private void ScaleNextStage(int birdLevel)
    {
        float oldHeight = birdSkeleton.rectTransform.rect.height;
        birdSkeleton.transform.localScale = Vector3.one * 0.9f + Vector3.one * ((birdLevel / 10f - 2) * stageScaleMultiplier);
        float newHeight = birdSkeleton.rectTransform.rect.height;
        float heightDiff = newHeight - oldHeight;
        Vector2 pos = birdSkeleton.rectTransform.anchoredPosition;
        birdSkeleton.rectTransform.anchoredPosition = new Vector2(pos.x, pos.y + heightDiff);
    }

    public void ShowTapEffect()
    {
        if(TimeManager.Instance.CurrentTime - lastTapTime < minVisualEffectInterval)
            return;
        lastTapTime = TimeManager.Instance.CurrentTime;
        EffectVisualManager.Instance.PlayTapEffect(tapEffectRoot, transform);
    }
}