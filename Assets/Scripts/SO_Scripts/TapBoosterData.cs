using System;
using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(TapBoosterData), menuName = "ScriptableObjects/OneTime/" + nameof(TapBoosterData))]
    public class TapBoosterData : ScriptableObject
    {
        [field: SerializeField] public int TapsPerSecond { get; private set; }
        [Space] [SerializeField] private int levelUpCostA;
        [SerializeField] private float levelUpCostPOW;
        [SerializeField] private int coolDownA;
        [SerializeField] private int durationA;

        public float GetDuration(int level)
        {
            float result = level + durationA;
            return result + result * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_tap_booster");
        }

        public float GetCD(int level) => level + coolDownA;
        //public float GetCD(int level) => 5; // TODO forTests (REMOVE)

        public double GetLevelUpCost(int level, int levels = 1)
        {
            double result = 0, oneLevelCost;
            for (int i = 0; i < levels; i++)
            {
                oneLevelCost = levelUpCostA * Math.Pow(level, levelUpCostPOW);
                oneLevelCost += oneLevelCost * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_boosters_level_up");
                result += oneLevelCost;
            }
            return result;
        }
    }
}
