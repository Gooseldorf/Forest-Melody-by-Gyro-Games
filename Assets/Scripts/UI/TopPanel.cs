using UnityEngine;
using UnityEngine.UIElements;
using I2.Loc;
using SO_Scripts;

namespace UI
{
    public class TopPanel : MonoBehaviour
    {
        private VisualElement topPanel;

        private VisualElement leftButtons;
        private VisualElement settingButton;

        private VisualElement expFg;
        private Label levelLabel;
        private Label notesLabel;
        private Label crystalsLabel;

        private VisualElement plusNotesButton;
        private VisualElement plusCrystalsButton;

        private VisualElement levelInfo;
        private VisualElement levelUpButton;
        private Label levelUpLabel;

        private Label doneLabel;

        public void SetUpTopPanel(VisualElement topPanel)
        {
            //topPanel = treeUI.rootVisualElement.Q("TopPanel");
            this.topPanel = topPanel;

            expFg = topPanel.Q("Fg");
            levelLabel = topPanel.Q("LevelLabel") as Label;
            notesLabel = topPanel.Q("NotesWidget").Q("CurrencyLabel") as Label;
            crystalsLabel = topPanel.Q("CrystalWidget").Q("CurrencyLabel") as Label;
            plusNotesButton = topPanel.Q("NotesWidget").Q("ShopButton");
            plusCrystalsButton = topPanel.Q("CrystalWidget").Q("ShopButton");
            levelInfo = topPanel.Q("LevelInfo");
            levelUpButton = topPanel.Q("LevelUpButton");
            levelUpLabel = levelUpButton.Q("LevelUpLabel") as Label;

            topPanel.Q("NotesWidget").Q("CurrencyImage").style.backgroundImage = new StyleBackground(UIHelper.Instance.GetSprite("Notes_icon"));
            topPanel.Q("CrystalWidget").Q("CurrencyImage").style.backgroundImage = new StyleBackground(UIHelper.Instance.GetSprite("Crystal_icon"));

            expFg.style.width = Length.Percent(0);

            settingButton = topPanel.Q("SettingButton");
            settingButton.RegisterCallback<ClickEvent>(SettingsClick);

            leftButtons = topPanel.Q("LeftButtons");


            doneLabel = topPanel.Q("DoneButton").Q("DoneLabel") as Label;

            plusNotesButton.RegisterCallback<ClickEvent>(ShopButtonsClick);
            plusCrystalsButton.RegisterCallback<ClickEvent>(ShopButtonsClick);
            levelUpButton.RegisterCallback<ClickEvent>(OnLevelUpButtonClick);

            Messenger.AddListener(GameEvents.UpdateLocalization, OnUpdateLocalization);

            OnUpdateLocalization();
        }

        public void DisposeTopPanel()
        {
            settingButton.UnregisterCallback<ClickEvent>(SettingsClick);
            plusNotesButton.UnregisterCallback<ClickEvent>(ShopButtonsClick);
            plusCrystalsButton.UnregisterCallback<ClickEvent>(ShopButtonsClick);
            levelUpButton.UnregisterCallback<ClickEvent>(OnLevelUpButtonClick);

            Messenger.RemoveListener(GameEvents.UpdateLocalization, OnUpdateLocalization);
        }

        private void ShopButtonsClick(ClickEvent evt)
        {
            Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UIManager.PanelType.Shop, null,
                MessengerMode.DONT_REQUIRE_LISTENER);
        }

        private void SettingsClick(ClickEvent evt)
        {
            Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UIManager.PanelType.Settings, null,
                MessengerMode.DONT_REQUIRE_LISTENER);
        }

        public void EnterEditMode()
        {
            leftButtons.visible = false;
            levelUpButton.style.display = DisplayStyle.None;
            levelInfo.style.display = DisplayStyle.None;
        }

        public void ExitEditMode() => leftButtons.visible = true;

        private void OnLevelUpButtonClick(ClickEvent evt)
        {
            if (ADsManager.Instance.InterstitialADLoaded)
                ADsManager.Instance.ShowInterstitialAd();
            
            PlayerData.Instance.LevelUp();
            Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel,
                                                            UIManager.PanelType.LevelUpEarnings,
                                                            DataHolder.Instance.GetRewardForLevelUp(PlayerData.Instance.PlayerLevel),
                                                            MessengerMode.DONT_REQUIRE_LISTENER);
            SoundManager.Instance.PlaySound("LevelUp");
        }

        public void UpdateValues(bool isEditMode)
        {
            if (!isEditMode)
            {
                if (PlayerData.Instance.Experience >= PlayerData.Instance.ExperienceForNextLevel)
                {
                    if (levelUpButton.style.display != DisplayStyle.Flex)
                    {
                        levelUpButton.style.display = DisplayStyle.Flex;
                        levelInfo.style.display = DisplayStyle.None;
                    }
                }
                else
                {
                    if (levelInfo.style.display != DisplayStyle.Flex)
                    {
                        levelInfo.style.display = DisplayStyle.Flex;
                        levelUpButton.style.display = DisplayStyle.None;
                    }
                    expFg.style.width = Length.Percent(Mathf.Clamp((int)((PlayerData.Instance.Experience * 100) / PlayerData.Instance.ExperienceForNextLevel), 0, 100));
                }
            }

            levelLabel.text = LocalizationManager.GetTranslation("Level") + " " + PlayerData.Instance.PlayerLevel;
            notesLabel.text = Utilities.GetNotesString(PlayerData.Instance.Notes);
            crystalsLabel.text = PlayerData.Instance.Crystals.ToString();
        }

        private void OnUpdateLocalization()
        {
            levelUpLabel.text = LocalizationManager.GetTranslation("LevelUp");
            doneLabel.text = LocalizationManager.GetTermTranslation("Done");
        }
    }
}
