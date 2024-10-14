using Sirenix.OdinInspector;
using SO_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using I2.Loc;

namespace UI
{
    public class BirdsTab : BottomTab
    {
        [SerializeField]
        private VisualTreeAsset birdsWidget;
        
        public void Init(IReadOnlyList<BirdData> birds)
        {
            VisualElement bird;
            for (int i = 0; i < birds.Count; i++)
            {
                bird = birdsWidget.CloneTree();

                scrollView.Add(bird);
                widgets.Add(new BirdWidget(bird, birds[i]));
            }
            OnUpdateLocalization();
            Messenger<bool, bool>.AddListener(GameEvents.EmptyContainersCheck, BlockHatchButtons);
        }

        private void BlockHatchButtons(bool commonExists, bool specialExists)
        {
            for (int i = 0; i < widgets.Count; i++)
            {
                if (widgets[i] is BirdWidget bird)
                {
                    if(bird.ContainerType == ContainerType.Common)
                        bird.IsBlocked = !commonExists;
                    else if (bird.ContainerType == ContainerType.Special)
                        bird.IsBlocked = !specialExists;
                }
            }
        }

        protected override void OnUpdateLocalization() => title.text = LocalizationManager.GetTranslation("BirdsCollection");

        public override void DisposeTab()
        {
            base.DisposeTab();
            Messenger<bool, bool>.RemoveListener(GameEvents.EmptyContainersCheck, BlockHatchButtons);
        }
    }
}
