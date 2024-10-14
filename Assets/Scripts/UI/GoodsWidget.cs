using System;
using System.Collections;
using System.Collections.Generic;
using CardTD.NoAssembly;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class GoodsWidget
    {
        protected VisualElement rootVisualElement;
        protected VisualElement bg;

        protected VisualElement topHolder;
        protected Label topLabel;
        protected VisualElement topIcon;

        protected VisualElement image;
        protected VisualElement bgImage;

        //protected VisualElement button;
        protected VisualElement buttonIcon;
        protected Label buttonLabel;

        private Action clickAction = null;

        public float Cost;

        public GoodsWidget(VisualElement rootVisualElement)
        {
            this.rootVisualElement = rootVisualElement;

            bg = rootVisualElement.Q("Bg");

            topHolder = rootVisualElement.Q("TopHolder");
            topLabel = topHolder.Q("TopLabel") as Label;
            topIcon = topHolder.Q("Icon");

            image = rootVisualElement.Q("Image");
            bgImage = rootVisualElement.Q("BgImage");

            buttonIcon = rootVisualElement.Q("ButtonIcon");
            buttonLabel = rootVisualElement.Q("ButtonLabel") as Label;

            //topHolder.style.display = DisplayStyle.None;
            Cost = 0;
        }

        public void Init(Sprite topSprite, string topText, float cost, Action clickAction, Sprite bgImage, Color bgImageColor, Color bgColor, Sprite goodsImage, Sprite buttonSprite = null)
        {
            topIcon.style.backgroundImage = new StyleBackground(topSprite);
            topLabel.text = topText;

            if (buttonSprite != null)
            {
                buttonIcon.style.display = DisplayStyle.Flex;
                buttonIcon.style.backgroundImage = new StyleBackground(buttonSprite);
            }
            else
                buttonIcon.style.display = DisplayStyle.None;

            Cost = cost;
            buttonLabel.text = cost.ToString();

            image.style.backgroundImage = new StyleBackground(goodsImage);

            this.clickAction = clickAction;
            rootVisualElement.RegisterCallback<ClickEvent>(OnClick);

            this.bgImage.style.backgroundImage = new StyleBackground(bgImage);
            this.bgImage.style.unityBackgroundImageTintColor = new StyleColor(bgImageColor);

            bg.style.backgroundColor = new StyleColor(bgColor);
        }

        public void InitForIAP(Sprite topSprite, string topText, string itemId, string cost, Sprite bgImage, Color bgImageColor, Color bgColor, Sprite goodsImage, Sprite buttonSprite = null)
        {
            topIcon.style.backgroundImage = new StyleBackground(topSprite);
            topLabel.text = topText;

            if (buttonSprite != null)
            {
                buttonIcon.style.display = DisplayStyle.Flex;
                buttonIcon.style.backgroundImage = new StyleBackground(buttonSprite);
            }
            else
                buttonIcon.style.display = DisplayStyle.None;

            //Cost = cost;
            buttonLabel.text = cost;

            image.style.backgroundImage = new StyleBackground(goodsImage);

            clickAction = () => IAPManager.BuyProduct(itemId);
            rootVisualElement.RegisterCallback<ClickEvent>(OnClickForIAP);

            this.bgImage.style.backgroundImage = new StyleBackground(bgImage);
            this.bgImage.style.unityBackgroundImageTintColor = new StyleColor(bgImageColor);

            bg.style.backgroundColor = new StyleColor(bgColor);
        }

        private void OnClick(ClickEvent evt)
        {
            if (Cost <= PlayerData.Instance.Crystals)
            {
                clickAction?.Invoke();
                PlayerData.Instance.ChangeCrystals(-Mathf.RoundToInt(Cost));
                PlayerData.Instance.Save();
            }
            //TODO: subtract notes from player + show confirm purchase window
            //TODO: if (Cost > PlayerData.Instance.Crystals) => scroll shop to crystal goods
        }

        private void OnClickForIAP(ClickEvent evt)
        {
            clickAction?.Invoke();
        }

        public void Dispose()
        {
            if (clickAction != null)
            {
                rootVisualElement.UnregisterCallback<ClickEvent>(OnClick);
                clickAction = null;
            }
        }

        public void Update()
        {
            if (bgImage != null)
                bgImage.style.rotate = new Rotate(bgImage.style.rotate.value.angle.value + 10 * Time.deltaTime);
        }
    }
}
