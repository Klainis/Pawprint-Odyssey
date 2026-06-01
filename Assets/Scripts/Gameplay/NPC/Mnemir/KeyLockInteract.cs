using UnityEngine;

public class KeyLockInteract : MonoBehaviour
{
    public HelpUI helpUI;
    public ShowEducation showEducation;
    public GameObject wall;

    private Interact _interact;
    private DestroyBrokenWalls _destroyBrokenWalls;
    private InstantiateParticles _instantiateParticles;

    #region Common Methods

    private void Awake()
    {
        _interact = FindAnyObjectByType<Interact>();

        if (wall != null) _destroyBrokenWalls = wall.GetComponent<DestroyBrokenWalls>();

        _instantiateParticles = GetComponent<InstantiateParticles>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _interact.enabled = true;
            _interact.MnemirKeyLock = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _interact.enabled = false;
            _interact.MnemirKeyLock = false;
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

            showEducation.gameObject.SetActive(false);
            helpUI.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            Debug.LogError($"closedGr is NUll");
        }
    }

    public void InstantiateParticle()
    {
        if (_instantiateParticles == null)
        {
            Debug.LogError("KeyLockInteract : Отсутствует ссылка на InstantiateParticles");
            return;
        }

        _instantiateParticles.InstantiatePollen();
    }

    #endregion
}
