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
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        var json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SaveFileName(), json);

        Debug.Log("Game Saved: " + SaveFileName());
    }

    public static bool TryLoad()
    {
        var path = SaveFileName();

        if (!File.Exists(path)) return false;

        try
        {
            var json = File.ReadAllText(path);

            if (string.IsNullOrEmpty(json)) return false;

            saveData = JsonUtility.FromJson<SaveData>(json);

            if (PlayerView.Instance != null)
            {
                PlayerView.Instance.PlayerModel = PlayerModel.CreateFromSave(ref saveData.PlayerSaveData);

                // Если нужно загрузить позицию игрока из сохранения
                // PlayerView.Instance.transform.position = ...
                
                return true;
            }
            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка чтения сохранения: " + e.Message);
            return false;
        }
    }

    public static void DeleteSave()
    {
        var path = SaveFileName();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted.");
        }
    }

    private static void HandleSaveData()
    {
        if (PlayerView.Instance != null && PlayerView.Instance.PlayerModel != null)
        {
            PlayerView.Instance.PlayerModel.Save(ref saveData.PlayerSaveData);
        }
    }
}
