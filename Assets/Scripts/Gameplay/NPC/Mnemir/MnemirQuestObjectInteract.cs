using UnityEngine;
using UnityEngine.SceneManagement;

public class MnemirQuestObjectInteract : MonoBehaviour
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
            interact.MnemirQuestObject = SceneManager.GetActiveScene().name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.MnemirQuestObject = "";
        }
    }

    #endregion
}
