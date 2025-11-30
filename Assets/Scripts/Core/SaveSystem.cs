using UnityEngine;
using System.IO;

public class SaveSystem
{
    public static int CurrentProfileIndex { get; set; } = 1;

    private static SaveData saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerSaveData;
        public WallSaveData WallSaveData;
        public CrystalSaveData CrystalSaveData;
    }

    public static string SaveFileName()
    {
        var saveFile = $"{Application.persistentDataPath}/Saves/Profile_{CurrentProfileIndex}/save_{CurrentProfileIndex}.dat";
        return saveFile;
    }

    // Можно использовать для изменения UI кнопки с уже имеющемся сохранением. Или наоборот.
    public static bool ProfileHasSave(int profileIndex)
    {
        var path = $"{Application.persistentDataPath}/Saves/Profile_{CurrentProfileIndex}/save_{profileIndex}.dat";
        return File.Exists(path);
    }

    public static void Save()
    {
        HandleSaveData();

        var json = JsonUtility.ToJson(saveData, true);

        var fullPath = SaveFileName();
        var directoryPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(SaveFileName(), json);

        Debug.Log($"SaveSystem: Игра сохранена в профиль {CurrentProfileIndex}: {SaveFileName()}");
    }

    public static bool TryLoad()
    {
        var path = SaveFileName();

        if (!File.Exists(path))
        {
            saveData = new SaveData();
            return false;
        }

        try
        {
            var json = File.ReadAllText(path);

            if (string.IsNullOrEmpty(json)) return false;

            saveData = JsonUtility.FromJson<SaveData>(json);

            if (CreatePlayerModel(ref saveData.PlayerSaveData) &&
                DestroyBrokenWalls(ref saveData.WallSaveData) &&
                DestroyBrokenCrystals(ref saveData.CrystalSaveData)) return true;

            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveSystem: Ошибка чтения сохранения: " + e.Message);
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

    private static bool DestroyBrokenCrystals(ref CrystalSaveData data)
    {
        if (CrystalsManager.Instance != null)
        {
            CrystalsManager.Instance.CrystalsExistenceInstance = CrystalsExistence.CreateCrystalsExistence(ref data);
            CrystalsManager.DestroyBrokenCrystals();
            return true;
        }
        return false;
    }

    private static void HandleSaveData()
    {
        if (PlayerView.Instance != null && PlayerView.Instance.PlayerModel != null)
        {
            saveData.PlayerSaveData.FacingRight = true;
            PlayerView.Instance.PlayerModel.Save(ref saveData.PlayerSaveData);
        }

        if (WallsManager.Instance != null && WallsManager.Instance.WallsExistenceInstance != null)
        {
            WallsManager.Instance.WallsExistenceInstance.Save(ref saveData.WallSaveData);
        }

        if (CrystalsManager.Instance != null && CrystalsManager.Instance.CrystalsExistenceInstance != null)
        {
            CrystalsManager.Instance.CrystalsExistenceInstance.Save(ref saveData.CrystalSaveData);
        }
    }
}
