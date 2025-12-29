using UnityEngine;

public class InteractArea : MonoBehaviour
{
    [SerializeField] private Interact interact;
    [SerializeField] private AbilitiesTreeUIManager _abilitiesTreeManager;

    private void Awake()
    {
        interact = GameObject.FindAnyObjectByType<Interact>();
        _abilitiesTreeManager = GameObject.FindAnyObjectByType<AbilitiesTreeUIManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = true;

            if (gameObject.CompareTag("Heal"))
            {
                interact.FullHeal = true;
                interact.AbilitiesTree = true;
                //_abilitiesTreeManager.EnableAreaErrorText(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interact.enabled = false;
            interact.FullHeal = false;
            interact.AbilitiesTree = false;
            //_abilitiesTreeManager.EnableAreaErrorText(true);
        }
    }
}
