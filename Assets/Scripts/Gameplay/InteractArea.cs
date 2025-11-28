using UnityEngine;

public class InteractArea : MonoBehaviour
{
    [SerializeField] private Interact interact;

    private void Awake()
    {
        interact = GameObject.FindAnyObjectByType<Interact>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = true;

            if (gameObject.CompareTag("Heal"))
                interact.Heal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.Heal = false;
        }
    }
}
