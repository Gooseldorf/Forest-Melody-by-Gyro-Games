using SO_Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using I2.Loc;

namespace UI
{
    public class BirdWidget : TwoButtonsWidget
    {
        private BirdData birdData;
        private List<Bird> birdsOfThisType;
        private bool isBlocked = false;

        protected VisualElement progressFg;
        protected VisualElement progressBar;

        bool needToShowX10;

        public ContainerType ContainerType => birdData.Type;

        public bool IsBlocked
        {
            get => isBlocked;
            set
            {
                isBlocked = value;
                SetState();
            }
        }


        public BirdWidget(VisualElement rootVisualElement, BirdData birdData) : base(rootVisualElement)
        {
            this.birdData = birdData;

            progressBar = rootVisualElement.Q("ProgressBar");
            progressFg = rootVisualElement.Q("Fg");

            needToShowX10 = false;

            //SetImage(EffectVisualManager.Instance.birdsAtlas.GetSprite(birdData.ID));
            SetImage(UIHelper.Instance.GetBirdSprite(birdData.ID));

            OnUpdateLocalization();
            UpdateValues();
        }

        private void GetBirds()
        {
            birdsOfThisType = PlayerData.Instance.CurrentTree.Birds.FindAll(x => x.DataID == birdData.ID);
            birdsOfThisType.Sort(BirdComparer);
        }

        private void SetState(bool showX10 = false)
        {
            GetBirds();

            buttonX10.SetActive(false);

            //if bird is not accessible
            if (!birdData.Condition.CheckCondition())
            {
                button.SetText(LocalizationManager.GetTranslation("Require") + "\n" + birdData.Condition.GetCondition());
                levelLabel.text = LocalizationManager.GetTranslation("Egg");
                descLabel.text = "";

                needToShowX10 = false;

                button.ShowBottomPart(false);
                progressBar.style.display = DisplayStyle.None;
                buttonState = ButtonState.Closed;
                return;
            }

            //if it first bird on the tree
            if (birdsOfThisType.Count == 0)
            {
                SetPlantState();

                button.ShowBottomPart(true);
                progressBar.style.display = DisplayStyle.None;
                button.SetCost(birdData.BuildCost);

                needToShowX10 = false;

                buttonState = ButtonState.BuyAndPlant;
            }
            else
            {
                //if bird hatching
                if (birdsOfThisType[^1].Level == 0 && birdsOfThisType[^1].PositionIndex != -1)
                {
                    button.SetText(LocalizationManager.GetTranslation("Hatching"));
                    levelLabel.text = LocalizationManager.GetTranslation("Egg") + (birdsOfThisType.Count > 1 ? " " + (birdsOfThisType.Count).ToString() : "");
                    descLabel.text = "";

                    needToShowX10 = false;

                    button.ShowBottomPart(false);
                    progressBar.style.display = DisplayStyle.None;
                    buttonState = ButtonState.Waiting;
                    return;
                }

                //if previous bird is maxLevel or was bought but wasn't plant
                if (birdsOfThisType[^1].Level == BirdData.MaxLevel || birdsOfThisType[^1].PositionIndex == -1)
                {
                    SetPlantState();

                    button.ShowBottomPart(false);
                    progressBar.style.display = DisplayStyle.None;
                    button.SetCost(0);

                    needToShowX10 = false;

                    buttonState = ButtonState.Plant;
                }
                //if upgrade
                else
                {
                    button.SetText(LocalizationManager.GetTranslation("Upgrade"));
                    levelLabel.text = birdsOfThisType.Count > 1 ? (birdsOfThisType.Count).ToString() + "/" + (birdsOfThisType.Count).ToString() + " " : "";
                    levelLabel.text += LocalizationManager.GetTranslation("Level") + " " + birdsOfThisType[^1].Level;
                    descLabel.text = Utilities.GetNotesString(birdsOfThisType[^1].Notes) + " " + LocalizationManager.GetTranslation("NotesPerSec");

                    progressBar.style.display = DisplayStyle.Flex;
                    progressFg.style.width = Length.Percent((birdsOfThisType[^1].Level % 10 * 100f) / 10);

                    button.ShowBottomPart(true);
                    //Debug.Log(birdsOfThisType[^1].Level +" "+ birdData.GetLevelUpCost(birdsOfThisType[^1].Level, birdsOfThisType.Count));
                    button.SetCost(birdData.GetLevelUpCost(birdsOfThisType[^1].Level, birdsOfThisType.Count));

                    buttonState = ButtonState.Upgrade;

                    if (showX10)
                    {
                        int levels = Mathf.Min(10, BirdData.MaxLevel - birdsOfThisType[^1].Level);

                        buttonX10.SetText("x" + levels);
                        buttonX10.SetCost(birdData.GetLevelUpCost(birdsOfThisType[^1].Level, birdsOfThisType.Count, levels));

                        showX10 = levels > 1;
                    }
                    needToShowX10 = showX10;
                }
            }
            
            //if there is no empty containers for this bird type 
            if (IsBlocked && buttonState is ButtonState.Plant or ButtonState.BuyAndPlant)
            {
                buttonState = ButtonState.Blocked;
            }

            void SetPlantState()
            {
                button.SetText(LocalizationManager.GetTranslation("Hatch"));
                levelLabel.text = LocalizationManager.GetTranslation("Egg") + (birdsOfThisType.Count > 1 ? " " + (birdsOfThisType.Count + 1).ToString() : "");
                descLabel.text = "";
            }
        }

