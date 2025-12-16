using UnityEngine;

public class DestroyBrokenWalls : MonoBehaviour
{
    [SerializeField] private string _wallID;

    [Space(5)]
    [SerializeField] private GameObject _lightDoorGameObject;
    [SerializeField] private string entryGate;

    private WallsManager wallsManager;
    private GameObject _lightDoorGameObjectInstance;

    public string WallID { get { return _wallID; } }

    private void Awake()
    {
        wallsManager = WallsManager.Instance;

        if (wallsManager.WallsExistenceInstance != null)
            DestroyWall();

        //if (_lightDoorGameObjectInstance != null)
        //    Destroy(_lightDoorGameObjectInstance);
    }

    public void DestroyWall()
    {
        if (wallsManager.WallsExistenceInstance.IsWallBroken(_wallID))
        {
            Destroy(gameObject);
            InstantiateDoorLight();
        }
    }
    private void InstantiateDoorLight()
    {
        Quaternion rotaion = Quaternion.identity;
        Vector3 positionOffset = new Vector3(0.4f, 0, 0);
        Vector3 position = Vector3.zero;

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
    }
}
