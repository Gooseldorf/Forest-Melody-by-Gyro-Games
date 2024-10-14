using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using Animation = Spine.Animation;

public class DecorVisual: MonoBehaviour
{
    [SerializeField] private SkeletonGraphic decorSkeleton;
    [SerializeField] private RectTransform visualsRect;
    [SerializeField] private float delayBetweenIdleAnimations;
    
    private Animation decorIdleAnimation;
    private float nextIdleAnimationTime;

    public RectTransform VisualsRect => visualsRect;

    public void Init()
    {
        decorIdleAnimation = decorSkeleton.SkeletonData.FindAnimation("Idle");
        nextIdleAnimationTime = TimeManager.Instance.CurrentTime + delayBetweenIdleAnimations;
    }

    private void Update()
    {
        if (TimeManager.Instance.CurrentTime >= nextIdleAnimationTime)
        {
            PlayIdleAnimation();
            nextIdleAnimationTime = TimeManager.Instance.CurrentTime + delayBetweenIdleAnimations;
        }
    }

    private protected virtual void PlayIdleAnimation()
    {
        decorSkeleton.AnimationState.ClearTrack(1);
        decorSkeleton.AnimationState.SetAnimation(1, decorIdleAnimation, false);
    }
}
