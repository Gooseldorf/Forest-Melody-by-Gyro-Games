using SO_Scripts;
using UnityEngine;

namespace Vizualization
{
    public class Visualizator : MonoBehaviour
    {
        [SerializeField]
        private FreeMultiplierBoosterVisual freeMultiplierComponent;
        [SerializeField]
        private FreeIncomeBoosterVisual freeIncomeComponent;

        private void Awake()
        {         
            Messenger.AddListener(GameEvents.OnFreeMultiplierBoosterActivation, OnFreeMultiplierActivated);
            Messenger.AddListener(GameEvents.OnFreeIncomeBoosterActivation, OnFreeIncomeActivated);
        }

        private void OnDestroy()
        {
            Messenger.RemoveListener(GameEvents.OnFreeMultiplierBoosterActivation, OnFreeMultiplierActivated);
            Messenger.RemoveListener(GameEvents.OnFreeIncomeBoosterActivation, OnFreeIncomeActivated);
        }

        private void OnFreeMultiplierActivated()
        {
            if (freeMultiplierComponent == null) return;
            
            float time = DataHolder.Instance.MultiplierBoosterData.GetDuration(PlayerData.Instance.MultiplierBooster.Level);

            freeMultiplierComponent.Activate(time);
            SoundManager.Instance.PlaySound("MultiplierBooster", time);
        }

        private void OnFreeIncomeActivated()
        {
            if (freeIncomeComponent == null) return;
            
            freeIncomeComponent.Activate();
            SoundManager.Instance.PlaySound("IncomeBooster");
        }
    }
}
