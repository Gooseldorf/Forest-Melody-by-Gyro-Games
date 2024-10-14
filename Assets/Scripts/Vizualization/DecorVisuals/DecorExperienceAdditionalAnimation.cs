using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Vizualization
{
    public class DecorExperienceAdditionalAnimation: AdditionalDecorEffect
    {
        [SerializeField] private Image glowEffectImage;
        
        public override void Play()
        {
            Sequence glowSeq = DOTween.Sequence();

            glowSeq.Append(glowEffectImage.DOFade(1, 1.67f))
                .Append(glowEffectImage.DOFade(0.7f, 1.67f))
                .Append(glowEffectImage.DOFade(1, 1.67f))
                .Append(glowEffectImage.DOFade(0.7f, 1.67f))
                .Insert(0, glowEffectImage.rectTransform.DORotate(new Vector3(0,0,40), 6.67f));
            
            glowSeq.OnComplete(() =>
            {
                glowEffectImage.DOFade(0, 3);
            });
            glowSeq.Play();

        }
    }
}