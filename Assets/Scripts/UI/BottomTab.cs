using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using SO_Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public abstract class BottomTab : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        protected VisualElement tab;
        [ShowInInspector, ReadOnly]
        private VisualElement button;

        protected VisualElement scrollView;
        protected Label title;

        protected List<ListItemWidget> widgets = new();

        public virtual void SetUpTab(VisualElement tab, VisualElement button)
        {
            this.tab = tab;
            this.button = button;
            Messenger.AddListener(GameEvents.UpdateLocalization, OnUpdateLocalization);
            scrollView = tab.Q("ScrollView").Q("unity-content-container");
            title = tab.Q("TabTitle") as Label;

#if UNITY_EDITOR
            ScrollView scrollView1 = tab.Q("ScrollView") as ScrollView;
            scrollView1.RegisterCallback<WheelEvent>((evt) =>
            {
                scrollView1.scrollOffset = new Vector2(0, scrollView1.scrollOffset.y + 1000 * evt.delta.y);
                evt.StopPropagation();
            }
             );
#endif
        }

        public virtual void DisposeTab()
        {
            Messenger.RemoveListener(GameEvents.UpdateLocalization, OnUpdateLocalization);
            widgets.ForEach(x => x.Dispose());
            widgets.Clear();
        }

        public virtual void Show(bool withAnimation)
        {
            if (withAnimation)
            {
                tab.style.height = 0;
                tab.style.display = DisplayStyle.Flex;
                DOTween.To(() => tab.worldBound.height, x => tab.style.height = x, 1290, 0.5f).SetEase(Ease.OutBack);
                SoundManager.Instance.PlaySound("OpenMenu");
            }
            else
            {
                tab.style.height = 1290;
                tab.style.display = DisplayStyle.Flex;
                SoundManager.Instance.PlaySound("MenuClick");
            }

            button.style.display = DisplayStyle.Flex;

            widgets.ForEach(x => x.OnShow());
        }

        public void Hide(bool withAnimation)
        {
            if (withAnimation)
            {
                DOTween.To(() => tab.worldBound.height, x => tab.style.height = x, 0, 0.25f).OnComplete(() => tab.style.display = DisplayStyle.None);
                //    .OnComplete(() =>
                //{
                //    tab.style.display = DisplayStyle.None;
                //    tab.style.height = 1290;
                //});
                SoundManager.Instance.PlaySound("CloseMenu");
            }
            else
            {
                tab.style.display = DisplayStyle.None;
            }

            button.style.display = DisplayStyle.None;
        }

        public virtual void UpdateValues() => widgets.ForEach(x => x.UpdateValues());

        protected abstract void OnUpdateLocalization();

    }
}
