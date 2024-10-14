using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BoosterWidget : TwoButtonsWidget
    {
        private FreeBooster booster;

        public BoosterWidget(VisualElement rootVisualElement, FreeBooster booster) : base(rootVisualElement)
        {
            this.booster = booster;

            rootVisualElement.Q("ProgressBar").style.display = DisplayStyle.None;

            OnUpdateLocalization();
            UpdateValues();
        }

        private void SetState(bool showX10 = false)
        {
            button.SetCost(booster.GetLevelUpCost());
            levelLabel.text = LocalizationManager.GetTranslation("Level") + " " + booster.Level;
            descLabel.text = booster.GetDescription();

            if (showX10)
            {
                buttonX10.SetCost(booster.GetLevelUpCost(10));
                buttonX10.SetActive(buttonX10.Cost <= PlayerData.Instance.Notes);
            }
        }

        public override void UpdateValues()
        {
            button.SetColor(button.Cost <= PlayerData.Instance.Notes ? UIHelper.Instance.UpgradeActiveColor : UIHelper.Instance.InactiveColor);
        }

        protected override void OnButtonClick(ClickEvent evt)
        {
            if (button.IsColorInactive)
                return;

            PlayerData.Instance.ChangeNotes(-button.Cost);
            booster.LevelUp();
            SetState(true);
        }

        protected override void OnButtonX10Click(ClickEvent evt)
        {
            PlayerData.Instance.ChangeNotes(-buttonX10.Cost);
            booster.LevelUp(10);
            SetState(true);
        }

        protected override void OnUpdateLocalization()
        {
            nameLabel.text = LocalizationManager.GetTranslation(booster.GetType().ToString());
            button.SetText(LocalizationManager.GetTranslation("LevelUp"));
            SetState();
            //LocalizationManager.GetTranslation("FreeMultiplierBooster");
        }

        public override void OnShow() => SetState();
    }
}
