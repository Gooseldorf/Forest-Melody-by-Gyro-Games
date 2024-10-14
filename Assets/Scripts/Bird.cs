using System;
using System.Runtime.Serialization;
using SO_Scripts;
using UnityEngine;

[Serializable]
public class Bird: IPosition, IDeserializationCallback
{
    [field: SerializeField] public string DataID { get; private set; }
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int BranchPosition { get; set; }
    [field: SerializeField] public int PositionIndex { get; set; }
    
    [SerializeField] private float timeOffset;
    [SerializeField] private float placementTime;
    [field: SerializeField] public float HatchTime { get; private set; }
    private float timer;
    public BirdData Data { get; private set; }
    public ContainerType ContainerType => Data.Type;

    public event Action<double, bool> NotesGain;
    public event Action<int, int> OnLevelUp;

    public double Notes { get; private set; }
    public float Timer => timer;

    public Bird(string dataID)
    {
        DataID = dataID;
        Data = DataHolder.Instance.GetBirdData(dataID);
        Level = 0;
        Notes = Data.GetNotes(Level);
        BranchPosition = -1;
        PositionIndex = -1;
    }

    public void OnDeserialization(object sender)
    {
        Data = DataHolder.Instance.GetBirdData(DataID);
        Notes = Data.GetNotes(Level);
        timer = timeOffset;
    }

    public void Place(int branchPosition, int positionIndex)
    {
        BranchPosition = branchPosition;
        PositionIndex = positionIndex;
        timeOffset = TimeManager.Instance.GetTimeOffset();
        timer = timeOffset;
        placementTime = TimeManager.Instance.CurrentTime;
        HatchTime = placementTime + Data.IncubationTime;
        PlayerData.Instance.Save();
    }

    public void LevelUp(int levels = 1)
    {
        OnLevelUp?.Invoke(Level, Level + levels);
        Level += levels;
        Notes = Data.GetNotes(Level);
        PlayerData.Instance.Save();
    }

    public void GenerateNotesWithOffset()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            timer = 1;
            GetNotes();
        }
    }

    public void GetNotes(bool isTap = false)
    {
        if (PositionIndex == -1 || BranchPosition == -1 || Level == 0) return;
        NotesGain?.Invoke(Notes * PlayerData.Instance.Multiplier, isTap);
        PlayerData.Instance.ChangeNotes(Notes * PlayerData.Instance.Multiplier);
        PlayerData.Instance.AddExperience(Notes * PlayerData.Instance.Multiplier);
    }
}
