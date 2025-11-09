using System;
using UnityEngine;

public class WSAttack : MonoBehaviour
{
    public event Action OnPlayerDetected;

    private WanderingSpiritView wsView;

    private void Awake()
    {
        wsView = GetComponent<WanderingSpiritView>();
    }

    private void FixedUpdate()
    {
        var playerHitDir = wsView.FacingRight ? Vector2.left : Vector2.right;
        var playerHit = Physics2D.Raycast(transform.position, playerHitDir, wsView.PlayerDetectDist, wsView.PlayerLayer);
        if (playerHit.collider != null)
            OnPlayerDetected?.Invoke();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            wsView.IsAccelerated = false;
            if (!wsView.Model.IsDead)
            {
                var controller = collision.gameObject.GetComponent<CharacterController2D>();
                controller.ApplyDamage(wsView.Model.Damage, transform.position);
            }
        }
    }
}
