using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(TreeData), menuName = "ScriptableObjects/" + nameof(TreeData))]
    public class TreeData : ScriptableObject
    {
        [SerializeField] private List<BirdData> birdsData;
        
        public int SpecialBranchFrequency;
        
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public Condition UnlockCondition { get; private set; }

        public IReadOnlyList<BirdData> BirdsData => birdsData;

        public BirdData GetBirdData(string id) => birdsData.Find(x => x.ID == id);

        #region Math
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private int thresholdLevel;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float notesCostA;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float notesCostB;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float notesCostC;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float notesCostD;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float notesCostE;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float lateNotesCostA;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float lateNotesCostB;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float lateNotesCostC;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float lateNotesCostD;
        [FoldoutGroup("Branch Condition Notes"), SerializeField] private float lateNotesCostE;

        [FoldoutGroup("Branch Condition Level"), SerializeField] private int lvlA;
        [FoldoutGroup("Branch Condition Level"), SerializeField] private int lvlB;
        #endregion

        private Condition GetNewBranchCondition(int branchCount)
        {
            return new Condition(
                playerLevel: CalculateNextBranchLevel(branchCount));
        }

        public Branch GetBranch(int branchCount)
        {
            return new Branch(
                containerTypes: GetNextBranchType(branchCount),
                condition: GetNewBranchCondition(branchCount + 1),
                cost: GetBranchCost(branchCount + 1));
        }
        private List<ContainerType> GetNextBranchType(int branchCount)
        {
            if (SpecialBranchFrequency != -1 && branchCount != 0 && (branchCount + 1) % SpecialBranchFrequency == 0)
            {
                int specialBranchCount = branchCount / SpecialBranchFrequency;
                if (specialBranchCount % 2 == 0)
                    return new List<ContainerType>() { ContainerType.Special, ContainerType.Common, ContainerType.Common};
                return new List<ContainerType>() { ContainerType.Common, ContainerType.Common, ContainerType.Special,};
            }
            return new List<ContainerType>() { ContainerType.Common , ContainerType.Common, ContainerType.Common, ContainerType.Common};
        }

        private double GetBranchCost(int branchCount)
        {
            double result;
            if (branchCount <= thresholdLevel)
            {
                result = notesCostA * Math.Pow(branchCount, 4)
                         + notesCostB * Math.Pow(branchCount, 3)
                         + notesCostC * Math.Pow(branchCount, 2)
                         + notesCostD * branchCount
                         + notesCostE;
            }
            else
            {
                result = lateNotesCostA * Math.Pow(branchCount, 4)
                       + lateNotesCostB * Math.Pow(branchCount, 3)
                       + lateNotesCostC * Math.Pow(branchCount, 2)
                       + lateNotesCostD * branchCount
                       + lateNotesCostE;
            }

            return result;
        }

        private int CalculateNextBranchLevel(int branchCount)
        {
            return lvlA * (branchCount + lvlB);
        }
    }
}
