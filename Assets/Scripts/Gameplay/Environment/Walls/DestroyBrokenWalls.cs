using UnityEngine;

public class DestroyBrokenWalls : MonoBehaviour
{
    [SerializeField] private string _wallID;

    private void Awake()
    {
        DestroyWall();
    }

    private void DestroyWall()
    {
        if (WallsExistence.IsWallBroken(_wallID))
        {
            Destroy(gameObject);
        }
    }

    public void AddInDestroyWallList()
    {
        WallsExistence.BreakWall(_wallID);
        Debug.Log(_wallID);
    }
}
