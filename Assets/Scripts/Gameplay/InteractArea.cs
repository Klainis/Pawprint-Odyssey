using UnityEngine;

public class InteractArea : MonoBehaviour
{
    [SerializeField] private Interact interact;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = true;

            if (gameObject.tag == "Heal")
            {
                interact.heal = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.heal = false;
        }
    }
}
