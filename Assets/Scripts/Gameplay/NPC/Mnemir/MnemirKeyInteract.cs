using UnityEngine;

public class MnemirKeyInteract : MonoBehaviour
{
    private Interact interact;

    #region Common Methods

    private void Awake()
    {
        if (PlayerView.Instance.PlayerModel.HasMnemirKey)
        {
            Destroy(gameObject);
        }

        interact = FindAnyObjectByType<Interact>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = true;
            interact.MnemirKey = true;
            interact.mnemirKeyObject = gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.MnemirKey = false;
        }
    }

    #endregion
}
