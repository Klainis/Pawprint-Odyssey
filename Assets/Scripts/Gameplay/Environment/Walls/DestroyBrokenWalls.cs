using UnityEngine;

public class DestroyBrokenWalls : MonoBehaviour
{
    [SerializeField] private string _wallID;

    private WallsManager wallsManager;

    public string WallID { get { return _wallID; } }

    private void Awake()
    {
        wallsManager = GameObject.FindGameObjectWithTag("WallsManager").GetComponent<WallsManager>();
        DestroyWall();
    }

    public void DestroyWall()
    {
        if (wallsManager.WallsExistenceInstance.IsWallBroken(_wallID))
        {
            Destroy(gameObject);
        }
    }

    public void AddInDestroyWallList()
    {
        wallsManager.WallsExistenceInstance.BreakWall(_wallID);
        Debug.Log(_wallID);
    }
}
