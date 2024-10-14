using I2.Loc;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class EarningsPanel : MonoBehaviour
    {
        [SerializeField]
        private int adMultiplier = 3;

        private VisualElement earningsPanel;
        private Label titleLabel;
        private Label currancyLabel;
        private VisualElement button;
        private Label buttonLabel;
        private VisualElement adButton;
        private Label adButtonLabel;
        private Label adButtonLabel2;

        private double earningAmount;
        private RewardedADUnits currentAdName;

        public bool IsShown => earningsPanel.visible;

        public void SetUpEarningsPanel(VisualElement earningsPanel)
        {
            this.earningsPanel = earningsPanel;
            titleLabel = earningsPanel.Q("TitleLabel") as Label;
            currancyLabel = earningsPanel.Q("GainLabel") as Label;
            button = earningsPanel.Q("GainButton");
            buttonLabel = earningsPanel.Q("GainButtonLabel") as Label;
            adButton = earningsPanel.Q("AdButton");
            adButtonLabel = earningsPanel.Q("AdGainLabel") as Label;
            adButtonLabel2 = earningsPanel.Q("AdGainLabel2") as Label;
            button.RegisterCallback<ClickEvent>(OnCollectButtonClick);
            adButton.RegisterCallback<ClickEvent>(OnAdButtonClick);
            Messenger<RewardedADUnits, bool>.AddListener(GameEvents.IsRewardedADLoaded, UpdateADButton); 
            Show(false);
        }

        private void UpdateADButton(RewardedADUnits adName, bool ADisLoaded)
        {
            if(adName is not (RewardedADUnits.LevelUpMultiply or RewardedADUnits.OfflineIncomeMultiply)) return;
                
            adButton.style.backgroundColor = ADisLoaded
                ? new StyleColor(UIHelper.Instance.DecorActiveColor)
                : new StyleColor(UIHelper.Instance.InactiveColor);
        }

        public void DisposeEarningsPanel()
        {
            button.UnregisterCallback<ClickEvent>(OnCollectButtonClick);
            adButton.UnregisterCallback<ClickEvent>(OnAdButtonClick);
            Messenger<RewardedADUnits, bool>.RemoveListener(GameEvents.IsRewardedADLoaded, UpdateADButton);
        }

        private void OnCollectButtonClick(ClickEvent evt)
        {
            PlayerData.Instance.ChangeNotes(earningAmount);
            SoundManager.Instance.PlaySound("MenuClick");
            Show(false);
        }

        private void OnAdButtonClick(ClickEvent evt)
        {
            if(!ADsManager.Instance.RewardedUnitsDict[currentAdName].IsLoaded) return;
            SoundManager.Instance.PlaySound("MenuClick");
            ADsManager.Instance.ShowRewardedAd(currentAdName, () => PlayerData.Instance.ChangeNotes(earningAmount * adMultiplier));
            Show(false);
        }

        public void Init(double earningAmount, string title, RewardedADUnits adName)
        {
            this.earningAmount = earningAmount;
            titleLabel.text = title;
            currancyLabel.text = Utilities.GetNotesString(earningAmount);
            buttonLabel.text = LocalizationManager.GetTranslation("Collect");
            adButtonLabel.text = LocalizationManager.GetTranslation("Collect") + " x" + adMultiplier;
            adButtonLabel2.text = LocalizationManager.GetTranslation("ByWatchingAd");
            currentAdName = adName;
            Show(true);
        }

        public void Show(bool show) => earningsPanel.visible = show;
    }
}
