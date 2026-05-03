using System.Collections;
using UnityEngine;

public class FogShadowAttack : MonoBehaviour
{
    #region Variables

    private FogShadowAnimation _animation;
    private Rigidbody2D _rb;

    private Coroutine _telegraphCoroutine;
    private RigidbodyConstraints2D _defaultConstraints;

    private float _lastAttackTime;

    #endregion

    #region Properties

    public GameObject ProjectilePrefab { get; set; }
    public GameObject AttackPos { get; set; }
    public int Damage { get; set; }
    public float TelegraphTime { get; set; }
    public float AttackCooldown { get; set; }
    public float TimeToHit { get; set; }
    public bool IsAttacking { get; set; } = false;

    #endregion

    #region Common Methods

    private void Start()
    {
        _animation = GetComponent<FogShadowAnimation>();
        _rb = GetComponent<Rigidbody2D>();

        _defaultConstraints = _rb.constraints;
    }

    #endregion

    #region Attack

    public bool CanAttack(float attackCooldown)
    {
        return !((Time.time < _lastAttackTime + attackCooldown) || IsAttacking);
    }

    public void UpdateLastAttackTime()
    {
        _lastAttackTime = Time.time;
    }

    private void Attack()
    {
        var player = InitializeManager.Instance.player?.transform;
        if (player == null) return;

        var startPos = AttackPos.transform.position;
        var targetPos = new Vector2(player.position.x, player.position.y + 0.7f);

        var velocity = CalculateParabolicVelocity(startPos, targetPos, TimeToHit);

        var projObj = Instantiate(ProjectilePrefab, startPos, Quaternion.identity);
        var projectile = projObj.GetComponent<FogShadowProjectile>();

        projectile.Launch(velocity, Damage);
    }

    private Vector2 CalculateParabolicVelocity(Vector2 start, Vector2 target, float time)
    {
        var distX = target.x - start.x;
        var distY = target.y - start.y;

        var vx = distX / time;

        var gravity = Mathf.Abs(Physics2D.gravity.y);
        var vy = (distY + 0.5f * gravity * time * time) / time;

        return new Vector2(vx, vy);
    }

    public void StartAttackTelegraph(bool isDissipated)
    {
        if (isDissipated || !CanAttack(AttackCooldown))
            return;

        if (_telegraphCoroutine != null)
            StopCoroutine(_telegraphCoroutine);

        _telegraphCoroutine = StartCoroutine(AttackTelegraphRoutine());
    }

    #endregion

    #region IEnumerators

    private IEnumerator AttackTelegraphRoutine()
    {
        IsAttacking = true;
        _animation.SetBoolMove(false);
        _animation.SetBoolAttack(true);

        _rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(TelegraphTime);

        Attack();

        _rb.constraints = _defaultConstraints;
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);

        UpdateLastAttackTime();
        IsAttacking = false;
        _animation.SetBoolAttack(false);
        _telegraphCoroutine = null;
    }

    #endregion
}
