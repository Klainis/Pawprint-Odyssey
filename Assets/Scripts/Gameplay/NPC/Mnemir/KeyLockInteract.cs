using UnityEngine;

public class KeyLockInteract : MonoBehaviour
{
    public HelpUI helpUI;
    public ShowEducation showEducation;
    public GameObject wall;

    private Interact interact;
    private DestroyBrokenWalls _destroyBrokenWalls;

    #region Common Methods

    private void Awake()
    {
        interact = FindAnyObjectByType<Interact>();

        if (wall != null) _destroyBrokenWalls = wall.GetComponent<DestroyBrokenWalls>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = true;
            interact.MnemirKeyLock = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.MnemirKeyLock = false;
        }
    }

    public void DestroyKeyDoor()
    {
        if (wall == null)
        {
            Debug.LogError($"Отсутствует ссылка на KeyDoor: {wall}");
            return;
        }

        var closedGr = wall.GetComponent<ClosedGround>();
        if (closedGr != null && _destroyBrokenWalls != null)
        {
            closedGr.StartDestroyer();

            _destroyBrokenWalls.AddInDestroyWallList();
            SaveSystem.AutoSaveSimple();

            if (showEducation.gameObject.activeSelf) 
                showEducation.FadeOut();

            helpUI.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"closedGr is NUll");
        }
    }

    #endregion
}
