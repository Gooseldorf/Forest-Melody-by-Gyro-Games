using System;
using System.Runtime.Serialization;
using I2.Loc;
using SO_Scripts;
using UnityEngine;

[Serializable]
public class MultiplierBooster : FreeBooster
{
    private MultiplierBoosterData data;

    public override float Duration => data.GetDuration(Level);// { get; private set; }

    public int Multiplier => data.Multiplier;

    public MultiplierBooster(int level) : base(level) { }

    public override void OnDeserialization(object sender)
    {
        data = DataHolder.Instance.MultiplierBoosterData;
        Cd = data.GetCD(Level);
    }
    public override void LevelUp(int levels = 1)
    {
        base.LevelUp(levels);
        //Duration = data.GetDuration(Level);
        Cd = data.GetCD(Level);
        PlayerData.Instance.Save();
    }

    public override double GetLevelUpCost(int levels = 1) => data.GetLevelUpCost(Level, levels);

    public override string GetDescription()
    {
        return String.Format(LocalizationManager.GetTranslation("MultiplierBooster_desc"), Multiplier, Duration);
    }
}
