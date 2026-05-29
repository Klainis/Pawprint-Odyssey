using UnityEngine;

public class DestroyMnemirLockWalls : MonoBehaviour
{
    private DestroyBrokenWalls _destroyBrokenWalls;

    private void Awake()
    {
        _destroyBrokenWalls = GetComponent<DestroyBrokenWalls>();
    }

    private void Start()
    {
        if (PlayerView.Instance.PlayerModel.HasQuestMnemir)
        {
            if (_destroyBrokenWalls != null)
            {
                _destroyBrokenWalls.AddInDestroyWallList();
                SaveSystem.AutoSaveSimple();

                _destroyBrokenWalls.InstantiateDoorLight();
                Destroy(gameObject);
            }
        }
    }
}
