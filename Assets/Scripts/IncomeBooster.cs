using System;
using System.Runtime.Serialization;
using I2.Loc;
using SO_Scripts;
using UnityEngine;

[Serializable]
public class IncomeBooster : FreeBooster
{
    private IncomeBoosterData data;

    public IncomeBooster(int level) : base(level) { }

    public override void OnDeserialization(object sender)
    {
        data = DataHolder.Instance.IncomeBoosterData;
        Cd = data.GetCD();
        Duration = 0;
    }

    public override void Activate()
    {
        base.Activate();
        PlayerData.Instance.ChangeNotes(data.GetBonus(Level));
    }

    public override void LevelUp(int levels = 1)
    {
        base.LevelUp(levels);
        Cd = data.GetCD();
        PlayerData.Instance.Save();
    }

    public override double GetLevelUpCost(int levels = 1) => data.GetLevelUpCost(Level, levels);

    public override string GetDescription()
    {
        return String.Format(LocalizationManager.GetTranslation("IncomeBooster_desc"), Utilities.GetNotesString(data.GetBonus(Level)));
    }
}
