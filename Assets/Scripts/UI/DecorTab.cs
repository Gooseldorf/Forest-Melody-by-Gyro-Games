using I2.Loc;
using SO_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class DecorTab : BottomTab
    {
        [SerializeField]
        private VisualTreeAsset decorWidget;

        public void Init(IReadOnlyList<DecorData> decors)
        {
            VisualElement decor;
            for (int i = 0; i < decors.Count; i++)
            {
                decor = decorWidget.CloneTree();

                scrollView.Add(decor);
                widgets.Add(new DecorWidget(decor, decors[i]));
            }
            OnUpdateLocalization();
            Messenger<bool, bool>.AddListener(GameEvents.EmptyContainersCheck, BlockPlaceButtons);
        }
        
        private void BlockPlaceButtons(bool commonExists, bool specialExists)
        {
            for (int i = 0; i < widgets.Count; i++)
            {
                if (widgets[i] is DecorWidget decor)
                {
                    decor.IsBlocked = !commonExists;
                }
            }
        }
        
        public override void DisposeTab()
        {
            base.DisposeTab();
            Messenger<bool, bool>.RemoveListener(GameEvents.EmptyContainersCheck, BlockPlaceButtons);
        }

        protected override void OnUpdateLocalization() => title.text = LocalizationManager.GetTranslation("Decoration");
    }
}
