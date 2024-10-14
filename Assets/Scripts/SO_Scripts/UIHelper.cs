using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UI
{
    [CreateAssetMenu(fileName = nameof(UIHelper), menuName = "ScriptableObjects/OneTime/" + nameof(UIHelper))]
    public class UIHelper : ScriptableObjSingleton<UIHelper>
    {
        [field: SerializeField]
        public Color ActiveColor { get; private set; }
        [field: SerializeField]
        public Color InactiveColor { get; private set; }
        [field: SerializeField]
        public Color UpgradeActiveColor { get; private set; }
        [field: SerializeField]
        public Color DecorActiveColor { get; private set; }
        [field: SerializeField]
        public Color DecorPlantedColor { get; private set; }

        [SerializeField]
        private SpriteAtlas spritesAtlas;
        [SerializeField]
        private SpriteAtlas birdsAtlas;
        [SerializeField]
        private SpriteAtlas decorAtlas;

        public Sprite GetSprite(string spriteName) => spritesAtlas.GetSprite(spriteName);

        public Sprite GetBirdSprite(string spriteName) => birdsAtlas.GetSprite(spriteName);

        public Sprite GetDecorSprite(string spriteName) => decorAtlas.GetSprite(spriteName);

        [field: SerializeField]
        public Sprite TimeJumpBg { get; private set; }
        [field: SerializeField]
        public Sprite GoodsBg { get; private set; }

        [field: SerializeField]
        public Color GoodsCrystalBgColor { get; private set; }
        [field: SerializeField]
        public Color GoodsCrystalSpyralColor { get; private set; }
        [field: SerializeField]
        public Color GoodsMultiplierBgColor { get; private set; }
        [field: SerializeField]
        public Color GoodsMultiplierSpyralColor { get; private set; }
        [field: SerializeField]
        public Color GoodsTimeJumpBgColor { get; private set; }
        [field: SerializeField]
        public Color GoodsTimeJumpSpyralColor { get; private set; }
        [field: SerializeField]
        public Color MultiplierBoosterBgColor { get; private set; } = new Color(0.52156862745098039215686274509804f, 0.87058823529411764705882352941176f, 0.34901960784313725490196078431373f);
        [field: SerializeField]
        public Color TapBoosterBgColor { get; private set; } = new Color(0.49019607843137254901960784313725f, 0.92941176470588235294117647058824f, 0.74117647058823529411764705882353f);
        [field: SerializeField]
        public Color IncomeBoosterBgColor { get; private set; } = new Color(1, 0.83921568627450980392156862745098f, 0.42745098039215686274509803921569f);
        [field: SerializeField]
        public Color OfflneBoosterBgColor { get; private set; }
    }
}
