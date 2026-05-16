using UnityEngine;

public class MnemirInteract : MonoBehaviour
{
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
            interact.Mnemir = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.Mnemir = false;
        }
    }

    #endregion
}
