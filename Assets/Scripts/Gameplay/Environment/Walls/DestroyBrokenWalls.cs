using UnityEngine;

public class DestroyBrokenWalls : MonoBehaviour
{
    [SerializeField] private string _wallID;

    private WallsManager wallsManager;

    public string WallID { get { return _wallID; } }

    private void Awake()
    {
        wallsManager = GameObject.FindGameObjectWithTag("WallsManager").GetComponent<WallsManager>();
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
        if (wallsManager == null) Debug.Log("wallsManager == null");
        if (wallsManager.WallsExistenceInstance == null) Debug.Log("WallsExistenceInstance == null");

        wallsManager.WallsExistenceInstance.BreakWall(_wallID);
    }
}
