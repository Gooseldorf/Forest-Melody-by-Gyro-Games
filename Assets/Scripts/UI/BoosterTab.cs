using I2.Loc;
using SO_Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BoosterTab : BottomTab
    {
        [SerializeField]
        private VisualTreeAsset buyableWidget;

        private Label inventoryTitle;

        private VisualElement buyableScrollView;

        private SquareWidget shopButton;
        private List<SquareWidget> buyableButtons = new();

        private SquareWidget reloadButton;
        private SquareWidget multiplierButton;
        private SquareWidget tapButton;
        private SquareWidget incomeButton;

        private BoosterWidget multiplierWidget;
        private BoosterWidget tapWidget;
        private BoosterWidget incomeWidget;
        private OfflineBoosterWidget offlineBoosterWidget;

        public override void SetUpTab(VisualElement tab, VisualElement button)
        {
            base.SetUpTab(tab, button);

            inventoryTitle = tab.Q("InventoryLabel") as Label;

            buyableScrollView = tab.Q("ByableList").Q("unity-content-container");
            buyableScrollView.style.flexDirection = FlexDirection.RowReverse;
            buyableScrollView.style.alignContent = Align.FlexEnd;

            shopButton = new SquareWidget(tab.Q("ShopButton"));
            shopButton.RegisterCallback(() => Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UIManager.PanelType.Shop, null, MessengerMode.DONT_REQUIRE_LISTENER));
            shopButton.SetText(LocalizationManager.GetTranslation("Shop"));
            shopButton.SetBgColor(UIHelper.Instance.GoodsCrystalBgColor);
            shopButton.SetImages(UIHelper.Instance.GetSprite("Shop_Image"), UIHelper.Instance.GoodsBg, UIHelper.Instance.GoodsCrystalSpyralColor);

            reloadButton = new SquareWidget(tab.Q("ReloadButton"));
            reloadButton.RootVisualElement.RegisterCallback<ClickEvent>(OnReloadButtonClick);
            reloadButton.SetImages(UIHelper.Instance.GetSprite("ReloadBoosters"));
			UpdateReloadButton( RewardedADUnits.ReloadBoosters, false);
            Messenger<RewardedADUnits,bool>.AddListener(GameEvents.IsRewardedADLoaded, UpdateReloadButton);          

            multiplierButton = new SquareWidget(tab.Q("MultiplierButton"));
            multiplierButton.SetBgColor(UIHelper.Instance.MultiplierBoosterBgColor);
            multiplierButton.SetImages(UIHelper.Instance.GetSprite("Multiplier_Image"));
            multiplierButton.RegisterCallback(() =>
                {
                    if ((PlayerData.Instance.MultiplierBooster as ICooldownable).IsReady)
                    {
                        PlayerData.Instance.MultiplierBooster.Activate();
                        Messenger.Broadcast(GameEvents.OnFreeMultiplierBoosterActivation, MessengerMode.DONT_REQUIRE_LISTENER);
                    }
                });

            tapButton = new SquareWidget(tab.Q("TapButton"));
            tapButton.SetBgColor(UIHelper.Instance.TapBoosterBgColor);
            tapButton.SetImages(UIHelper.Instance.GetSprite("Tap_Image"));
            tapButton.RegisterCallback(() =>
            {
                if ((PlayerData.Instance.TapBooster as ICooldownable).IsReady)
                    PlayerData.Instance.TapBooster.Activate();
            });

            incomeButton = new SquareWidget(tab.Q("IncomeButton"));
            incomeButton.SetBgColor(UIHelper.Instance.IncomeBoosterBgColor);
            incomeButton.SetImages(UIHelper.Instance.GetSprite("Income_Image"));
            incomeButton.RegisterCallback(() =>
            {
                if ((PlayerData.Instance.IncomeBooster as ICooldownable).IsReady)
                {
                    PlayerData.Instance.IncomeBooster.Activate();
                    Messenger.Broadcast(GameEvents.OnFreeIncomeBoosterActivation, MessengerMode.DONT_REQUIRE_LISTENER);
                }
            });

//#if UNITY_EDITOR
//            ScrollView scrollView = tab.Q("ByableList") as ScrollView;
//            scrollView.RegisterCallback<WheelEvent>((evt) =>
//            {
//                scrollView.scrollOffset = new Vector2(0, scrollView.scrollOffset.y + 100 * evt.delta.y);
//                evt.StopPropagation();
//            }
//             );
//#endif
            OnUpdateLocalization();
        }

        public override void DisposeTab()
        {
            base.DisposeTab();
            shopButton.Dispose();
            multiplierButton.Dispose();
            tapButton.Dispose();
            incomeButton.Dispose();
            reloadButton.RootVisualElement.UnregisterCallback<ClickEvent>(OnReloadButtonClick);
            Messenger<RewardedADUnits, bool>.RemoveListener(GameEvents.IsRewardedADLoaded, UpdateReloadButton);
        }

        private void UpdateReloadButton(RewardedADUnits adName, bool isADLoaded)
        {
            if(adName != RewardedADUnits.ReloadBoosters) return;
            if (isADLoaded)
            {
                reloadButton.SetText(LocalizationManager.GetTranslation("Ready")); //"ReloadBoosters"
                reloadButton.BottomPart.style.backgroundColor = new StyleColor(UIHelper.Instance.UpgradeActiveColor);
            }
            else
            {
                reloadButton.SetText(LocalizationManager.GetTranslation(""));
                reloadButton.BottomPart.style.backgroundColor = new StyleColor(UIHelper.Instance.InactiveColor);
            }
        }

        private void OnReloadButtonClick(ClickEvent evt)
        {
            if (reloadButton.IsReady)
            {
                ADsManager.Instance.ShowRewardedAd(RewardedADUnits.ReloadBoosters, ReloadBoosters);
            }
        }

        private void ReloadBoosters()
        {
            if (!multiplierButton.IsReady)
                PlayerData.Instance.MultiplierBooster.ResetCd();
            if (!tapButton.IsReady)
                PlayerData.Instance.TapBooster.ResetCd();
            if (!incomeButton.IsReady)
                PlayerData.Instance.IncomeBooster.ResetCd();
        }

        public override void UpdateValues()
        {
            base.UpdateValues();

            UpdateButton(multiplierButton, PlayerData.Instance.MultiplierBooster);
            UpdateButton(tapButton, PlayerData.Instance.TapBooster);
            UpdateButton(incomeButton, PlayerData.Instance.IncomeBooster);

            CheckBuyablesBoosters();
        }

        private void UpdateButton(SquareWidget button, ICooldownable booster)
        {
            if (booster.IsReady)
            {
                if (!button.IsReady)
                    button.SetText(LocalizationManager.GetTranslation("Ready"));
            }
            else
                button.SetTimer(booster.TimePassed, booster.Duration + booster.Cd);
        }

        private void CheckBuyablesBoosters()
        {
            if (buyableButtons.Count != PlayerData.Instance.Inventory.Count)
            {
                SquareWidget buyableButton;
                VisualElement buyable;

                buyableButtons.ForEach(x =>
                {
                    x.Dispose();
                    x.RootVisualElement.Clear();
                    buyableScrollView.Remove(x.RootVisualElement);
                    //x.RootVisualElement.style.display = DisplayStyle.None;
                });
                buyableButtons.Clear();

                for (int i = 0; i < PlayerData.Instance.Inventory.Count; i++)
                {
                    buyable = buyableWidget.CloneTree();
                    buyable.style.paddingLeft = buyable.style.paddingRight = 9;
                    buyableScrollView.Add(buyable);
                    buyableButton = new SquareWidget(buyable);
                    buyableButtons.Add(buyableButton);

                    string buyableId = PlayerData.Instance.Inventory[i];
                    if (DataHolder.Instance.ExistBuyableMultiplierData(buyableId))
                    {
                        buyableButton.SetBgColor(UIHelper.Instance.GoodsMultiplierBgColor);
                        buyableButton.SetImages(UIHelper.Instance.GetSprite(buyableId), UIHelper.Instance.GoodsBg, UIHelper.Instance.GoodsMultiplierSpyralColor);
                        buyableButton.SetText(LocalizationManager.GetTranslation("UseNow"));
                        buyableButton.RegisterCallback(() => ActivateBuyableMultiplier(buyableId));

                    }
                    if (DataHolder.Instance.ExistBuyableSkipTimeData(buyableId))
                    {
                        buyableButton.SetBgColor(UIHelper.Instance.GoodsTimeJumpBgColor);
                        buyableButton.SetImages(UIHelper.Instance.GetSprite(buyableId), UIHelper.Instance.TimeJumpBg, UIHelper.Instance.GoodsTimeJumpSpyralColor);
                        buyableButton.SetText(LocalizationManager.GetTranslation("UseNow"));
                        buyableButton.RegisterCallback(() => ActivateTimeSkipMultiplier(buyableId));
                    }
                }
            }
        }

        private void ActivateBuyableMultiplier(string byableId)
        {
            BuyableMultiplier buyable = new BuyableMultiplier(byableId);
            buyable.Activate();
        }

        private void ActivateTimeSkipMultiplier(string byableId)
        {
            DataHolder.Instance.GetBuyableSkipTimeData(byableId).Activate();
        }

        public void Init()
        {
            multiplierWidget = new BoosterWidget(tab.Q("MultiplierBoosterWidget"), PlayerData.Instance.MultiplierBooster);
            multiplierWidget.SetBgColor(UIHelper.Instance.MultiplierBoosterBgColor);
            multiplierWidget.SetImage(UIHelper.Instance.GetSprite("Multiplier_Image"));

            tapWidget = new BoosterWidget(tab.Q("TapBoosterWidget"), PlayerData.Instance.TapBooster);
            tapWidget.SetBgColor(UIHelper.Instance.TapBoosterBgColor);
            tapWidget.SetImage(UIHelper.Instance.GetSprite("Tap_Image"));

            incomeWidget = new BoosterWidget(tab.Q("IncomeBoosterWidget"), PlayerData.Instance.IncomeBooster);
            incomeWidget.SetBgColor(UIHelper.Instance.IncomeBoosterBgColor);
            incomeWidget.SetImage(UIHelper.Instance.GetSprite("Income_Image"));

            offlineBoosterWidget = new OfflineBoosterWidget(tab.Q("OfflineBoosterWidget"));
            offlineBoosterWidget.SetImage(UIHelper.Instance.GetSprite("OfflineBooster"));

            widgets.Add(multiplierWidget);
            widgets.Add(tapWidget);
            widgets.Add(incomeWidget);
            widgets.Add(offlineBoosterWidget);

            OnUpdateLocalization();
        }

        protected override void OnUpdateLocalization()
        {
            title.text = LocalizationManager.GetTranslation("FreeBoosters");
            inventoryTitle.text = LocalizationManager.GetTranslation("Inventory");
            buyableButtons.ForEach(x => x.SetText(LocalizationManager.GetTranslation("UseNow")));
        }
    }
}
