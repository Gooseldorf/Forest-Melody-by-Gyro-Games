using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(DataHolder), menuName = "ScriptableObjects/OneTime/" + nameof(DataHolder))]
    public class DataHolder : ScriptableObjSingleton<DataHolder>
    {
        [SerializeField] private List<TreeData> treesData;
        [SerializeField] private List<DecorData> decorsData;
        [SerializeField] private List<BuyableMultiplierData> buyableMultipliersData;
        [SerializeField] private List<BuyableSkipTimeData> buyableTimeSkipsData;
        [SerializeField] private List<CrystalPack> crystalPacks;
        [SerializeField] public List<AudioClip> soundsData;
        [SerializeField] public List<AudioClip> birdSounds;

        public IncomeBoosterData IncomeBoosterData;
        public TapBoosterData TapBoosterData;
        public MultiplierBoosterData MultiplierBoosterData;
        public OfflineBoosterData OfflineBoosterData;

        [FoldoutGroup("Next level experience"), SerializeField] private int thresholdLevel;
        [FoldoutGroup("Next level experience"), SerializeField] private int nextLvlExpA;
        [FoldoutGroup("Next level experience"), SerializeField] private float nextLvlExpPOW;
        [FoldoutGroup("Next level experience"), SerializeField] private float lateNextLvlExpA;
        [FoldoutGroup("Next level experience"), SerializeField] private float lateNextLvlExpPOW;

        [FoldoutGroup("Next level reward"), SerializeField] private int nextLvlRewardA;
        [FoldoutGroup("Next level reward"), SerializeField] private float nextLvlRewardPOW;

        public IReadOnlyList<BuyableMultiplierData> BuyableMultipliers => buyableMultipliersData;
        public IReadOnlyList<BuyableSkipTimeData> BuyableTimeSkips => buyableTimeSkipsData;
        public IReadOnlyList<CrystalPack> CrystalPacks => crystalPacks;

        public IReadOnlyList<DecorData> AllDecors => decorsData;

        public double GetExperienceForNextLevel(int level)
        {
            if (level < thresholdLevel)
            {
                return CalculateNextLevelExperience(level, nextLvlExpA, nextLvlExpPOW);
            }

            return CalculateNextLevelExperience(level, lateNextLvlExpA, lateNextLvlExpPOW);
        }

        private double CalculateNextLevelExperience(int level, float a, float POW) => a * Math.Pow(level, POW);

        public double GetRewardForLevelUp(int level)
        {
            double result = nextLvlRewardA * Math.Pow(level, nextLvlRewardPOW);
            return result + result * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_level_up_reward");
        }

        public BirdData GetBirdData(string id)
        {
            BirdData result;
            for (int i = 0; i < treesData.Count; i++)
            {
                result = treesData[i].GetBirdData(id);
                if (result != null)
                    return result;
            }
            return null;
        }

        public TreeData GetTreeData(string id) => treesData.Find(x => x.ID == id);

        public DecorData GetDecorData(string id) => decorsData.Find(x => x.ID == id);

        public BuyableMultiplierData GetBuyableMultiplierData(string id) => buyableMultipliersData.Find(x => x.ID == id);
        public bool ExistBuyableMultiplierData(string id) => buyableMultipliersData.Exists(x => x.ID == id);

        public BuyableSkipTimeData GetBuyableSkipTimeData(string id) => buyableTimeSkipsData.Find(x => x.ID == id);
        public bool ExistBuyableSkipTimeData(string id) => buyableTimeSkipsData.Exists(x => x.ID == id);

        public CrystalPack GetCrystalPack(string id) => crystalPacks.Find(x => x.ID == id);

    }
}