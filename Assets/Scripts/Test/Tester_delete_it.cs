using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Windows;
using System.IO;
using SO_Scripts;
using File = System.IO.File;

public class Tester_delete_it : MonoBehaviour
{

#if UNITY_EDITOR
    
    public int PlayerLevel;
    public int Trees;
    public double Notes;
    public int Crystals;
    [SerializeField] 
    private Condition testCondition;
    [SerializeField] 
    private TreeData testTreeData;
    [SerializeField] private Bird bird;
    [SerializeField] private BirdData birdData;
    
    [Button]
    private void CheckCondition()
    {
        PlayerData playerData = PlayerData.Instance;
        
        for (int i = 0; i < Trees; i++)
        {
            playerData.Trees.Add(new Tree("test_tree_1"));
        }
        
        Debug.Log(testCondition.CheckCondition().ToString());
        //Debug.Log(testCondition.GetCondition(out double notes, out int crystals));
    }
    
    [Button]
    private void CheckBird()
    {
        Debug.Log(bird.Notes);
    }
    
    [Button]
    private void SaveJson()
    {
        /*Bird testBird = new Bird("testBirdData", 0.5f);
        var birdSerialized = JsonConvert.SerializeObject(testBird);
        File.WriteAllText(Application.persistentDataPath + "/Json.json",birdSerialized);
        Debug.Log(Application.persistentDataPath);*/
    }

    [Button]
    private void LoadJson()
    {
        /*string text = File.ReadAllText(Application.persistentDataPath + "/Json.json");
        Bird testBird = new Bird("testBirdData", 99);
        testBird = JsonConvert.DeserializeObject<Bird>(text);*/
    }
#endif
}
