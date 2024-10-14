using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(BirdData), menuName = "ScriptableObjects/" + nameof(BirdData))]
    public class BirdData : ScriptableObject
    {
        public const int MaxLevel = 50;
        public int index;
        public ContainerType Type;
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public Condition Condition { get; private set; }

        public float IncubationTime
        {
            get
            {
                float result = hatchTimeA * Mathf.Pow(index, 2) + hatchTimeB * index + hatchTimeC;
                return result + result * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_hatch_time");
            }
        }
        public double BuildCost
        {
            get
            {
                double result = buildCostA * Math.Pow(index, 4)
                    + buildCostB * Math.Pow(index, 3)
                    + buildCostC * Math.Pow(index, 2)
                    + buildCostD * index
                    + buildCostE;
                return result + result * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_bird_cost");
            }
        }

        #region Math
        [FoldoutGroup("Hatch Time"), SerializeField] private float hatchTimeA;
        [FoldoutGroup("Hatch Time"), SerializeField] private float hatchTimeB;
        [FoldoutGroup("Hatch Time"), SerializeField] private float hatchTimeC;

        [FoldoutGroup("Build Cost"), SerializeField] private float buildCostA;
        [FoldoutGroup("Build Cost"), SerializeField] private float buildCostB;
        [FoldoutGroup("Build Cost"), SerializeField] private float buildCostC;
        [FoldoutGroup("Build Cost"), SerializeField] private float buildCostD;
        [FoldoutGroup("Build Cost"), SerializeField] private float buildCostE;

        [FoldoutGroup("Notes"), SerializeField] private float notesA;
        [FoldoutGroup("Notes"), SerializeField] private float notesB;

        [FoldoutGroup("Level Up Cost"), SerializeField] private int thresholdLevel;
        [FoldoutGroup("Level Up Cost"), SerializeField] private float lvlUpA;
        [FoldoutGroup("Level Up Cost"), SerializeField] private float lvlUpPOW;
        [FoldoutGroup("Level Up Cost"), SerializeField] private float lvlUpEPOW;
        [FoldoutGroup("Level Up Cost"), SerializeField] private float LateLvlUpA;
        [FoldoutGroup("Level Up Cost"), SerializeField] private float LateLvlUpPOW;
        [FoldoutGroup("Level Up Cost"), SerializeField] private float LateLvlUpEPOW;

        #endregion

        public double GetNotes(int level)
        {
            return (notesA * index + notesB) * level;
        }

        public double GetLevelUpCost(int level, int count)
        {
            double result = 0;
            if (level <= thresholdLevel)
                result = lvlUpA * Math.Pow(index, lvlUpPOW) * Math.Pow(Math.E, (lvlUpEPOW * level));
            else
                result = LateLvlUpA * Math.Pow(index, LateLvlUpPOW) * Math.Pow(Math.E, (LateLvlUpEPOW * level));
            
            return result + result * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_bird_level_up");
        }

        public double GetLevelUpCost(int level, int count, int countLevelUpLevels)
        {
            double result = 0;
            for (int i = 0; i < countLevelUpLevels; i++)
                result += GetLevelUpCost(level + i, count);

            return result;
        }
    }
}
