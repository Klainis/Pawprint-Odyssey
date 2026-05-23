using UnityEngine;

public class InteractWithDoubleJumpItem : MonoBehaviour
{
    private Interact interact;

    #region Common Methods

    private void Awake()
    {
        if (PlayerView.Instance.PlayerModel.HasDoubleJump)
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
            interact.DoubleJumpItem = true;
            interact.doubleJumpItemObject = gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.DoubleJumpItem = false;
        }
    }

    #endregion
}
