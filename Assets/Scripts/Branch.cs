using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class Branch 
{
    public List<ContainerType> ContainerTypes;
    public Condition UnlockCondition;
    public double UnlockCost;

    public Branch(List<ContainerType> containerTypes, Condition condition, double cost)
    {
        ContainerTypes = containerTypes;
        UnlockCondition = condition;
        UnlockCost = cost;
    }
}

