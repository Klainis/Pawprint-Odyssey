using System;
using UnityEngine;

public class WSAttack : MonoBehaviour
{
    public event Action OnPlayerDetected;

    private WanderingSpiritManager manager;

    private void Awake()
    {
        manager = GetComponent<WanderingSpiritManager>();
    }

    private void FixedUpdate()
    {
        var playerHitDir = manager.FacingRight ? Vector2.left : Vector2.right;
        var playerHit = Physics2D.Raycast(transform.position, playerHitDir, manager.PlayerDetectDist, manager.PlayerLayer);
        if (playerHit.collider != null)
            OnPlayerDetected?.Invoke();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            manager.IsAccelerated = false;
            if (manager.CurrentLife > 0)
            {
                var controller = collision.gameObject.GetComponent<CharacterController2D>();
                controller.ApplyDamage(manager.Data.Damage, transform.position);
            }
        }
    }
}
