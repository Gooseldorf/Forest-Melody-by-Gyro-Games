using I2.Loc;
using SO_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class DecorWidget : ListItemWidget
    {
        private DecorData decorData;

        private VisualElement alreadyMark;

        private bool isBlocked = false;

        public DecorWidget(VisualElement rootVisualElement, DecorData decorData) : base(rootVisualElement)
        {
            this.decorData = decorData;
            descLabel.style.display = DisplayStyle.None;
            alreadyMark = rootVisualElement.Q("AlreadyMark");
            SetState();
        }

        public bool IsBlocked
        {
            get => isBlocked;
            set
            {
                isBlocked = value;
                SetState();
            }
        }

        private void SetState()
        {
            alreadyMark.style.display = DisplayStyle.None;
            button.ShowTopPart(true);

            //if can plant or buy
            if (decorData.Condition.CheckCondition())
            {
                nameLabel.text = LocalizationManager.GetTranslation("Decors/" + decorData.ID + "_title");
                levelLabel.text = LocalizationManager.GetTranslation("Decors/" + decorData.ID + "_desc");

                Decor decor = PlayerData.Instance.CurrentTree.Decors.Find(x => x.DataID == decorData.ID);
                // if already bought
                if (decor != null)
                {
                    //if already planted
                    if (decor.BranchPosition != -1)
                    {
                        alreadyMark.style.display = DisplayStyle.Flex;

                        button.ShowBottomPart(false);
                        button.ShowTopPart(false);

                        buttonState = ButtonState.AlreadyPlanted;
                        button.SetColor(UIHelper.Instance.DecorPlantedColor);
                    }
                    //if need to plant
                    else
                    {
                        button.SetText(LocalizationManager.GetTranslation("Plant"));
                        button.ShowBottomPart(false);

                        buttonState = ButtonState.Plant;
                        button.SetColor(UIHelper.Instance.ActiveColor);
                    }
                }
                // if can buy
                else
                {
                    button.SetText(LocalizationManager.GetTranslation("Plant"));
                    button.ShowBottomPart(true);
                    button.SetCost(decorData.CrystalCost);

                    buttonState = ButtonState.BuyAndPlant;
                    button.SetColor(UIHelper.Instance.DecorActiveColor);
                }
            }
            // if locked
            else
            {
                nameLabel.text = LocalizationManager.GetTranslation("Unknown");
                levelLabel.text = LocalizationManager.GetTranslation("UnlockBirds");

                button.SetText(LocalizationManager.GetTranslation("Locked"));
                button.ShowBottomPart(false);

                buttonState = ButtonState.Closed;
                button.SetColor(UIHelper.Instance.InactiveColor);
            }

            //if there is no empty containers for this decor type 
            if (IsBlocked && buttonState is ButtonState.Plant or ButtonState.BuyAndPlant && buttonState != ButtonState.Closed)
            {
                buttonState = ButtonState.Blocked;
                button.SetColor(UIHelper.Instance.InactiveColor);
            }

            SetImage(UIHelper.Instance.GetDecorSprite(buttonState == ButtonState.Closed? "Unknown" : decorData.ID));
        }

        protected override void OnButtonClick(ClickEvent evt)
        {
            Decor decor;
            switch (buttonState)
            {
                case ButtonState.Plant:
                    decor = PlayerData.Instance.CurrentTree.Decors.Find(x => x.DataID == decorData.ID);
                    Messenger<Decor>.Broadcast(GameEvents.PlantDecor, decor, MessengerMode.DONT_REQUIRE_LISTENER);
                    break;
                case ButtonState.BuyAndPlant:
                    decor = new Decor(decorData.ID);
                    PlayerData.Instance.CurrentTree.AddDecor(decor);
                    Messenger<Decor>.Broadcast(GameEvents.PlantDecor, decor, MessengerMode.DONT_REQUIRE_LISTENER);
                    break;
            }
            SetState();
        }
        public override void UpdateValues() { }
        protected override void OnUpdateLocalization() => SetState();
        public override void OnShow() => SetState();
    }
}
