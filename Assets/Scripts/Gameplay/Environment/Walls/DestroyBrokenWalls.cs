using UnityEngine;

public class DestroyBrokenWalls : MonoBehaviour
{
    [SerializeField] private string _wallID;

    [Space(5)]
    [SerializeField] private GameObject _lightDoorGameObject;
    [SerializeField] private string entryGate;

    private WallsManager wallsManager;
    private GameObject _lightDoorGameObjectInstance;

    [SerializeField] private ShowScretRoomAfterBrakeWall _showSecretRoomAfterBrakeWall;

    public string WallID { get { return _wallID; } }

    private void Awake()
    {
        wallsManager = WallsManager.Instance;

        if (wallsManager.WallsExistenceInstance != null)
            DestroyWall();

        //_showSecretRoomAfterBrakeWall = FindAnyObjectByType<ShowScretRoomAfterBrakeWall>();
    }

    public void DestroyWall()
    {
        Debug.Log(wallsManager);
        Debug.Log(wallsManager.WallsExistenceInstance);
        if (wallsManager.WallsExistenceInstance.IsWallBroken(_wallID))
        {
            if (gameObject.CompareTag("SecretWall"))
            {
                _showSecretRoomAfterBrakeWall.ShowOpenedSecretRoom();
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
                InstantiateDoorLight();
            }
        }
    }
    private void InstantiateDoorLight()
    {
        var rotaion = Quaternion.identity;
        var positionOffset = new Vector3(0.4f, 0, 0);
        var position = Vector3.zero;

        if (entryGate.Contains("right"))
        {
            position = transform.position + positionOffset;
            rotaion = Quaternion.Euler(0, 0, 90);
        }
        else if (entryGate.Contains("left"))
        {
            position = transform.position - positionOffset;
            rotaion = Quaternion.Euler(0, 0, -90);
        }

        _lightDoorGameObjectInstance = Instantiate(_lightDoorGameObject, position, rotaion);
    }

    public void AddInDestroyWallList()
    {
        wallsManager.WallsExistenceInstance.BreakWall(_wallID);
        if (!gameObject.CompareTag("SecretWall"))
            MapManager.Instance.OpenDestructibleWall(_wallID);
    }
}
