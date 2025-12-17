using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }

    private List<string> OpenedRooms = new();
    private List<string> OpenedDestructibleWalls = new();
    private GameObject _mapCanvasInstance;
    private Transform _mapIconTransform;
    private Transform _roomsContainer;

    private readonly string mapIconObjName = "PlayerIconImage";
    private readonly string startRoomName = "F_Room_Tutorial";
    private readonly string roomsContainerName = "RoomsImages";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;

        if (!OpenedRooms.Contains(startRoomName))
            OpenedRooms.Add(startRoomName);
    }

    public void SetMapCanvasInstance(GameObject obj)
    {
        _mapCanvasInstance = obj;
        _mapIconTransform = _mapCanvasInstance.transform.Find(mapIconObjName);
        _roomsContainer = _mapCanvasInstance.transform.Find(roomsContainerName);
    }

    public void SetMapIcon(string roomName)
    {
        if (_mapCanvasInstance == null)
        {
            Debug.LogError("MapManager: _mapCanvasInstance == null!");
            return;
        }

        var roomTransform = _roomsContainer.Find(roomName);
        if (roomTransform != null)
            _mapIconTransform.transform.position = roomTransform.position;
    }

    #region Show Opened Rooms & Walls

    public void ShowAllOpenedRoomsAndWalls()
    {
        foreach (var roomName in OpenedRooms)
            ShowOpenedObj(roomName);
        foreach (var wallID in OpenedDestructibleWalls)
            ShowOpenedObj(wallID);
    }

    public void OpenRoom(string roomName)
    {
        if (!OpenedRooms.Contains(roomName))
        {
            OpenedRooms.Add(roomName);
            ShowOpenedObj(roomName);
        }
    }

    public void OpenDestructibleWall(string wallID)
    {
        if (!OpenedDestructibleWalls.Contains(wallID))
        {
            OpenedDestructibleWalls.Add(wallID);
            ShowOpenedObj(wallID);
        }
    }

    private void ShowOpenedObj(string objName)
    {
        var objTransform = _roomsContainer.Find(objName);
        if (objTransform != null)
            objTransform.gameObject.SetActive(true);
        else
            Debug.LogWarning($"MapManager: Объект (комната/стена) с именем '{objName}' не найден в {_mapCanvasInstance.name}");
    }

    #endregion

    #region Save & Load

    public void Save(ref MapRoomsSaveData data)
    {
        data.OpenedRooms = new List<string>(OpenedRooms);
        data.OpenedDestructibleWalls = new List<string>(OpenedDestructibleWalls);
    }

    public void LoadAndShow(ref MapRoomsSaveData data)
    {
        OpenedRooms.Clear();
        OpenedDestructibleWalls.Clear();

        if (data.OpenedRooms != null)
            OpenedRooms = new List<string>(data.OpenedRooms);
        else
        {
            OpenedRooms = new List<string>();
            if (!OpenedRooms.Contains(startRoomName))
                OpenedRooms.Add(startRoomName);
            Debug.LogWarning("MapManager: Список комнат в сохранении был null.");
        }

        if (data.OpenedDestructibleWalls != null)
            OpenedDestructibleWalls = new List<string>(data.OpenedDestructibleWalls);

        ShowAllOpenedRoomsAndWalls();
    }

    #endregion
}

[System.Serializable]
public struct MapRoomsSaveData
{
    public List<string> OpenedRooms;
    public List<string> OpenedDestructibleWalls;
}
