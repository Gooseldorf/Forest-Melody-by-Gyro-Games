using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BirdsMenu
{
    [MenuItem("Birds/Player Data/Remove")]
    private static void RemoveData() => File.Delete(PlayerData.SaveFilePath);

    [MenuItem("Birds/Player Data/Remove", true)]
    private static bool ValidateRemoveData() => File.Exists(PlayerData.SaveFilePath) && !Application.isPlaying;

    [MenuItem("Birds/Player Data/LevelUp")]
    private static void LevelUp() => PlayerData.Instance.LevelUp();

    [MenuItem("Birds/Player Data/LevelUp100")]
    private static void LevelUp100()
    {
        for (int i = 0; i < 100; i++)
        {
            PlayerData.Instance.LevelUp();
        }
    }
    [MenuItem("Birds/Player Data/GiveNotes")]
    private static void GiveNotes() => PlayerData.Instance.ChangeNotes(999999999999999);
    
    [MenuItem("Birds/Player Data/GiveNotesAndLevelUp500")]
    private static void GiveNotesAndLevelUp500()
    {
        for (int i = 0; i < 500; i++)
        {
            PlayerData.Instance.LevelUp();
        }
        PlayerData.Instance.ChangeNotes(999999999999999);
    }
}
