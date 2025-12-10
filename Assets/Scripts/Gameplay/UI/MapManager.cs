using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }

    private List<string> OpenedRooms = new();
    private GameObject _mapCanvasInstance;
    private Transform _mapIconTransform;

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
    }

    public void OpenRoomsFromSave(ref MapRoomsSaveData data)
    {
        OpenedRooms.Clear();

        if (data.OpenedRooms != null)
        {
            OpenedRooms = new List<string>(data.OpenedRooms);
        }
        else
        {
            OpenedRooms = new List<string>();
            if (!OpenedRooms.Contains(startRoomName))
                OpenedRooms.Add(startRoomName);
            Debug.LogWarning("MapManager: Список комнат в сохранении был null.");
        }
        OpenedRooms = new List<string>(data.OpenedRooms);
        ShowOpenedRooms();
    }

    public void ShowOpenedRooms()
    {
        if (_mapCanvasInstance == null)
        {
            Debug.LogError("MapManager: _mapCanvasInstance == null!");
            return;
        }

        var roomsContainer = _mapCanvasInstance.transform.Find(roomsContainerName);
        foreach (var roomName in OpenedRooms)
        {
            var roomTransform = roomsContainer.Find(roomName);
            if (roomTransform != null)
                roomTransform.gameObject.SetActive(true);
            else
                Debug.LogWarning($"MapManager: Комната с именем '{roomName}' не найдена в {_mapCanvasInstance.name}");
        }
    }

    public void OpenRoom(string roomName)
    {
        if (!OpenedRooms.Contains(roomName))
            OpenedRooms.Add(roomName);
    }

    public void SetMapIcon(string roomName)
    {
        if (_mapCanvasInstance == null)
        {
            Debug.LogError("MapManager: _mapCanvasInstance == null!");
            return;
        }

        var roomsContainer = _mapCanvasInstance.transform.Find(roomsContainerName);
        var roomTransform = roomsContainer.Find(roomName);
        if (roomTransform != null)
            _mapIconTransform.transform.position = roomTransform.position;
    }

    public void Save(ref MapRoomsSaveData data)
    {
        data.OpenedRooms = new List<string>(OpenedRooms);
    }
}

[System.Serializable]
public struct MapRoomsSaveData
{
    public List<string> OpenedRooms;
}
