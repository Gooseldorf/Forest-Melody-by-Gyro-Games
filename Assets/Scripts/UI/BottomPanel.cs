using Sirenix.OdinInspector;
using SO_Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BottomPanel : MonoBehaviour
    {
        public enum BottomPanelTabType { Birds, Boosters, Decors }

        private VisualElement bottomPanel;

        [SerializeField]
        private BirdsTab birdsTab;
        [SerializeField]
        private BoosterTab boosterTab;
        [SerializeField]
        private DecorTab decorTab;

        [ShowInInspector, ReadOnly]
        private BottomTab currentTab;

        public bool IsShown => currentTab != null;

        public void SetUpBottomPanel(VisualElement bottomPanel)
        {
            this.bottomPanel = bottomPanel;

            VisualElement birdButton = bottomPanel.Q("BirdsButton");
            VisualElement boosterButton = bottomPanel.Q("BoostsButton");
            VisualElement decorButton = bottomPanel.Q("DecorsButton");

            birdsTab.SetUpTab(bottomPanel.parent.Q("BirdTab"), birdButton.Q("Selection"));
            boosterTab.SetUpTab(bottomPanel.parent.Q("BoosterTab"), boosterButton.Q("Selection"));
            decorTab.SetUpTab(bottomPanel.parent.Q("DecorTab"), decorButton.Q("Selection"));
        }

        public void DisposeBottomPanel()
        {
            birdsTab.DisposeTab();
            boosterTab.DisposeTab();
            decorTab.DisposeTab();
        }

        public void Init(Tree tree)
        {
            birdsTab.Init(DataHolder.Instance.GetTreeData(tree.DataId).BirdsData);
            decorTab.Init(DataHolder.Instance.AllDecors);
            boosterTab.Init();
        }

        public void DeselectAllBottomControls()
        {
            if (currentTab != null)
            {
                currentTab.Hide(true);
                currentTab = null;
            }
        }

        public void SwitchToTab(BottomPanelTabType tabType)
        {
            bool needAnimation = currentTab == null;

            if (currentTab != null)
                currentTab.Hide(false);

            currentTab = tabType switch
            {
                BottomPanelTabType.Birds => birdsTab,
                BottomPanelTabType.Boosters => boosterTab,
                BottomPanelTabType.Decors => decorTab,
                _ => throw new System.NotImplementedException()
            };


            currentTab.Show(needAnimation);
        }

        public void UpdateValues()
        {
            birdsTab.UpdateValues();
            boosterTab.UpdateValues();
            decorTab.UpdateValues();
        }

        public void Show(bool show) => bottomPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
