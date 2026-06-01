using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        StartCoroutine(Freeze());
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

    private IEnumerator Freeze()
    {
        var rb = GetComponent<Rigidbody2D>();
        yield return new WaitForSeconds(2f);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    #endregion
}
