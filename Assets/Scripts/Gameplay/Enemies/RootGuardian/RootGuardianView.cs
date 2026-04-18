using System.Collections;
using UnityEngine;

public class RootGuardianView : MonoBehaviour
{
    #region SerializeFields

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private PlayerAttack _playerAttack;
    [SerializeField] private float _patrolTimeWithoutPlayer = 2.5f;

    [Header("Attack")]
    [SerializeField] private float _playerDetectDist;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _telegraphTime;

    #endregion

    #region Variables

    private RootGuardianAnimation _animation;
    private RootGuardianAttack _attack;
    private RootGuardianMove _move;
    private RootGuardianTargetZoneHandler _targetZoneHandler;
    private Rigidbody2D _rb;

    private Coroutine _telegraphCoroutine;

    private Coroutine _retreatCoroutine;
    private Vector3 _centerPosition;

    private bool _isKnockback = false;
    private bool _isInvincible = false;
    private bool _isRetreating = false;

    #endregion

    #region Properties

    public EnemyModel Model { get; private set; }
    public bool FacingRight { get; private set; } = true;

    #endregion

    #region Common Methods

    private void Awake()
    {
        Model = new EnemyModel(_data.Life, _data.Speed, _data.Damage, _data.Reward);

        _playerAttack = InitializeManager.Instance.player?.GetComponent<PlayerAttack>();
        _animation = GetComponent<RootGuardianAnimation>();
        _attack = GetComponent<RootGuardianAttack>();
        _move = GetComponent<RootGuardianMove>();
        _targetZoneHandler = transform.parent.Find("TargetZone").GetComponent<RootGuardianTargetZoneHandler>();
        _rb = GetComponent<Rigidbody2D>();

        _attack.PlayerDetectDist = _playerDetectDist;
    }

    private void FixedUpdate()
    {
        if (Model.IsDead)
        {
            StartCoroutine(DestroySelf());
            return;
        }

        if (_isKnockback) return;

        if (_isRetreating || !_attack.IsAttacking)
            _move.Move(Model.Speed, FacingRight);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(Model.Damage, transform.position, gameObject);
            }
        }
    }

    #endregion

    private void KnockBack(int direction, float forceAttack)
    {
        if (_isKnockback)
            StopCoroutine(WaitForKnockBack());

        _rb.linearVelocity = new Vector2(direction * forceAttack, _rb.linearVelocity.y);
        StartCoroutine(WaitForKnockBack());
    }

    public void StartRetreatSequence(Vector3 center)
    {
        _centerPosition = center;
        if (_retreatCoroutine != null)
            StopCoroutine(_retreatCoroutine);
        _retreatCoroutine = StartCoroutine(RetreatRoutine());
    }

    public void StopRetreatSequence()
    {
        if (_retreatCoroutine != null)
        {
            StopCoroutine(_retreatCoroutine);
            _retreatCoroutine = null;
        }
        _isRetreating = false;
    }

    #region Events

    private void OnEnable()
    {
        _move.OnWallHit += HandleWallHit;

        _attack.OnPlayerLeftDetected += HandlePlayerLeftDetected;
        _attack.OnPlayerRightDetected += HandlePlayerRightDetected;
    }

    private void OnDisable()
    {
        _move.OnWallHit -= HandleWallHit;

        _attack.OnPlayerLeftDetected -= HandlePlayerLeftDetected;
        _attack.OnPlayerRightDetected -= HandlePlayerRightDetected;
    }

    private void HandleWallHit()
    {
        FacingRight = _move.Turn(FacingRight);
    }

    private void HandlePlayerLeftDetected() => StartTelegraph(true);
    private void HandlePlayerRightDetected() => StartTelegraph(false);

    private void StartTelegraph(bool faceRight)
    {
        if (!_attack.CanAttack(_attackCooldown) || _attack.IsAttacking || _telegraphCoroutine != null)
            return;

        if (faceRight != FacingRight)
            FacingRight = _move.Turn(FacingRight);

        _telegraphCoroutine = StartCoroutine(AttackWrapperRoutine());
    }

    #endregion

    #region Change Tag & Layer

    private void ChangeTag(string tag)
    {
        gameObject.tag = tag;
    }

    private void ChangeLayer(string layer)
    {
        gameObject.layer = LayerMask.NameToLayer(layer);
    }

    #endregion

    #region IEnumerators

    private IEnumerator DestroySelf()
    {
        _isInvincible = true;
        ChangeLayer("DeadEnemy");
        ChangeTag("isDead");

        _animation.SetTriggerDead();
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }

    private IEnumerator WaitForKnockBack()
    {
        _isKnockback = true;
        yield return new WaitForSeconds(0.2f);
        _isKnockback = false;
    }

    private IEnumerator RetreatRoutine()
    {
        yield return new WaitForSeconds(_patrolTimeWithoutPlayer);

        _isRetreating = true;

        var distanceToCenter = Mathf.Abs(transform.position.x - _centerPosition.x);
        while (distanceToCenter > 0.5f)
        {
            var xDiff = _centerPosition.x - transform.position.x;
            var shouldFaceRight = xDiff > 0;

            if (Mathf.Abs(xDiff) > 0.6f && shouldFaceRight != FacingRight)
                FacingRight = _move.Turn(FacingRight);

            distanceToCenter = Mathf.Abs(transform.position.x - _centerPosition.x);
            yield return null;
        }

        _isRetreating = false;
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);

        _animation.SetBoolHiding(true);
        yield return new WaitForSeconds(0.2f);
        _animation.SetBoolHiding(false);

        _retreatCoroutine = null;
        _targetZoneHandler.HideEnemy();
    }

    private IEnumerator AttackWrapperRoutine()
    {
        yield return StartCoroutine(_attack.AttackTelegraphRoutine(_telegraphTime));

        _telegraphCoroutine = null;
    }

    #endregion
}
