using UnityEngine;

public class FogShadowAttack : MonoBehaviour
{
    #region Variables

    private FogShadowView _view;

    private float _lastAttackTime;

    #endregion

    #region Properties

    public bool IsAttacking { get; set; } = false;

    #endregion

    #region Common Methods

    private void Start()
    {
        _view = GetComponent<FogShadowView>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_view.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_view.Model.Damage, transform.position, gameObject);
            }
        }
    }

    #endregion

    public bool CanAttack(float attackCooldown)
    {
        return !((Time.time < _lastAttackTime + attackCooldown) || IsAttacking);
    }

    public void UpdateLastAttackTime()
    {
        _lastAttackTime = Time.time;
    }

    public void Attack()
    {
        
    }
}
