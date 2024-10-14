using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Vizualization
{
    public class FreeMultiplierBoosterVisual : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private float alphaDuration;
        [SerializeField] private UIParticle mainEffect;

        private bool isStarted;
        private float timer;

        public void Activate(float value) 
        {
            timer = value;
            gameObject.SetActive(true);
            background.DOColor(new Color(background.color.r, background.color.g, background.color.b, .15f), alphaDuration)
                .SetEase(Ease.Linear);
            
            if(mainEffect != null)
                mainEffect.Play();
        
            isStarted = true;
        }

        private void Update()
        {
            if (isStarted)
            {
                if (timer > 0)
                    timer -= Time.deltaTime;
                else
                    Deactivate();
            }
        }

        private void Deactivate()
        {
            background.DOColor(new Color(background.color.r, background.color.g, background.color.b, 0f), alphaDuration)
                .SetEase(Ease.Linear).OnComplete((() =>
                {
                    isStarted = false;
                    mainEffect.Stop();
                    gameObject.SetActive(false);
                }));
        }
    }
}
