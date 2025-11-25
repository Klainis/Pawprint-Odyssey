using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerSaveData;
        public WallSaveData WallSaveData;
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

            if (CreatePlayerModel(ref saveData.PlayerSaveData) &&
                DestroyBrokenWalls(ref saveData.WallSaveData)) return true;

            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка чтения сохранения: " + e.Message);
            return false;
        }
    }

    private static bool CreatePlayerModel(ref PlayerSaveData data)
    {
        if (PlayerView.Instance != null)
        {
            PlayerView.Instance.PlayerModel = PlayerModel.CreateFromSave(ref data);

            // Если нужно загрузить позицию игрока из сохранения
            // PlayerView.Instance.transform.position = ...

            return true;
        }
        return false;
    }

    private static bool DestroyBrokenWalls(ref WallSaveData data)
    {
        if (WallsManager.Instance != null)
        {
            WallsManager.Instance.WallsExistenceInstance = WallsExistence.CreateWallsExistence(ref data);
            WallsManager.DestroyBrokenWalls();
            return true;
        }
        return false;
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

        if (WallsManager.Instance != null && WallsManager.Instance.WallsExistenceInstance != null)
        {
            WallsManager.Instance.WallsExistenceInstance.Save(ref saveData.WallSaveData);
        }
    }
}
