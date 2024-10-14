using System;
using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(OfflineBoosterData), menuName = "ScriptableObjects/OneTime/" + nameof(OfflineBoosterData))]
    public class OfflineBoosterData : ScriptableObject
    {
        [SerializeField] private float bonusA;
        [SerializeField] private float bonusB;
        [SerializeField] private float levelUpCostA;
        [SerializeField] private float levelUpCostPOW;
        [SerializeField] private int offlineIncomeTimeLimit;

        public int OfflineIncomeTimeLimit => offlineIncomeTimeLimit * 3600;

        public double GetOfflineBonusInSecond(int level, double currentIncome)
        {
            double result = currentIncome * GetMultiplier(level);
            result += result * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_offline_income_booster");
            result = Math.Ceiling(result);
            return result;
        }

        public float GetMultiplier(int level)
        {
            return bonusA + (bonusB * level);
        }

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
