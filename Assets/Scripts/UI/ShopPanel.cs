using I2.Loc;
using SO_Scripts;
using System.Collections.Generic;
using System.Linq;
using CardTD.NoAssembly;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

namespace UI
{
    public class ShopPanel : MonoBehaviour
    {
        [SerializeField]
        private VisualTreeAsset goodsWidget;

        private VisualElement shopPanel;

        private Label crystalsLabel;
        private VisualElement crystalsRoot;

        private Label multipliersLabel;
        private VisualElement multipliersRoot;

        private Label timeSkipLabel;
        private VisualElement timeSkipRoot;

        private List<GoodsWidget> goods = new();

        public bool IsShown => shopPanel.visible;
        public void SetUpShopPanel(VisualElement shopPanel)
        {
            this.shopPanel = shopPanel;

            crystalsLabel = shopPanel.Q("CrystalsLabel") as Label;
            crystalsRoot = shopPanel.Q("CrystalsRoot");

            multipliersLabel = shopPanel.Q("MultipliersLabel") as Label;
            multipliersRoot = shopPanel.Q("MultipliersRoot");

            timeSkipLabel = shopPanel.Q("TimeSkipLabel") as Label;
            timeSkipRoot = shopPanel.Q("TimeSkipRoot");

            Messenger.AddListener(GameEvents.UpdateLocalization, OnUpdateLocalization);



#if UNITY_EDITOR
            ScrollView scrollView1 = shopPanel.Q("ScrollView") as ScrollView;
            scrollView1.RegisterCallback<WheelEvent>((evt) =>
            {
                scrollView1.scrollOffset = new Vector2(0, scrollView1.scrollOffset.y + 1000 * evt.delta.y);
                evt.StopPropagation();
            }
             );
#endif
            Show(false);
        }

        public void Init()
        {
            VisualElement tmpGoodsVisual;
            GoodsWidget tmpGoodsWidget;

            List<ProductCatalogItem> productIDs = IAPManager.Link.ProductCatalog.allProducts.ToList();
            int i = 0;

            foreach (var crystalPack in DataHolder.Instance.CrystalPacks)
            {
                tmpGoodsVisual = goodsWidget.CloneTree();
                crystalsRoot.Add(tmpGoodsVisual);
                tmpGoodsWidget = new GoodsWidget(tmpGoodsVisual);
                tmpGoodsWidget.InitForIAP(UIHelper.Instance.GetSprite("Crystal_icon"),
                                    "x" + crystalPack.CrystalAmount,
                                    productIDs[i].id,
                                    IAPManager.GetProductCost(productIDs[i].id),
                                    UIHelper.Instance.GoodsBg,
                                    UIHelper.Instance.GoodsCrystalSpyralColor,
                                    UIHelper.Instance.GoodsCrystalBgColor,
                                    UIHelper.Instance.GetSprite(crystalPack.ID));
                goods.Add(tmpGoodsWidget);
                i++;
            }

            i = 0;
            foreach (var buyableMultiplier in DataHolder.Instance.BuyableMultipliers)
            {
                tmpGoodsVisual = goodsWidget.CloneTree();
                multipliersRoot.Add(tmpGoodsVisual);
                tmpGoodsWidget = new GoodsWidget(tmpGoodsVisual);
                string id = buyableMultiplier.ID;
                tmpGoodsWidget.Init(UIHelper.Instance.GetSprite("Time_icon"),
                                    "for " + Utilities.GetTimeString(buyableMultiplier.Duration),
                                    buyableMultiplier.CrystalPrice,
                                    () => BuyMultiplier(id),
                                    UIHelper.Instance.GoodsBg,
                                    UIHelper.Instance.GoodsMultiplierSpyralColor,
                                    UIHelper.Instance.GoodsMultiplierBgColor,
                                    UIHelper.Instance.GetSprite(buyableMultiplier.ID),
                                    UIHelper.Instance.GetSprite("Crystal_icon"));
                goods.Add(tmpGoodsWidget);
                i++;
            }

            i = 0;
            foreach (var buyableTimeSkip in DataHolder.Instance.BuyableTimeSkips)
            {
                tmpGoodsVisual = goodsWidget.CloneTree();
                timeSkipRoot.Add(tmpGoodsVisual);
                tmpGoodsWidget = new GoodsWidget(tmpGoodsVisual);
                string id = buyableTimeSkip.ID;
                tmpGoodsWidget.Init(UIHelper.Instance.GetSprite("Notes_icon"),
                                    Utilities.GetNotesString(buyableTimeSkip.Income),
                                    buyableTimeSkip.CrystalPrice,
                                    () => BuyTimeSkip(id),
                                    UIHelper.Instance.TimeJumpBg,
                                    UIHelper.Instance.GoodsTimeJumpSpyralColor,
                                    UIHelper.Instance.GoodsTimeJumpBgColor,
                                    UIHelper.Instance.GetSprite(buyableTimeSkip.ID),
                                    UIHelper.Instance.GetSprite("Crystal_icon"));
                goods.Add(tmpGoodsWidget);
                i++;
            }
        }

        private void BuyMultiplier(string id)
        {
            BuyableMultiplierData buyableMultiplier = DataHolder.Instance.GetBuyableMultiplierData(id);
            string localization = "x" + buyableMultiplier.Multiplier + " for " + Utilities.GetTimeString(buyableMultiplier.Duration);
            Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UIManager.PanelType.PurchaseWindow, new Pair<string, string> { Value1 = id, Value2 = localization });
        }

        private void BuyTimeSkip(string id)
        {
            BuyableSkipTimeData buyableTimeSkip = DataHolder.Instance.GetBuyableSkipTimeData(id);
            Messenger<UIManager.PanelType, object>.Broadcast(GameEvents.ShowPanel, UIManager.PanelType.PurchaseWindow, new Pair<string, string> { Value1 = id, Value2 = Utilities.GetNotesString(buyableTimeSkip.Income) });
        }

        public void DisposeShopPanel()
        {
            goods.ForEach(x => x.Dispose());
            Messenger.RemoveListener(GameEvents.UpdateLocalization, OnUpdateLocalization);
        }

        private void OnUpdateLocalization()
        {
            crystalsLabel.text = LocalizationManager.GetTranslation("Crystals");
            multipliersLabel.text = LocalizationManager.GetTranslation("Multipliers");
            timeSkipLabel.text = LocalizationManager.GetTranslation("TimeSkipers");
        }

        public void Show(bool show) => shopPanel.visible = show;

        private void Update()
        {
            if (!IsShown) return;

            goods.ForEach(x => x.Update());
        }
    }
}
