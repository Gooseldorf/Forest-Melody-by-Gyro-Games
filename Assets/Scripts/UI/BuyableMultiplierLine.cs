using I2.Loc;
using UnityEngine.UIElements;

namespace UI
{
    public class BuyableMultiplierLine
    {
        private BuyableMultiplier buyableMultiplier;
        private Label signLabel;
        private Label bonusLabel;
        private Label durationLabel;
        public VisualElement RootVisualElement;

        public BuyableMultiplierLine(VisualElement rootVisualElement, int multiplier) : this(rootVisualElement)
        {
            signLabel.text = "=";
            bonusLabel.text = "x" + multiplier + " " + LocalizationManager.GetTranslation("Multiplier");
            durationLabel.text = "";
        }

        public BuyableMultiplierLine(VisualElement rootVisualElement, BuyableMultiplier buyableMultiplier, bool showSign = true) : this(rootVisualElement)
        {
            this.buyableMultiplier = buyableMultiplier;
            signLabel.text = showSign ? "+" : "";
            bonusLabel.text = "x" + buyableMultiplier.Multiplier + " " + LocalizationManager.GetTranslation("Multiplier");
            durationLabel.text = Utilities.GetTimeString(buyableMultiplier.Duration - (buyableMultiplier as ICooldownable).TimePassed);
        }

        public BuyableMultiplierLine(VisualElement rootVisualElement)
        {
            RootVisualElement = rootVisualElement;
            signLabel = rootVisualElement.Q("SignLabel") as Label;
            bonusLabel = rootVisualElement.Q("MultiplierLabel") as Label;
            durationLabel = rootVisualElement.Q("DurationLabel") as Label;
        }

        public void UpdateValues()
        {
            if (buyableMultiplier != null)
                durationLabel.text = Utilities.GetTimeString((buyableMultiplier as ICooldownable).TimeLeft);
        }

        public void Dispose()
        {
            buyableMultiplier = null;
            RootVisualElement.Clear();
        }
    }
}
