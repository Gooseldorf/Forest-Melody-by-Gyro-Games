using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Sirenix.Serialization;
using SO_Scripts;
using UnityEngine;

[Serializable]
public class PlayerData : IDeserializationCallback
{
    private static PlayerData instance;
    public static string SaveFilePath = Application.persistentDataPath + "/test.fun";

    [field: SerializeField] public int PlayerLevel { get; private set; }
    [field: SerializeField] public double Experience { get; private set; }
    [field: SerializeField] public double Notes { get; private set; }
    [field: SerializeField] public int Crystals { get; private set; }
    [field: SerializeField] public List<Tree> Trees { get; private set; }
    [field: SerializeField] public IncomeBooster IncomeBooster { get; private set; }
    [field: SerializeField] public MultiplierBooster MultiplierBooster { get; private set; }
    [field: SerializeField] public TapBooster TapBooster { get; private set; }
    [field: SerializeField] public int OfflineBoosterLevel { get; private set; }
    [field: SerializeField] public List<BuyableMultiplier> ActiveMultipliers { get; private set; }
    [field: SerializeField] public List<string> Inventory { get; private set; } = new();
    [field: SerializeField] public DateTime GameStart { get; private set; }
    [field: SerializeField] public int CurrentTreeIndex { get; private set; } = 0;

    public double ExperienceForNextLevel { get; private set; }
    public float Multiplier { get; private set; }
    public Tree CurrentTree => Trees[CurrentTreeIndex];
    
    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
                throw new Exception("trying to call PlayerData.Instance before load, fix it");

            return instance;
        }
    }

    private PlayerData(string firstTreeName)
    {
        PlayerLevel = 1;
        Experience = 0;
        ExperienceForNextLevel = DataHolder.Instance.GetExperienceForNextLevel(PlayerLevel);
        Notes = 100;
        Crystals = 70;
        Trees = new List<Tree>();
        Trees.Add(new Tree(firstTreeName));
        IncomeBooster = new IncomeBooster(1);
        MultiplierBooster = new MultiplierBooster(1);
        TapBooster = new TapBooster(1);
        OfflineBoosterLevel = 1;
        ActiveMultipliers = new List<BuyableMultiplier>();
        Inventory = new();
        GameStart = DateTime.Now;
    }

    public void Save()
    {
        byte[] bytes = SerializationUtility.SerializeValue(this, DataFormat.Binary);
        File.WriteAllBytes(SaveFilePath, bytes);
    }

    public static void Load()
    {
        if (!File.Exists(SaveFilePath))
        {
            instance = new PlayerData("test_tree_1");
            Debug.Log("Player data created");
            instance.CurrentTree.AddBird(new Bird(DataHolder.Instance.GetTreeData("test_tree_1").BirdsData[0].ID));
            instance.Save();
        }
        else
        {
            byte[] bytes = File.ReadAllBytes(SaveFilePath);
            instance = SerializationUtility.DeserializeValue<PlayerData>(bytes, DataFormat.Binary);
            Debug.Log("Player data loaded");
        }
    }

    public void OnDeserialization(object sender)
    {
        ExperienceForNextLevel = DataHolder.Instance.GetExperienceForNextLevel(PlayerLevel);
    }

    public void LevelUp()
    {
        PlayerLevel++;
        ExperienceForNextLevel = DataHolder.Instance.GetExperienceForNextLevel(PlayerLevel);
        Experience = 0;
        Messenger.Broadcast(GameEvents.OnPlayerLevelUp, MessengerMode.DONT_REQUIRE_LISTENER);
    }

    public void AddExperience(double value)
    {
        if (Experience >= ExperienceForNextLevel)
        {
            Experience = ExperienceForNextLevel;
            return;
        }
        value += value * CurrentTree.GetDecorBonus("decor_experience");
        Experience += value;
    }

    public void ChangeNotes(double value)
    {
        Notes += value;
    }

    public void ChangeCrystals(int value)
    {
        Crystals += value;
    }

    public void AddToInventory(string dataID) => Inventory.Add(dataID);

    public void RemoveFromInventory(string dataID) => Inventory.Remove(dataID);

    public void AddActiveBuyableMultiplier(BuyableMultiplier multiplier)
    {
        ActiveMultipliers.Add(multiplier);
        Messenger<int, bool>.Broadcast(GameEvents.OnBuybleMultiplierBoosterAction, multiplier.Multiplier, true);
    }

    public void LevelUpOfflineBooster(int levels = 1) => OfflineBoosterLevel += levels;

    public void CalculateMultiplier()
    {
        Multiplier = 0;

        for (int i = ActiveMultipliers.Count - 1; i >= 0; i--)
        {
            if((ActiveMultipliers[i] as ICooldownable).IsActive)
                Multiplier += ActiveMultipliers[i].Multiplier;
            else
            {
                ActiveMultipliers.Remove(ActiveMultipliers[i]);
                Messenger<int, bool>.Broadcast(GameEvents.OnBuybleMultiplierBoosterAction, ActiveMultipliers[i].Multiplier, false);
            }
        }
        if ((MultiplierBooster as ICooldownable).IsActive)
            Multiplier += MultiplierBooster.Multiplier;
        if (Multiplier < 1)
            Multiplier = 1;
        Multiplier += Multiplier * CurrentTree.GetDecorBonus("decor_notes_production");
    }
}
