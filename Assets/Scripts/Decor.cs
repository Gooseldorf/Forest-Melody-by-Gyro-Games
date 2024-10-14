using System;
using System.Runtime.Serialization;
using SO_Scripts;
using UnityEngine;

[Serializable]
public class Decor: IPosition, IDeserializationCallback
{
    [field: SerializeField] public string DataID { get; private set; }
    private DecorData data;

    public int CrystalCost => data.CrystalCost;
    [field: SerializeField] public int BranchPosition { get; set; }
    [field: SerializeField] public int PositionIndex { get; set; }

    public Decor(string dataID)
    {
        DataID = dataID;
        BranchPosition = -1;
        PositionIndex = -1;
        data = DataHolder.Instance.GetDecorData(dataID);
    }

    public void OnDeserialization(object sender)
    {
        data = DataHolder.Instance.GetDecorData(DataID);
    }
    
    public void Place(int branchPosition, int positionIndex)
    {
        BranchPosition = branchPosition;
        PositionIndex = positionIndex;
        PlayerData.Instance.Save();
    }

    public float GetBonus()
    {
        if (BranchPosition == -1 || PositionIndex == -1) return 0;
        return data.Percent;
    }
}
