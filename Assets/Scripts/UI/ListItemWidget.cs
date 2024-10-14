using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public abstract class ListItemWidget
    {
        protected enum ButtonState { Plant, BuyAndPlant, Upgrade, Closed, AlreadyPlanted, Waiting, Blocked }

        protected VisualElement rootVisualElement;
        protected Label nameLabel;
        protected Label levelLabel;
        protected Label descLabel;
        protected ButtonState buttonState;
        protected Button button;
        protected VisualElement image;

        public ListItemWidget(VisualElement rootVisualElement)
        {
            this.rootVisualElement = rootVisualElement;
            button = new Button(rootVisualElement.Q("Button"));
            nameLabel = rootVisualElement.Q("NameLabel") as Label;
            levelLabel = rootVisualElement.Q("LevelLabel") as Label;
            descLabel = rootVisualElement.Q("DescLabel") as Label;
            image = rootVisualElement.Q("Icon");

            button.rootVisualElement.RegisterCallback<ClickEvent>(OnButtonClick);
            Messenger.AddListener(GameEvents.UpdateLocalization, OnUpdateLocalization);
        }

        public virtual void Dispose()
        {
            button.rootVisualElement.UnregisterCallback<ClickEvent>(OnButtonClick);
            Messenger.RemoveListener(GameEvents.UpdateLocalization, OnUpdateLocalization);
        }

        public void SetBgColor(Color color) => image.style.backgroundColor = new StyleColor(color);

        public void SetImage(Sprite sprite) => image.style.backgroundImage = new StyleBackground(sprite);

        protected abstract void OnUpdateLocalization();

        protected abstract void OnButtonClick(ClickEvent evt);

        public abstract void UpdateValues();

        public abstract void OnShow();
    }
}
