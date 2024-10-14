using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class Button
    {
        public VisualElement rootVisualElement;
        private VisualElement bottomButtonPart;
        private Label priceLabel;
        private Label buttonLabel;
        public double Cost { get; private set; }

        public bool IsColorInactive => rootVisualElement.style.backgroundColor == UIHelper.Instance.InactiveColor;
        public bool activeSelf => rootVisualElement.style.display == DisplayStyle.Flex;

        public Button(VisualElement rootVisualElement)
        {
            this.rootVisualElement = rootVisualElement;
            bottomButtonPart = rootVisualElement.Q("BottomButtonPart");
            priceLabel = rootVisualElement.Q("PriceLabel") as Label;
            buttonLabel = rootVisualElement.Q("ButtonLabel") as Label;
        }

        public void SetText(string text)
        {
            buttonLabel.text = text;
        }

        public void SetCost(double price)
        {
            Cost = price;
            priceLabel.text = Utilities.GetNotesString(Cost);
        }

        public void ShowTopPart(bool show)
        {
            buttonLabel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void ShowBottomPart(bool show)
        {
            bottomButtonPart.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetColor(Color color)
        {
            rootVisualElement.style.backgroundColor = new StyleColor(color);
        }

        public void SetActive(bool show)
        {
            rootVisualElement.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
