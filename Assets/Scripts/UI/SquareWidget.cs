using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class SquareWidget
    {
        enum ButtonState { None, Ready, Wait }

        private ButtonState buttonState;

        public VisualElement RootVisualElement;
        protected VisualElement bottomPart;
        protected Label label;
        protected VisualElement cooldownPart;
        protected VisualElement progressBarFg;
        protected Label timerLabel;
        protected VisualElement image1;
        protected VisualElement image2;
        protected VisualElement image3;

        public bool IsReady => buttonState == ButtonState.Ready;
        public VisualElement BottomPart => bottomPart;

        private Action clickAction = null;

        public SquareWidget(VisualElement rootVisualElement)
        {
            RootVisualElement = rootVisualElement;
            bottomPart = rootVisualElement.Q("Bottom");
            label = rootVisualElement.Q("Label") as Label;
            cooldownPart = rootVisualElement.Q("Cooldown");
            progressBarFg = cooldownPart.Q("Fg");
            timerLabel = cooldownPart.Q("TimerLabel") as Label;
            image1 = rootVisualElement.Q("Image1");
            image2 = rootVisualElement.Q("Image2");
            image3 = rootVisualElement.Q("Image3");
            buttonState = ButtonState.None;
        }

        public void SetText(string text)
        {
            if (buttonState != ButtonState.Ready)
            {
                buttonState = ButtonState.Ready;
                CheckState();
            }

            label.text = text;
        }

        public void SetBgColor(Color color) => image1.style.backgroundColor = new StyleColor(color);

        public void SetImages(Sprite topSprite, Sprite secondSprite = null, Color secondColor = default)
        {
            if (secondSprite != null)
            {
                image2.style.backgroundImage = new StyleBackground(secondSprite);
                if(secondColor!=default)
                    image2.style.unityBackgroundImageTintColor = new StyleColor(secondColor);
            }
            image3.style.backgroundImage = new StyleBackground(topSprite);
        }

        public void SetTimer(float current, float max)
        {
            if (buttonState != ButtonState.Wait)
            {
                buttonState = ButtonState.Wait;
                CheckState();
            }

            progressBarFg.style.width = Length.Percent((current * 100f) / max);
            timerLabel.text = TimeSpan.FromSeconds((int)(max - current)).ToString("c");
        }

        private void CheckState()
        {
            switch (buttonState)
            {
                case ButtonState.Ready:
                    label.style.display = DisplayStyle.Flex;
                    cooldownPart.style.display = DisplayStyle.None;
                    bottomPart.style.backgroundColor = new StyleColor(UIHelper.Instance.UpgradeActiveColor);
                    break;
                case ButtonState.Wait:
                    label.style.display = DisplayStyle.None;
                    cooldownPart.style.display = DisplayStyle.Flex;
                    bottomPart.style.backgroundColor = new StyleColor(UIHelper.Instance.InactiveColor);
                    break;
            }
        }

        public void RegisterCallback(Action clickAction)
        {
            this.clickAction = clickAction;
            RootVisualElement.RegisterCallback<ClickEvent>(OnClick);
        }

        private void OnClick(ClickEvent evt)
        {
            clickAction?.Invoke();
        }

        public void Dispose()
        {
            if (clickAction != null)
            {
                RootVisualElement.UnregisterCallback<ClickEvent>(OnClick);
                clickAction = null;
            }
        }
    }
}
