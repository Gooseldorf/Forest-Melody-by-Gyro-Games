using I2.Loc;
using System;
using UnityEngine;

[Serializable]
public class Condition
{
    [field: SerializeField] public int PlayerLevel { get; private set; }
    [field: SerializeField] public int TreeCount { get; private set; }

    public Condition(int playerLevel = 0, int treeCount = 0)
    {
        PlayerLevel = playerLevel;
        TreeCount = treeCount;
    }

    public bool CheckCondition()
    {
        PlayerData playerData = PlayerData.Instance;

        return (playerData.PlayerLevel >= PlayerLevel &&
                playerData.Trees.Count >= TreeCount);
    }

    public string GetCondition()
    {
        PlayerData playerData = PlayerData.Instance;
        if (playerData.PlayerLevel < PlayerLevel) return LocalizationManager.GetTranslation("Level") + " " + PlayerLevel;
        if (playerData.Trees.Count < TreeCount) return $"Open {TreeCount} tree to unlock!";

        return "Ready to buy!";
    }
}
