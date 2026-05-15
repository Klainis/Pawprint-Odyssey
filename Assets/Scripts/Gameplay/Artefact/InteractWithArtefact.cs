using UnityEngine;

public class InteractWithArtefact : MonoBehaviour
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
            interact.Artefact = true;
            interact.artefactObject = gameObject;
            //Debug.Log("Передали все в Interact");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.Artefact = false;
        }
    }

    #endregion
}
