using UnityEngine;

public class EyeOwlAttack : MonoBehaviour
{
    private GuardianOwlView _guardianOwlView;

    private void Awake()
    {
        _guardianOwlView = GameManager.FindAnyObjectByType<GuardianOwlView>();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_guardianOwlView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_guardianOwlView.Model.Damage, transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_guardianOwlView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_guardianOwlView.Model.Damage, transform.position);
            }
        }
    }
}
