using I2.Loc;
using SO_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class AfterPurchaseWindow : MonoBehaviour
    {
        private VisualElement root;

        private Label titleLabel;
        private VisualElement image;
        private Label textLabel;
        private VisualElement laterButton;
        private Label laterLabel;
        private VisualElement useButton;
        private Label useLabel;

        private string byableId;

        public void SetUpAfterPurchaseWindow(VisualElement root)
        {
            this.root = root;
            titleLabel = root.Q("TitleLabel") as Label;
            image = root.Q("Image");
            textLabel = root.Q("TextLabel") as Label;
            laterButton = root.Q("LaterButton");
            laterLabel = root.Q("LaterButtonLabel") as Label;
            useButton = root.Q("UseButton");
            useLabel = root.Q("UseButtonLabel") as Label;

            laterButton.RegisterCallback<ClickEvent>(OnLaterButtonClick);
            useButton.RegisterCallback<ClickEvent>(OnUseButtonClick);

            Show(false);
        }

        public void DisposeAfterPurchaseWindow()
        {
            laterButton.UnregisterCallback<ClickEvent>(OnLaterButtonClick);
            useButton.UnregisterCallback<ClickEvent>(OnUseButtonClick);
        }

        private void OnLaterButtonClick(ClickEvent evt) => Show(false);

        private void OnUseButtonClick(ClickEvent evt)
        {
            BuyableSkipTimeData timeSkip = DataHolder.Instance.GetBuyableSkipTimeData(byableId);
            if (timeSkip != null)
                timeSkip.Activate();

            if (DataHolder.Instance.ExistBuyableMultiplierData(byableId))
            {
                BuyableMultiplier buyable = new BuyableMultiplier(byableId);
                buyable.Activate();
            }

            Show(false);
        }

        public void Init(Pair<string, string> info)
        {
            titleLabel.text = LocalizationManager.GetTranslation("ItemPurchased");
            laterLabel.text = LocalizationManager.GetTranslation("Later");
            useLabel.text = LocalizationManager.GetTranslation("UseNow");

            byableId = info.Value1;
            textLabel.text = info.Value2;

            PlayerData.Instance.AddToInventory(byableId);

            image.style.backgroundImage = new StyleBackground(UIHelper.Instance.GetSprite(byableId));

            Show(true);
        }

        public void Show(bool show) => root.visible = show;
    }
}
