using System.Collections.Generic;
using Controllers;
using Sirenix.OdinInspector;
using SO_Scripts;
using UnityEngine;
using UnityEngine.UI;

public class TesterYar : MonoBehaviour
{
    [Button]
    private void TestSave()
    {
        PlayerData.Instance.Save();
        Debug.Log("Saved!");
    }

    [Button]
    private void TestLoad()
    {
        PlayerData.Load();
    }

    [Button]
    private void TestAddExperience(double value)
    {
        PlayerData.Instance.AddExperience(value);
        Debug.Log($"Experience:{PlayerData.Instance.Experience}");
    }
    
    [Button]
    private void TestLevelUp()
    {
        PlayerData.Instance.LevelUp();
        Debug.Log($"Level:{PlayerData.Instance.PlayerLevel}");
    }

    [Button]
    private void TestChangeNotes(double value)
    {
        PlayerData.Instance.ChangeNotes(value);
        Debug.Log($"Notes:{PlayerData.Instance.Notes}");
    }
    
    [Button]
    private void TestChangeCrystals(int value)
    {
        PlayerData.Instance.ChangeCrystals(value);
        Debug.Log($"Crystals:{PlayerData.Instance.Crystals}");
    }

    [Button]
    private void TestAddBird(string dataID, int treeIndex)
    {
        PlayerData.Instance.Trees[treeIndex].AddBird(new Bird(dataID));
        Debug.Log($"{dataID} added to tree {treeIndex}");
    }
    
    [Button]
    private void TestAddDecor(string dataID, int treeIndex)
    {
        PlayerData.Instance.Trees[treeIndex].Decors.Add(new Decor(dataID));
        Debug.Log($"{dataID} added to tree {treeIndex}");
    }
    
    [Button]
    private void TestAddTree(string dataID)
    {
        PlayerData.Instance.Trees.Add(new Tree(dataID));
        Debug.Log($"{dataID} added");
    }

    [Button]
    private void TestLevelUpBoosters()
    {
        PlayerData data = PlayerData.Instance;

        data.IncomeBooster.LevelUp();
        Debug.Log($"income booster: {PlayerData.Instance.IncomeBooster.Level}");
        data.MultiplierBooster.LevelUp();
        Debug.Log($"multiply booster: {PlayerData.Instance.MultiplierBooster.Level}");
        data.TapBooster.LevelUp();
        Debug.Log($"tap booster: {PlayerData.Instance.TapBooster.Level}");
        data.LevelUpOfflineBooster();
        Debug.Log($"offline booster: {PlayerData.Instance.OfflineBoosterLevel}");
    }
    
    [Button]
    private void TestActivateBoosters()
    {
        PlayerData data = PlayerData.Instance;

        //data.IncomeBooster.Activate();
        data.MultiplierBooster.Activate();
        data.TapBooster.Activate();
    }

    [Button]
    private void AddBuyable(string dataId)
    {
        PlayerData.Instance.AddToInventory(dataId);
        //Debug.Log( $"{dataId} : {PlayerData.Instance.Inventory.Find(x=> x.Value1 == dataId).Value2}");
    }
    
    [Button]
    private void ActivateBuyableMultiplier(string dataId)
    {
        var buyable = new BuyableMultiplier(dataId);
        buyable.Activate();
    }
    
    [Button]
    private void ActivateBuyableTimeSkip(string dataId)
    {
        DataHolder.Instance.GetBuyableSkipTimeData(dataId).Activate();
    }

    [Button]
    private void GetBirdData(string dataId)
    {
        BirdData data = DataHolder.Instance.GetBirdData(dataId);
    }

    [Button]
    private void GetTimeOffset()
    {
        Debug.Log(TimeManager.Instance.GetTimeOffset());
    }

    [Button]
    private string ShortenedStringTest(double value)
    {
        return Utilities.GetNotesString(value);
    }

    [Button]
    private void CheckDecor()
    {
        List<Decor> Decors = new List<Decor>() { new Decor("decor_level_up_reward"), new Decor("decor_decoration_effect") };
        
        Debug.Log(GetDecorBonus("decor_level_up_reward", Decors));
    }
    
    public float GetDecorBonus(string decorID, List<Decor> Decors)
    {
        float decorBonus = Decors?.Find(x => x.DataID == decorID)?.GetBonus() ?? 0;
        float decorationMultiplier = Decors?.Find(x => x.DataID == "decor_decoration_effect")?.GetBonus() ?? 0;

        return decorBonus + decorBonus * decorationMultiplier;
    }

    [Button]
    private void TestSounds()
    {
        /*SoundManager.Instance.PlayBirdSound("Owl");
        SoundManager.Instance.PlayBirdSound("Crow");
        SoundManager.Instance.PlayBirdSound("Catbird");*/
    }

    [SerializeField] private BirdController birdController;
    private BirdController bird;
    [Button]
    private void CreateBird()
    {
        RectTransform parent = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        bird = Instantiate(birdController, parent);
        bird.Init(new Bird("american_osprey"));
        bird.Bird.BranchPosition = 1;
        bird.Bird.PositionIndex = 1;
        bird.Bird.LevelUp(50);
    }
    
    [Button]
    private void TestBirdNotes()
    {
        bird.Bird.GetNotes();
    }

    [Button]
    private void TestContinuousSound()
    {
        SoundManager.Instance.PlaySound("IncomeBooster", 10);
    }
    
    public void GiveNotesAndLevelUp500()
    {
        for (int i = 0; i < 500; i++)
        {
            PlayerData.Instance.LevelUp();
        }
        PlayerData.Instance.ChangeNotes(999999999999999);
    }

    [SerializeField] private ScrollRect scrollRect;
    [Button]
    private void TestVisibleOnScreen()
    {
        // Get the viewport rect of the scroll rect
        Rect viewportRect = new Rect(scrollRect.viewport.rect.x, scrollRect.viewport.rect.y, scrollRect.viewport.rect.width, scrollRect.viewport.rect.height);

        // Loop through each child of the content transform
        
    }
    
    [Button]
    private void RemoveMultiplier(int multiplier)
    {
        BuyableMultiplier test = PlayerData.Instance.ActiveMultipliers.Find(x=> x.Multiplier == multiplier);
        PlayerData.Instance.ActiveMultipliers.Remove(test);
        Messenger<int, bool>.Broadcast(GameEvents.OnBuybleMultiplierBoosterAction, multiplier, false);
    }
}
