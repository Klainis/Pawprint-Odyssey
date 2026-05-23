using UnityEngine;

public class InteractWithClawItem : MonoBehaviour
{
    private Interact interact;

    #region Common Methods

    private void Awake()
    {
        if (PlayerView.Instance.PlayerModel.HasClaw)
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
            interact.ClawItem = true;
            interact.clawItemObject = gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.ClawItem = false;
        }
    }

    #endregion
}
