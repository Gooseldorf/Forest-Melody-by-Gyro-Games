using UnityEngine.UIElements;

namespace UI
{
    public abstract class TwoButtonsWidget : ListItemWidget
    {
        protected Button buttonX10;

        public TwoButtonsWidget(VisualElement rootVisualElement) : base(rootVisualElement)
        {
            buttonX10 = new Button(rootVisualElement.Q("ButtonX10"));

            buttonX10.SetActive(false);
            buttonX10.rootVisualElement.RegisterCallback<ClickEvent>(OnButtonX10Click);
        }

        protected abstract void OnButtonX10Click(ClickEvent evt);

    }
}
