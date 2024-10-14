using System;
using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Vizualization
{
    public class FreeIncomeBoosterVisual : MonoBehaviour
    {
        [SerializeField] private float effectDuration;
        [SerializeField] private Image rainbow;
        
        private Vector3[] waypoints;
        
        public void Activate()
        {
            gameObject.SetActive(true);

            Sequence rainbowSeq = DOTween.Sequence();

            rainbowSeq.Append(rainbow.DOFade(1, effectDuration/3))
                .AppendInterval(effectDuration/3)
                .Append(rainbow.DOFade(0, effectDuration/3));

            rainbowSeq.OnComplete(Deactivate);
        }

        private  void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
