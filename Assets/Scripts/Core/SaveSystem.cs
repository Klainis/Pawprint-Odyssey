using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerSaveData;
    }

    public static string SaveFileName()
    {
        var saveFile = Application.persistentDataPath + "/save" + ".save";
        Debug.Log(saveFile);
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    private static void HandleSaveData()
    {
        PlayerView.Instance.PlayerModel.Save(ref saveData.PlayerSaveData);
    }

    public static void Load()
    {
        var saveContent = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    public static void HandleLoadData()
    {
        PlayerView.Instance.PlayerModel.Load(saveData.PlayerSaveData);
    }
}
