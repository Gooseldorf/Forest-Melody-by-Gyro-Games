using UnityEngine;

namespace Vizualization
{
    public class DecorWithAdditionalEffects: DecorVisual
    {
        [SerializeField] private AdditionalDecorEffect additionalDecorEffect;

        private protected override void PlayIdleAnimation()
        {
            base.PlayIdleAnimation();
            additionalDecorEffect.Play();
        }
    }
}