using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class DestructibleWall : MonoBehaviour
{
    //public WallsExistence wallsExistence { get; private set; }

    [Header("Data")]
    [SerializeField] private EnvironmentData environmentData;

    [Space(5)]
    private ShakeObjectAfterDamage shakeObjectAfterDamage;

    [Space(5)]
    private DestroyBrokenWalls _destroyBrokenWalls;
    //private string _currentScene;
    //[SerializeField] private string _currentWall;

    private int life;

    private void Awake()
    {
        shakeObjectAfterDamage = GetComponent<ShakeObjectAfterDamage>();
        _destroyBrokenWalls = GetComponent<DestroyBrokenWalls>();
        //_currentScene = SceneManager.GetActiveScene().name;

        //foreach (WallsExistence.TransitionRoomWall room in WallsExistence.TransitionWalls)
        //{
        //    if (room.SceneName == _currentScene)
        //    {
        //        WallsExistence.WallItem[] wall = room.WallItems;
        //        foreach (WallsExistence.WallItem wallItem in wall)
        //        {
        //            if (wallItem.WallName == _currentWall)
        //            {
        //                if (wallItem.isOpen)
        //                {
        //                    Debug.Log("Стена уничтожена");
        //                    Destroy(gameObject);
        //                }
        //            }
        //        }
        //    }
        //}
    }

    private void Start()
    {
        life = environmentData.wallLife;
    }

    private void Update()
    {
        if (life <= 0)
        {
            shakeObjectAfterDamage.Shake();
            Destroy(gameObject);
            _destroyBrokenWalls.AddInDestroyWallList();
        }
        else if (shakeObjectAfterDamage.shakeDuration > 0)
        {
            shakeObjectAfterDamage.Shake();
        }
    }

    public void ApplyDamage(bool isClaw)
    {
        if (isClaw)
        {
            life -= 9999;
            shakeObjectAfterDamage.shakeDuration = environmentData.shakeDuration;
        }
        else
        {
            life -= 1;
            shakeObjectAfterDamage.shakeDuration = environmentData.shakeDuration;
        };
    }
}
