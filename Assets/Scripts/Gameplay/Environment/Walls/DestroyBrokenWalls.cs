using UnityEngine;

public class DestroyBrokenWalls : MonoBehaviour
{
    [SerializeField] private string _wallID;

    private WallsManager wallsManager;

    public string WallID { get { return _wallID; } }

    private void Awake()
    {
        wallsManager = WallsManager.Instance;

        if (wallsManager.WallsExistenceInstance != null)
            DestroyWall();
    }

    public void DestroyWall()
    {
        if (wallsManager.WallsExistenceInstance.IsWallBroken(_wallID))
            Destroy(gameObject);
    }

    public void AddInDestroyWallList()
    {
        wallsManager.WallsExistenceInstance.BreakWall(_wallID);
    }
}
