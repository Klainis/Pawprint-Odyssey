using UnityEngine;

public class EnterBossFight : MonoBehaviour
{
    [SerializeField] private BossHealth health;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            health.InstantiateBossHealth();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            health.DestroyBossHealthSlider();
        }
    }
}
