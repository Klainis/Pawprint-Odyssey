using UnityEngine;

public class WallsManager : MonoBehaviour
{
    private static WallsManager instance;
    public static WallsManager Instance { get { return instance; } }

    public WallsExistence WallsExistenceInstance { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public static void DestroyBrokenWalls()
    {
        var walls = GameObject.FindGameObjectsWithTag("Object");
        foreach (var wall in walls)
        {
            if (wall.name == "Wall" ||  wall.name == "ClawWall")
            {
                var destroyWalls = wall.GetComponent<DestroyBrokenWalls>();
                destroyWalls.DestroyWall();
            }
        }
    }
}
