using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SO_Scripts;
using UnityEngine;

[Serializable]
public class Tree : IDeserializationCallback
{
    private const int startNumberOfBranches = 2;
    private TreeData data;
    
    [field: SerializeField] public string DataId { get; private set; }
    [field: SerializeField] public List<Branch> Branches { get; private set; }
    [field: SerializeField] public List<Bird> Birds{ get; private set; }
    [field: SerializeField] public List<Decor> Decors{ get; private set; }
    [field: SerializeField] public float LastActivityTime { get; set; }

    public Tree(string dataID)
    {
        DataId = dataID;
        data = DataHolder.Instance.GetTreeData(dataID);
        Birds = new List<Bird>();
        Decors = new List<Decor>();
        Branches = new List<Branch>();

        for (int i = 0; i < startNumberOfBranches; i++)
        {
            Branches.Add(data.GetBranch(i));
        }
    }

    public void OnDeserialization(object sender) => data = DataHolder.Instance.GetTreeData(DataId);

    public void AddNewBranch(Branch branch)
    {
        Branches.Add(branch);
        PlayerData.Instance.Save();
    }

    public Branch GetBranch(int branchCount) => data.GetBranch(branchCount);

    public void AddBird(Bird bird)
    {
        Birds.Add(bird);
        PlayerData.Instance.Save();
    }

    public void AddDecor(Decor decor)
    {
        Decors.Add(decor);
        PlayerData.Instance.Save();
    }

    public double CalculateCleanIncomeInSecond()
    {
        double cleanIncomeInSecond = 0;
        foreach (var bird in Birds)
        {
            if(bird.BranchPosition != -1 && bird.PositionIndex != -1 && bird.HatchTime < LastActivityTime)
                cleanIncomeInSecond += bird.Notes;
        }
        
        return cleanIncomeInSecond;
    }

    private double CalculateOfflineHatch()
    {
        double cleanIncome = 0;
        foreach (var bird in Birds)
        {
            if (bird.HatchTime > LastActivityTime && bird.HatchTime < TimeManager.Instance.CurrentTime)
            {
                float activeTime = TimeManager.Instance.CurrentTime - bird.HatchTime;
                bird.LevelUp();
                cleanIncome += bird.Notes * DataHolder.Instance.OfflineBoosterData.GetMultiplier(PlayerData.Instance.OfflineBoosterLevel) * activeTime;
            }
        }
        return cleanIncome + cleanIncome * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_offline_notes_production");
    }

    public double CalculateOfflineIncome()
    {
        float timeInOffline = Mathf.Min(TimeManager.Instance.CurrentTime - LastActivityTime,
            DataHolder.Instance.OfflineBoosterData.OfflineIncomeTimeLimit);
        
        timeInOffline += GetDecorBonus("decor_add_time_to_offline_income");
        
        double offlineIncomeInSecond = DataHolder.Instance.OfflineBoosterData.GetOfflineBonusInSecond(PlayerData.Instance.OfflineBoosterLevel, CalculateCleanIncomeInSecond());
        offlineIncomeInSecond += offlineIncomeInSecond * PlayerData.Instance.CurrentTree.GetDecorBonus("decor_offline_notes_production");
       
        double totalIncome = timeInOffline * offlineIncomeInSecond + CalculateOfflineHatch();

        foreach (var multiplier in PlayerData.Instance.ActiveMultipliers)
        {
            float timeWithBuyableMultiplier =
                TimeManager.Instance.GetActiveTimeInOffline(multiplier.Duration, multiplier.ActivationTime, timeInOffline);
            totalIncome += timeWithBuyableMultiplier * offlineIncomeInSecond * multiplier.Multiplier;
        }

        MultiplierBooster multiplierBooster = PlayerData.Instance.MultiplierBooster;
        float timeWithMultiplierBooster =
            TimeManager.Instance.GetActiveTimeInOffline(multiplierBooster.Duration, multiplierBooster.ActivationTime, timeInOffline);
        totalIncome += timeWithMultiplierBooster * offlineIncomeInSecond * multiplierBooster.Multiplier;

        TapBooster tapBooster = PlayerData.Instance.TapBooster;
        float timeWithTapBooster = TimeManager.Instance.GetActiveTimeInOffline(tapBooster.Duration, tapBooster.ActivationTime, timeInOffline);

        totalIncome += timeWithTapBooster * (offlineIncomeInSecond + tapBooster.TapsPerSecond);

        return totalIncome;
    }

    public float GetDecorBonus(string decorID)
    {
        float decorBonus = Decors?.Find(x => x.DataID == decorID)?.GetBonus() ?? 0;
        if (decorBonus == 0)
            return 0;
        float decorationMultiplier = Decors?.Find(x => x.DataID == "decor_decoration_effect")?.GetBonus() ?? 0;

        return decorBonus + decorBonus * decorationMultiplier;
    }
}
