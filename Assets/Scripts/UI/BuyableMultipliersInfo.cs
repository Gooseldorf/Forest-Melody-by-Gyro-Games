using I2.Loc;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BuyableMultipliersInfo : MonoBehaviour
    {
        [SerializeField]
        private VisualTreeAsset multiplierLine;

        private VisualElement button;
        private VisualElement addMultiplierIcon;
        private VisualElement icon1;
        private VisualElement icon2;
        private VisualElement icon3;
        private VisualElement icon4;
        private VisualElement descWindow;
        private Label bonusTitle;
        private Label durationTitle;
        private Label timeLeftLabel;
        private VisualElement boosterIcon;

        private List<BuyableMultiplierLine> lines = new();
        private Dictionary<int, VisualElement>  multipliersDictionary = new ();
        private List<VisualElement> activeIcons = new(4);
        private readonly int[] xPositions = { -10, 0, 10, 20 };
        private BuyableMultiplier minTimeLeftMultiplier;

        private bool isDescShown => descWindow.style.display == DisplayStyle.Flex;

        public void SetUpBuyableMultipliersInfo(VisualElement rootVisualElement)
        {
            button = rootVisualElement;
            descWindow = rootVisualElement.Q("MultipliersDescWindow");
            addMultiplierIcon = rootVisualElement.Q("MultiplierIcon");
            icon1 = rootVisualElement.Q("2");
            icon2 = rootVisualElement.Q("4");
            icon3 = rootVisualElement.Q("10");
            icon4 = rootVisualElement.Q("50");
            bonusTitle = rootVisualElement.Q("BonusTitle") as Label;
            durationTitle = rootVisualElement.Q("DurationTitle") as Label;
            timeLeftLabel = rootVisualElement.Q<Label>("TimeLeft");

            multipliersDictionary = new Dictionary<int, VisualElement>()
            {
                { 2, icon1 },
                { 4, icon2 },
                { 10, icon3 },
                { 50, icon4 }
            };

            descWindow.style.display = DisplayStyle.None;

            button.RegisterCallback<ClickEvent>(OnButtonClick);
            Messenger<int, bool>.AddListener(GameEvents.OnBuybleMultiplierBoosterAction, ToggleMultiplierIcon);
        }

        public void Init()
        {
            UpdateMultiplierIcons();
        }

        public void DisposeBuyableMultipliersInfo()
        {
            button.UnregisterCallback<ClickEvent>(OnButtonClick);
            Messenger<int, bool>.RemoveListener(GameEvents.OnBuybleMultiplierBoosterAction, ToggleMultiplierIcon);
        }

        private void ToggleMultiplierIcon(int multiplier, bool isActive)
        {
            if (isActive)
            {
                if(!activeIcons.Contains(multipliersDictionary[multiplier]))
                    activeIcons.Add(multipliersDictionary[multiplier]);
            }
            else
            {
                activeIcons.Remove(multipliersDictionary[multiplier]);
                MoveMultiplierIconToPositionX(multipliersDictionary[multiplier], 150);
            }
            UpdateMultiplierIcons();
        }
        
        private void MoveMultiplierIconToPositionX(VisualElement element, float targetPositionX)
        {
            float positionX = element.transform.position.x;
            Tweener moveIconTweener = DOTween.To(() => positionX, x => positionX = x,
                targetPositionX, 2);
            
            moveIconTweener.OnUpdate(() => {
                element.transform.position = new Vector3(positionX, 0);
            });
        }

        private void UpdateMultiplierIcons()
        {
            foreach (var multiplier in PlayerData.Instance.ActiveMultipliers)
            {
                if(!activeIcons.Contains(multipliersDictionary[multiplier.Multiplier]))
                    activeIcons.Add(multipliersDictionary[multiplier.Multiplier]);
            }

            List<VisualElement> orderedIcons = activeIcons.OrderByDescending(c => int.Parse(c.name)).ToList();
            for (int i = 0; i < activeIcons.Count; i++)
            {
                MoveMultiplierIconToPositionX(orderedIcons[i], xPositions[i]);
            }
            
            if (PlayerData.Instance.ActiveMultipliers.Count <= 0)
            {
                addMultiplierIcon.visible = true;
                timeLeftLabel.style.display = DisplayStyle.None;
            }
            else
            {
                addMultiplierIcon.visible = false;
                timeLeftLabel.style.display = DisplayStyle.Flex;
            }
            UpdateTimeLeftLabel();
        }

        private void UpdateTimeLeftLabel()
        {
            float timeLeft = float.MaxValue;
            foreach (var multiplier in PlayerData.Instance.ActiveMultipliers)
            {
                if ((multiplier as ICooldownable).TimeLeft < timeLeft)
                {
                    minTimeLeftMultiplier = multiplier;
                    timeLeft = (multiplier as ICooldownable).TimeLeft;
                }
            }
        } 

        private void OnButtonClick(ClickEvent evt)
        {
            if (PlayerData.Instance.ActiveMultipliers.Count != 0)
            {
                if (!isDescShown)
                    Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UIManager.PanelType.MultipliersDescription, null, MessengerMode.DONT_REQUIRE_LISTENER);
                else
                    Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UIManager.PanelType.Shop, null, MessengerMode.DONT_REQUIRE_LISTENER);
            }
            else
                Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UIManager.PanelType.Shop, null, MessengerMode.DONT_REQUIRE_LISTENER);
        }

        public void UpdateValues()
        {
            if (isDescShown)
                lines.ForEach(x => x.UpdateValues());
            if(minTimeLeftMultiplier != null)
                timeLeftLabel.text = Utilities.GetTimeString((minTimeLeftMultiplier as ICooldownable).TimeLeft);
        }

        public void ShowDesc()
        {
            bonusTitle.text = LocalizationManager.GetTranslation("Bonus");
            durationTitle.text = LocalizationManager.GetTranslation("Duration");

            VisualElement line;
            int totalMultiplier = 0;
            for (int i = 0; i < PlayerData.Instance.ActiveMultipliers.Count; i++)
            {
                line = multiplierLine.CloneTree();
                descWindow.Add(line);
                lines.Add(new BuyableMultiplierLine(line, PlayerData.Instance.ActiveMultipliers[i], i != 0));
                totalMultiplier += PlayerData.Instance.ActiveMultipliers[i].Multiplier;
            }
            //the last line in desc (sum)
            line = multiplierLine.CloneTree();
            descWindow.Add(line);
            lines.Add(new BuyableMultiplierLine(line, totalMultiplier));
            descWindow.style.display = DisplayStyle.Flex;
        }

        public void HideDesc()
        {
            if (isDescShown)
            {
                lines.ForEach((x) =>
                {
                    descWindow.Remove(x.RootVisualElement);
                    x.Dispose();
                });
                lines.Clear();
                descWindow.style.display = DisplayStyle.None;
            }
        }
    }
}
