using System;
using I2.Loc;
using SO_Scripts;
using UnityEngine;

[Serializable]
public class TapBooster : FreeBooster
{
    private TapBoosterData data;

    public override float Duration => data.GetDuration(Level);//{ get; private set; }

    public int TapsPerSecond => data.TapsPerSecond;

    private float timer;
    private float offset;

    public TapBooster(int level) : base(level)
    {
        offset = (float)1 / DataHolder.Instance.TapBoosterData.TapsPerSecond;
    }

    public override void OnDeserialization(object sender)
    {
        data = DataHolder.Instance.TapBoosterData;
        //Duration = data.GetDuration(Level);
        Cd = data.GetCD(Level);
    }

    public override void Activate()
    {
        base.Activate();
        SoundManager.Instance.PlaySound("TapBooster", Duration);
    }

    public void ApplyEffect()
    {
        if (timer < offset)
            timer += Time.deltaTime;
        else
        {
            PlayerData.Instance.CurrentTree.Birds.ForEach(x => x.GetNotes(true));
            timer = 0;
        }
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
        return String.Format(LocalizationManager.GetTranslation("TapBooster_desc"), data.TapsPerSecond, Duration);
    }
}
