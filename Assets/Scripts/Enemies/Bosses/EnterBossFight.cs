using UnityEngine;

public class EnterBossFight : MonoBehaviour
{
    [SerializeField] private BossHealth health;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collison");
            health.InstantiateBossHealth();
        }

    }
}
