using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class SettingsSwitcher
    {
        public VisualElement RootVisualElement;
        private Label settingsLabel;
        private VisualElement switcher;

        public bool Active => switcher.style.alignItems == Align.FlexEnd;

        public SettingsSwitcher(VisualElement rootVisualElement)
        {
            RootVisualElement = rootVisualElement;
            settingsLabel = rootVisualElement.Q("Title") as Label;
            switcher = rootVisualElement.Q("SwitcherBg");
        }

        public void SetState(bool active)
        {
            switcher.style.backgroundColor = new StyleColor(active ? UIHelper.Instance.ActiveColor : UIHelper.Instance.DecorActiveColor);
            //switcher.style.alignContent = 
            switcher.style.alignItems= active ? Align.FlexEnd : Align.FlexStart;
        }

        public void SetText(string text)
        {
            settingsLabel.text = text;
        }
    }
}
