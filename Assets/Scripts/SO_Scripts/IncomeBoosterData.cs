using System;
using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(IncomeBoosterData), menuName = "ScriptableObjects/OneTime/" + nameof(IncomeBoosterData))]
    public class IncomeBoosterData : ScriptableObject
    {
        [SerializeField] private int bonusA;
        [SerializeField] private float bonusPOW;
        [SerializeField] private int levelUpCostA;
        [SerializeField] private float levelUpCostPOW;
        [SerializeField] private int coolDown;

        public double GetBonus(int level)
        {
            double result = bonusA * Math.Pow(level, bonusPOW);
            return result + result * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_income_booster");
        }

        public float GetCD() => coolDown;

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
