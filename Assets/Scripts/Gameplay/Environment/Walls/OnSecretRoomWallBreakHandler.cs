using UnityEngine;

public class OnSecretRoomWallBreakHandler : MonoBehaviour
{
    [SerializeField] private string _roomToOpenOnMap;

    private DestructibleWall _destructibleWall;

    private void Start()
    {
        _destructibleWall = GetComponent<DestructibleWall>();
        _destructibleWall.OnWallBreak += OnWallBreakHandler;
    }

    private void OnWallBreakHandler()
    {
        MapManager.Instance.OpenRoom(_roomToOpenOnMap);
    }
}
