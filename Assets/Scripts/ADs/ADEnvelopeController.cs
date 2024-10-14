using System.Collections.Generic;
using Controllers;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ADEnvelopeController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Button button;
    [SerializeField] private float repeatInterval;
    [SerializeField] private float inOutTime;
    [SerializeField] private float idleTime;
    [SerializeField] private float horizontalOffset;
    [SerializeField] private float verticalOffset;
    [SerializeField] private float borderDistance;
    
    [FoldoutGroup("Notes for AD"), SerializeField] private float multiplier;
    [FoldoutGroup("Notes for AD"), SerializeField] private float term;
    
    private RectTransform targetRect;
    private Vector3 idlePosition;
    private bool inOutPositionFlag;

    private float startTime;
    private bool isPlaying;

    private Sequence idleSequence;
    private Tweener inTweener;
    private Tweener outTweener;

    private List<BranchController> visibleBranches = new();

    private bool RandomBool => Random.value > 0.5f;

    private void Start()
    {
        startTime = TimeManager.Instance.CurrentTime + repeatInterval;
        GetVisibleBranches();
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (TimeManager.Instance.CurrentTime < startTime || isPlaying) return;
        if (!ADsManager.Instance.RewardedUnitsDict[RewardedADUnits.Envelope].IsLoaded)
        {
            startTime = TimeManager.Instance.CurrentTime + repeatInterval;
            return;
        }
        InAnimation();
    }

    private void OnClick()
    {
        double reward = CalculateADReward();
        Messenger<UI.UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UI.UIManager.PanelType.AdOffer, reward, MessengerMode.DONT_REQUIRE_LISTENER);
    }

    public void Toggle(bool isActive) => gameObject.SetActive(isActive);

    private double CalculateADReward()
    {
        double result = multiplier * PlayerData.Instance.CurrentTree.CalculateCleanIncomeInSecond() + term;
        return Utilities.RoundOff(result, 10);
    }

    private void InAnimation()
    {
        Toggle(true);
        button.interactable = true;
        isPlaying = true;
        inOutPositionFlag = RandomBool;
        GetVisibleBranches();
        bool getBranchFlag = false;
        while (!getBranchFlag)
        {
            BranchController targetBranch = visibleBranches[Random.Range(1, visibleBranches.Count - 1)];
            if (targetBranch.IsLocked) continue;
            getBranchFlag = true;
            targetRect = targetBranch.GetComponent<RectTransform>();
            transform.SetParent(targetRect);
            rect.localPosition = GetPointOutsideScreen(targetRect.position);
            float x = Random.Range(-horizontalOffset, horizontalOffset);
            idlePosition = new Vector3(x, 0);
            inTweener = rect.DOLocalMove(idlePosition, inOutTime).OnComplete(StartIdleAnimation);
            inTweener.Play();
        }
    }

    private void StartIdleAnimation()
    {
        Tweener scaleTween = rect.DOScale(1.5f, 0.5f);
        Tweener rotationTween = rect.DOBlendableRotateBy(new Vector3(0, 0, 60), 0.25f)
            .SetEase(Ease.InOutSine)
            .SetLoops(4, LoopType.Yoyo);
        Tweener scaleBackTween = rect.DOScale(1.0f, 0.5f);

        float loopDuration = 2;

        idleSequence = DOTween.Sequence();
        idleSequence.Append(scaleTween)
            .Append(rect.DOLocalRotate(new Vector3(0,0,-30), 0.25f))
            .Append(rotationTween)
            .Append(rect.DOLocalRotate(new Vector3(0,0,0), 0.25f))
            .Append(scaleBackTween);
        idleSequence.SetLoops((int)(idleTime / loopDuration));
        idleSequence.OnComplete(OutAnimation);
        idleSequence.OnKill(() =>
        {
            rect.DOLocalRotate(new Vector3(0, 0, 0), 0.25f);
            rect.DOScale(1, 0.5f);
        });

        idleSequence.Play();
    }

    private void OutAnimation()
    {
        CleanTweeners();
        outTweener = rect.DOLocalMove(GetPointOutsideScreen(targetRect.position), inOutTime).OnComplete((() => isPlaying = false));
        outTweener.Play();
        startTime = TimeManager.Instance.CurrentTime + repeatInterval;
    }

    private void CleanTweeners()
    {
        if (inTweener != null) inTweener.Kill();
        if (idleSequence != null) idleSequence.Kill();
        if (outTweener != null) outTweener.Kill();
    }

    private void GetVisibleBranches()
    {
        visibleBranches.Clear();
        foreach (RectTransform child in scrollRect.content.transform)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(scrollRect.viewport, new Vector2(child.position.x, child.position.y)))
            {
                visibleBranches.Add(child.GetComponent<BranchController>());
            }
        }
    }

    private Vector3 GetPointOutsideScreen(Vector3 parentPosition)
    {
        float x = inOutPositionFlag ? -borderDistance : Screen.width + borderDistance;
        inOutPositionFlag = !inOutPositionFlag;
        float y = Random.Range(parentPosition.y - verticalOffset, parentPosition.y + verticalOffset);

        Vector3 globalPosition = new Vector3(x, y, 0f);
        Vector3 localPosition = rect.parent.InverseTransformPoint(globalPosition);

        return localPosition;
    }
}
