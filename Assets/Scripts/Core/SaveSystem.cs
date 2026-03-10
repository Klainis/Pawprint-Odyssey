using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static float increasePosYFromSave = 1f;

    public static int CurrentProfileIndex { get; set; } = 1;

    private static SaveData saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public string BuildVersion;
        public PlayerSaveData PlayerSaveData;
        public WallSaveData WallSaveData;
        public CrystalSaveData CrystalSaveData;
        public MapRoomsSaveData MapRoomsSaveData;
    }

    private static string SaveFileName()
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

    private static void SaveDataToFile()
    {
        var json = JsonUtility.ToJson(saveData, true);

        var fullPath = SaveFileName();
        var directoryPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(SaveFileName(), json);
    }

    public static void Save()
    {
        HandleSaveData();
        SaveDataToFile();
        Debug.Log($"SaveSystem.Save: Game saved to the profile {CurrentProfileIndex}: {CurrentProfileIndex}");
    }

    public static void AutoSave()
    {
        HandleAutoSaveData();
        SaveDataToFile();
        Debug.Log($"SaveSystem.AutoSave: Game saved to the profile {CurrentProfileIndex}: {CurrentProfileIndex}");
    }

    // Сохранение для того, чтобы сохранились сломанные стены и кристаллы, а также карта
    public static void AutoSaveBeforePlayerDeath()
    {
        HandleAutoSaveDataWithoutPlayerPos();
        SaveDataToFile();
        Debug.Log($"SaveSystem.AutoSaveWithoutPlayerPos: Game saved to the profile {CurrentProfileIndex}: {CurrentProfileIndex}");
    }

    public static void CrystalSave()
    {
        CrystalHandleSaveData();

        var json = JsonUtility.ToJson(saveData, true);

        var fullPath = SaveFileName();
        var directoryPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(SaveFileName(), json);

        Debug.Log($"SaveSystem.CrystalSave: Игра (Кристаллы) сохранена в профиль {CurrentProfileIndex}: {SaveFileName()}");
    }

    public static void MoneySave()
    {
        MoneyHandleSaveData();

        var json = JsonUtility.ToJson(saveData, true);

        var fullPath = SaveFileName();
        var directoryPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(SaveFileName(), json);

        Debug.Log($"SaveSystem.CrystalSave: Игра (Кристаллы) сохранена в профиль {CurrentProfileIndex}: {SaveFileName()}");
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

            if (Application.version != saveData.BuildVersion)
                return false;

            if (CreatePlayerModel(ref saveData.PlayerSaveData) &&
                DestroyBrokenWalls(ref saveData.WallSaveData) &&
                DestroyBrokenCrystals(ref saveData.CrystalSaveData) &&
                LoadOpenedMapRooms(ref saveData.MapRoomsSaveData)) return true;

            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SaveSystem: Ошибка чтения сохранения: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private static bool CreatePlayerModel(ref PlayerSaveData data)
    {
        if (PlayerView.Instance != null)
        {
            PlayerView.Instance.PlayerModel = PlayerModel.CreateFromSave(ref data);
            GameManager.Instance.CurrentScene = PlayerView.Instance.PlayerModel.CurrentScene;

            var checkPointPosFromSave = new Vector2(data.CheckPointPosX, data.CheckPointPosY + increasePosYFromSave);
            EntryPoint.Instance.SetPositionFromSave(checkPointPosFromSave);

            MapManager.Instance.SetMapIcon(PlayerView.Instance.PlayerModel.CurrentScene);
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

    private static bool LoadOpenedMapRooms(ref MapRoomsSaveData data)
    {
        if (MapManager.Instance != null)
        {
            MapManager.Instance.LoadAndShow(ref data);
            return true;
        }
        return false;
    }

    private static void HandleSaveData()
    {
        saveData.BuildVersion = Application.version;

        if (PlayerView.Instance != null && PlayerView.Instance.PlayerModel != null)
        {
            //Позиция записывается в PlayerModel в скрипте сейвки
            PlayerView.Instance.PlayerModel.SetCurrentScene(GameManager.Instance.CurrentScene);
            PlayerView.Instance.PlayerModel.Save(ref saveData.PlayerSaveData);
            Debug.Log($"{PlayerView.Instance.PlayerModel.CheckPointPosX}, {PlayerView.Instance.PlayerModel.CheckPointPosY}");
        }

        if (WallsManager.Instance != null && WallsManager.Instance.WallsExistenceInstance != null)
        {
            WallsManager.Instance.WallsExistenceInstance.Save(ref saveData.WallSaveData);
        }

        if (CrystalsManager.Instance != null && CrystalsManager.Instance.CrystalsExistenceInstance != null)
        {
            CrystalsManager.Instance.CrystalsExistenceInstance.Save(ref saveData.CrystalSaveData);
        }

        if (MapManager.Instance != null)
        {
            MapManager.Instance.Save(ref saveData.MapRoomsSaveData);
        }
    }

    private static void HandleAutoSaveData()
    {
        if (PlayerView.Instance != null && PlayerView.Instance.PlayerModel != null)
        {
            //Позиция
            var curPos = PlayerView.Instance.gameObject.transform.position;
            //var curPos = SafeGroundSaver.Instance.SafeGroundLocation;
            //if (curPos == Vector2.zero)
            //    curPos = PlayerView.Instance.gameObject.transform.position;
            PlayerView.Instance.PlayerModel.SetCurrentPosition(curPos.x, curPos.y);
            PlayerView.Instance.PlayerModel.SetCheckPointPosition(curPos.x, curPos.y);
            
            //Сцена
            PlayerView.Instance.PlayerModel.SetCurrentScene(GameManager.Instance.CurrentScene);
            PlayerView.Instance.PlayerModel.SetCheckPointScene(GameManager.Instance.CurrentScene);

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

        if (MapManager.Instance != null)
        {
            MapManager.Instance.Save(ref saveData.MapRoomsSaveData);
        }
    }

    private static void HandleAutoSaveDataWithoutPlayerPos()
    {
        if (WallsManager.Instance != null && WallsManager.Instance.WallsExistenceInstance != null)
        {
            WallsManager.Instance.WallsExistenceInstance.Save(ref saveData.WallSaveData);
        }

        if (CrystalsManager.Instance != null && CrystalsManager.Instance.CrystalsExistenceInstance != null)
        {
            CrystalsManager.Instance.CrystalsExistenceInstance.Save(ref saveData.CrystalSaveData);
        }

        if (MapManager.Instance != null)
        {
            MapManager.Instance.Save(ref saveData.MapRoomsSaveData);
        }
    }

    private static void CrystalHandleSaveData()
    {
        if (PlayerView.Instance != null && PlayerView.Instance.PlayerModel != null)
        {
            PlayerView.Instance.PlayerModel.Save(ref saveData.PlayerSaveData);
        }

        if (CrystalsManager.Instance != null && CrystalsManager.Instance.CrystalsExistenceInstance != null)
        {
            CrystalsManager.Instance.CrystalsExistenceInstance.Save(ref saveData.CrystalSaveData);
        }
        //Debug.Log($"{PlayerView.Instance.PlayerModel.CheckPointPosX}, {PlayerView.Instance.PlayerModel.CheckPointPosY}");
    }

    private static void MoneyHandleSaveData()
    {
        if (PlayerView.Instance != null && PlayerView.Instance.PlayerModel != null)
        {
            PlayerView.Instance.PlayerModel.Save(ref saveData.PlayerSaveData);
        }
    }
}
