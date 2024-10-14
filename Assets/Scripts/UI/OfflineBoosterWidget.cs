using I2.Loc;
using SO_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class OfflineBoosterWidget : TwoButtonsWidget
    {
        private OfflineBoosterData booster => DataHolder.Instance.OfflineBoosterData;

        public OfflineBoosterWidget(VisualElement rootVisualElement) : base(rootVisualElement)
        {
            OnUpdateLocalization();
            UpdateValues();
        }
        public override void OnShow() => SetState();

        public override void UpdateValues()
        {
            button.SetColor(button.Cost <= PlayerData.Instance.Notes ? UIHelper.Instance.UpgradeActiveColor : UIHelper.Instance.InactiveColor);
        }

        protected override void OnButtonClick(ClickEvent evt)
        {
            if (button.IsColorInactive)
                return;

            PlayerData.Instance.ChangeNotes(-button.Cost);
            PlayerData.Instance.LevelUpOfflineBooster();
            SetState(true);
        }

        protected override void OnButtonX10Click(ClickEvent evt)
        {
            PlayerData.Instance.ChangeNotes(-buttonX10.Cost);
            PlayerData.Instance.LevelUpOfflineBooster(10);
            SetState(true);
        }

        protected override void OnUpdateLocalization()
        {
            nameLabel.text = LocalizationManager.GetTranslation("OfflineBooster");
            button.SetText(LocalizationManager.GetTranslation("LevelUp"));
            SetState();
        }

        private void SetState(bool showX10 = false)
        {
            button.SetCost(booster.GetLevelUpCost(PlayerData.Instance.OfflineBoosterLevel));
            levelLabel.text = LocalizationManager.GetTranslation("Level") + " " + PlayerData.Instance.OfflineBoosterLevel;
            descLabel.text = string.Format(LocalizationManager.GetTranslation("OfflineBooster_desc"), 
                Utilities.GetNotesString(booster.GetOfflineBonusInSecond(PlayerData.Instance.OfflineBoosterLevel, PlayerData.Instance.CurrentTree.CalculateCleanIncomeInSecond())));

            if (showX10)
            {
                buttonX10.SetCost(booster.GetLevelUpCost(10));
                buttonX10.SetActive(buttonX10.Cost <= PlayerData.Instance.Notes);
            }
        }
    }
}
