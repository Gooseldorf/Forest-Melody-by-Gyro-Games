using System.Runtime.Serialization;
using UnityEngine;

public abstract class FreeBooster : IDeserializationCallback, ICooldownable
{
    [field: SerializeField] public float ActivationTime { get; protected set; }
    [field: SerializeField] public int Level { get; protected set; }

    public virtual float Duration { get; protected set; }

    public float Cd { get; protected set; }

    public FreeBooster(int level)
    {
        Level = level;
        ResetCd();
        OnDeserialization(null);
    }

    public abstract void OnDeserialization(object sender);

    public void ResetCd() => ActivationTime = float.MinValue;

    public virtual void Activate()
    {
        ActivationTime = TimeManager.Instance.CurrentTime;
        PlayerData.Instance.Save();
    }

    public virtual void LevelUp(int levels = 1) => Level += levels;

    public abstract double GetLevelUpCost(int levels = 1);

    public abstract string GetDescription();
}
