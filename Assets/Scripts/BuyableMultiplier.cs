using System;
using System.Runtime.Serialization;
using SO_Scripts;
using UnityEngine;

[Serializable]
public class BuyableMultiplier : IDeserializationCallback, ICooldownable
{
    [field: SerializeField] public string dataID { get; private set; }
    [field: SerializeField] public float ActivationTime { get; private set; }

    private BuyableMultiplierData data;

    public float Duration => data.Duration;
    public int Multiplier => data.Multiplier;
    public float Cd => 0;

    public BuyableMultiplier(string dataID)
    {
        this.dataID = dataID;
        OnDeserialization(null);
    }

    public void OnDeserialization(object sender)
    {
        data = DataHolder.Instance.GetBuyableMultiplierData(dataID);
    }

    public void Activate()
    {
        PlayerData.Instance.AddActiveBuyableMultiplier(this);
        PlayerData.Instance.RemoveFromInventory(dataID);
        ActivationTime = TimeManager.Instance.CurrentTime;
        PlayerData.Instance.Save();
    }
}
