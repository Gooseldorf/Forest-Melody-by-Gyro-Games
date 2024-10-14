using System;
using System.Collections;
using I2.Loc;
using Sirenix.OdinInspector;
using SO_Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public partial class UIManager : MonoBehaviour
    {
        public enum PanelType { Shop, OfflineEarnings, LevelUpEarnings, PurchaseWindow, BottomPanel, MultipliersDescription, Settings, AdOffer }

        [SerializeField]
        private float uiUpdateDelay = .1f;
        private float lastUpdate;

        [SerializeField, BoxGroup("UI Documents")]
        private UIDocument loadingPanelUI;
        [SerializeField, BoxGroup("UI Documents")]
        private UIDocument treePanelUI;
        [SerializeField, BoxGroup("UI Documents")]
        private UIDocument topPanelUI;
        [SerializeField, BoxGroup("UI Documents")]
        private UIDocument shopPanelUI;
        [SerializeField, BoxGroup("UI Documents")]
        private UIDocument earningsPanelUI;
        [SerializeField, BoxGroup("UI Documents")]
        private UIDocument afterPurchaseWindowUI;
        [SerializeField, BoxGroup("UI Documents")]
        private UIDocument settingsPanelUI;
        [SerializeField, BoxGroup("UI Documents")]
        private UIDocument adWindowUI;

        [SerializeField, BoxGroup("UI Scripts")]
        private TopPanel topPanel;
        [SerializeField, BoxGroup("UI Scripts")]
        private BottomPanel bottomPanel;
        [SerializeField, BoxGroup("UI Scripts")]
        private ShopPanel shopPanel;
        [SerializeField, BoxGroup("UI Scripts")]
        private EarningsPanel earningsPanel;
        [SerializeField, BoxGroup("UI Scripts")]
        private AfterPurchaseWindow afterPurchaseWindow;
        [SerializeField, BoxGroup("UI Scripts")]
        private BuyableMultipliersInfo multipliersInfo;
        [SerializeField, BoxGroup("UI Scripts")]
        private SettingsPanel settingsPanel;
        [SerializeField, BoxGroup("UI Scripts")]
        private AdWindow adWindow;

        [SerializeField]
        private PanelSettings uiSettings;

        private bool wasInited = false;
        private bool isEditMode = false;

        private VisualElement freeSpace;
        private VisualElement shopExitButton;
        private VisualElement shopButton;
        private VisualElement settingsExitButton;
        private VisualElement eidtButton;
        private VisualElement doneButton;
        private VisualElement adExitButton;

        private VisualElement birdButton;
        private VisualElement boosterButton;
        private VisualElement decorButton;

        private void Awake()
        {
            topPanel.SetUpTopPanel(topPanelUI.rootVisualElement);

            bottomPanel.SetUpBottomPanel(treePanelUI.rootVisualElement.Q("BottomPanel"));

            shopPanel.SetUpShopPanel(shopPanelUI.rootVisualElement);

            earningsPanel.SetUpEarningsPanel(earningsPanelUI.rootVisualElement);

            multipliersInfo.SetUpBuyableMultipliersInfo(treePanelUI.rootVisualElement.Q("BoostersInfo"));

            afterPurchaseWindow.SetUpAfterPurchaseWindow(afterPurchaseWindowUI.rootVisualElement);

            settingsPanel.SetUpSettingsPanel(settingsPanelUI.rootVisualElement);

            adWindow.SetUpAdWindow(adWindowUI.rootVisualElement);

            freeSpace = treePanelUI.rootVisualElement.Q("FreeSpace");
            freeSpace.RegisterCallback<ClickEvent>(DeselectAll);

            shopExitButton = shopPanelUI.rootVisualElement.Q("ExitButton");
            shopExitButton.RegisterCallback<ClickEvent>(CloseShop);

            settingsExitButton = settingsPanelUI.rootVisualElement.Q("CloseButton");
            settingsExitButton.RegisterCallback<ClickEvent>(CloseSettings);

            shopButton = treePanelUI.rootVisualElement.Q("ShopButton");
            shopButton.RegisterCallback<ClickEvent>(ShowShop);

            eidtButton = topPanelUI.rootVisualElement.Q("EditButton");
            eidtButton.RegisterCallback<ClickEvent>(EnterEditMode);

            doneButton = topPanelUI.rootVisualElement.Q("DoneButton");
            doneButton.RegisterCallback<ClickEvent>(ExitEditMode);

            adExitButton = adWindowUI.rootVisualElement.Q("CancelButton");
            adExitButton.RegisterCallback<ClickEvent>(CloseAdOffer);


            birdButton = treePanelUI.rootVisualElement.Q("BottomPanel").Q("BirdsButton");
            birdButton.RegisterCallback<ClickEvent>(BirdButtonClick);

            boosterButton = treePanelUI.rootVisualElement.Q("BottomPanel").Q("BoostsButton");
            boosterButton.RegisterCallback<ClickEvent>(BoosterButtonClick);

            decorButton = treePanelUI.rootVisualElement.Q("BottomPanel").Q("DecorsButton");
            decorButton.RegisterCallback<ClickEvent>(DecorButtonClick);

            Messenger<PanelType, object>.AddListener(GameEvents.ShowPanel, OnShowPanel);
            Messenger<Bird>.AddListener(GameEvents.PlantBird, DeselectAll);
            Messenger<Decor>.AddListener(GameEvents.PlantDecor, DeselectAll);

            StartCoroutine(SimpleTimer(1, HideLoading));
        }
        private void OnDestroy()
        {
            freeSpace.UnregisterCallback<ClickEvent>(DeselectAll);
            shopExitButton.UnregisterCallback<ClickEvent>(CloseShop);
            settingsExitButton.UnregisterCallback<ClickEvent>(CloseSettings);
            shopButton.UnregisterCallback<ClickEvent>(ShowShop);
            eidtButton.UnregisterCallback<ClickEvent>(EnterEditMode);
            doneButton.UnregisterCallback<ClickEvent>(ExitEditMode);
            adExitButton.UnregisterCallback<ClickEvent>(CloseAdOffer);

            birdButton.UnregisterCallback<ClickEvent>(BirdButtonClick);
            boosterButton.UnregisterCallback<ClickEvent>(BoosterButtonClick);
            decorButton.UnregisterCallback<ClickEvent>(DecorButtonClick);

            topPanel.DisposeTopPanel();
            bottomPanel.DisposeBottomPanel();
            shopPanel.DisposeShopPanel();
            earningsPanel.DisposeEarningsPanel();
            afterPurchaseWindow.DisposeAfterPurchaseWindow();
            multipliersInfo.DisposeBuyableMultipliersInfo();
            settingsPanel.DisposeSettingsPanel();
            adWindow.DisposeAdWindow();

            Messenger<PanelType, object>.RemoveListener(GameEvents.ShowPanel, OnShowPanel);
            Messenger<Bird>.RemoveListener(GameEvents.PlantBird, DeselectAll);
            Messenger<Decor>.RemoveListener(GameEvents.PlantDecor, DeselectAll);
        }

        public void Init(Tree tree)
        {
            lastUpdate = 0;

            bottomPanel.Init(tree);

            shopPanel.Init();

            multipliersInfo.Init();

            wasInited = true;
        }

        private void Update()
        {
            if (!wasInited) return;

            if (Time.realtimeSinceStartup - lastUpdate > uiUpdateDelay)
            {
                topPanel.UpdateValues(isEditMode);
                bottomPanel.UpdateValues();
                multipliersInfo.UpdateValues();

                lastUpdate = Time.realtimeSinceStartup;
            }
        }

        private void OnShowPanel(PanelType panelType, object args = null)
        {
            multipliersInfo.HideDesc();

            if (isEditMode)
                ExitEditMode();

            switch (panelType)
            {
                case PanelType.Shop:
                    if (!shopPanel.IsShown)
                    {
                        shopPanel.Show(true);
                        SoundManager.Instance.PlaySound("MenuClick");
                    }
                    break;
                case PanelType.OfflineEarnings:
                    earningsPanel.Init((double)args, LocalizationManager.GetTranslation("OfflineIncome"), RewardedADUnits.OfflineIncomeMultiply);
                    break;
                case PanelType.LevelUpEarnings:
                    earningsPanel.Init((double)args, LocalizationManager.GetTranslation("LevelUpBonus"), RewardedADUnits.LevelUpMultiply);
                    break;
                case PanelType.PurchaseWindow:
                    afterPurchaseWindow.Init((Pair<string, string>)args);
                    break;
                case PanelType.BottomPanel:
                    bottomPanel.SwitchToTab((BottomPanel.BottomPanelTabType)args);
                    break;
                case PanelType.MultipliersDescription:
                    multipliersInfo.ShowDesc();
                    break;
                case PanelType.Settings:
                    settingsPanel.Show(true);
                    SoundManager.Instance.PlaySound("MenuClick");
                    break;
                case PanelType.AdOffer:
                    adWindow.Init((double)args);
                    break;
            }
            uiSettings.sortingOrder = 1;
        }

        private IEnumerator SimpleTimer(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }

        private void HideLoading()
        {
            loadingPanelUI.enabled = false;
            if (!earningsPanel.IsShown)
                uiSettings.sortingOrder = 0;
        }

        private void DeselectAll(object _ = null)
        {
            bottomPanel.DeselectAllBottomControls();
            multipliersInfo.HideDesc();
            uiSettings.sortingOrder = 0;
        }

        #region ButtonEvents
        private void CloseSettings(ClickEvent evt)
        {
            settingsPanel.Show(false);
            if (!bottomPanel.IsShown && !shopPanel.IsShown)
                uiSettings.sortingOrder = 0;
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private void CloseShop(ClickEvent evt = null)
        {
            shopPanel.Show(false);
            if (!bottomPanel.IsShown)
                uiSettings.sortingOrder = 0;
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private void CloseAdOffer(ClickEvent evt)
        {
            adWindow.Show(false);
            if (!bottomPanel.IsShown)
                uiSettings.sortingOrder = 0;
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private void ShowShop(ClickEvent evt)
        {
            OnShowPanel(PanelType.Shop);
        }

        private void BirdButtonClick(ClickEvent evt)
        {
            OnShowPanel(PanelType.BottomPanel, BottomPanel.BottomPanelTabType.Birds);
        }

        private void BoosterButtonClick(ClickEvent evt)
        {
            OnShowPanel(PanelType.BottomPanel, BottomPanel.BottomPanelTabType.Boosters);
        }

        private void DecorButtonClick(ClickEvent evt)
        {
            OnShowPanel(PanelType.BottomPanel, BottomPanel.BottomPanelTabType.Decors);
        }

        private void EnterEditMode(ClickEvent evt)
        {
            DeselectAll();
            if (shopPanel.IsShown)
                CloseShop();
            else
                SoundManager.Instance.PlaySound("MenuClick");

            isEditMode = true;
            bottomPanel.Show(false);
            doneButton.style.display = DisplayStyle.Flex;
            topPanel.EnterEditMode();
            Messenger<bool>.Broadcast(GameEvents.ActivateEditMode, true, MessengerMode.DONT_REQUIRE_LISTENER);
        }

        private void ExitEditMode(ClickEvent evt = null)
        {
            isEditMode = false;
            bottomPanel.Show(true);
            doneButton.style.display = DisplayStyle.None;
            topPanel.ExitEditMode();
            Messenger<bool>.Broadcast(GameEvents.ActivateEditMode, false, MessengerMode.DONT_REQUIRE_LISTENER);
            SoundManager.Instance.PlaySound("MenuClick");
        }

        #endregion
    }
}
