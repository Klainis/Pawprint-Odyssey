using UnityEngine;

public class WallsManager : MonoBehaviour
{
    private static WallsManager instance;
    public static WallsManager Instance { get { return instance; } }

    public WallsExistence WallsExistenceInstance { get; set; }

    private void Awake()
    {
        instance = this;

        if (WallsExistenceInstance == null)
            WallsExistenceInstance = WallsExistence.CreateEmpty();
    }

    public static void DestroyBrokenWalls()
    {
        var objects = GameObject.FindGameObjectsWithTag("Object");
        foreach (var obj in objects)
        {
            if (obj.name == "Wall" ||  obj.name == "ClawWall")
            {
                var destroyWalls = obj.GetComponent<DestroyBrokenWalls>();
                destroyWalls.DestroyWall();
            }
        }
    }
}