        public override void UpdateValues()
        {
            switch (buttonState)
            {
                case ButtonState.Plant:
                case ButtonState.BuyAndPlant:
                    button.SetColor(button.Cost <= PlayerData.Instance.Notes ? UIHelper.Instance.ActiveColor : UIHelper.Instance.InactiveColor);
                    break;
                case ButtonState.Upgrade:
                    button.SetColor(button.Cost <= PlayerData.Instance.Notes ? UIHelper.Instance.UpgradeActiveColor : UIHelper.Instance.InactiveColor);
                    break;
                case ButtonState.Blocked:
                case ButtonState.Closed:
                    button.SetColor(UIHelper.Instance.InactiveColor);
                    break;
                case ButtonState.Waiting:
                    button.SetColor(UIHelper.Instance.InactiveColor);
                    if (birdsOfThisType[^1].Level != 0)
                        SetState();
                    break;
                default:
                    button.SetColor(Color.magenta);
                    break;
            }

            if (needToShowX10)
                buttonX10.SetActive(buttonX10.Cost <= PlayerData.Instance.Notes);
            //button.style.backgroundColor = buttonState switch
            //{
            //    ButtonState.BuyAndPlant => cost <= PlayerData.Instance.Notes ? new StyleColor(UIHelper.Instance.ActiveColor) : new StyleColor(UIHelper.Instance.InactiveColor),
            //    ButtonState.Upgrade => cost <= PlayerData.Instance.Notes ? new StyleColor(UIHelper.Instance.UpgradeActiveColor) : new StyleColor(UIHelper.Instance.InactiveColor),
            //    ButtonState.Closed => new StyleColor(UIHelper.Instance.InactiveColor),
            //    _ => new StyleColor(UIHelper.Instance.ActiveColor)
            //};
        }

        public override void Dispose()
        {
            buttonX10.rootVisualElement.UnregisterCallback<ClickEvent>(OnButtonX10Click);
        }

        protected override void OnUpdateLocalization()
        {
            nameLabel.text = LocalizationManager.GetTranslation("Birds/" + birdData.ID + "_title");
            SetState();
        }

        protected override void OnButtonX10Click(ClickEvent evt)
        {
            PlayerData.Instance.ChangeNotes(-buttonX10.Cost);
            birdsOfThisType[^1].LevelUp(Mathf.Min(10, BirdData.MaxLevel - birdsOfThisType[^1].Level));
            SetState(true);
            SoundManager.Instance.PlaySound("MenuClick");
        }

        protected override void OnButtonClick(ClickEvent evt)
        {
            if (button.IsColorInactive)
                return;

            PlayerData.Instance.ChangeNotes(-button.Cost);

            switch (buttonState)
            {
                case ButtonState.BuyAndPlant:
                    PlantNewBird(birdData.ID);
                    break;
                case ButtonState.Plant:
                    if (birdsOfThisType[^1].PositionIndex == -1)
                        PlantBird(birdsOfThisType[^1]);
                    else
                        PlantNewBird(birdData.ID);
                    break;
                case ButtonState.Upgrade:
                    birdsOfThisType[^1].LevelUp();
                    break;
            }

            void PlantBird(Bird bird) => Messenger<Bird>.Broadcast(GameEvents.PlantBird, bird, MessengerMode.DONT_REQUIRE_LISTENER);
            void PlantNewBird(string id)
            {
                Bird bird = new Bird(id);
                PlayerData.Instance.CurrentTree.AddBird(bird);
                PlantBird(bird);
            }

            SetState(true);
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private int BirdComparer(Bird a, Bird b)
        {
            return b.Level.CompareTo(a.Level);
        }

        public override void OnShow() => SetState();
    }
}
