using UnityEngine;

public class KeyLockInteract : MonoBehaviour
{
    public GameObject wall;

    private Interact interact;

    #region Common Methods

    private void Awake()
    {
        interact = FindAnyObjectByType<Interact>();
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
        if (closedGr != null)
        {
            closedGr.StartDestroyer();
        }
        else
        {
            Debug.LogError($"closedGr is NUll");
        }
    }

    #endregion
}
