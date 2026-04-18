using System.Collections;
using UnityEngine;

public class RootGuardianView : MonoBehaviour
{
    #region SerializeFields

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private PlayerAttack _playerAttack;
    [SerializeField] private float _patrolTimeWithoutPlayer = 2.5f;
    [SerializeField] private float _lastPlayerAttackForce = 20f;
    [SerializeField] private AudioClip _hitClip;

    [Header("Attack")]
    [SerializeField] private float _playerDetectDist;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _telegraphTime;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponLastSliceParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;

    #endregion

    #region Variables

    private RootGuardianAnimation _animation;
    private RootGuardianAttack _attack;
    private RootGuardianMove _move;
    private RootGuardianTargetZoneHandler _targetZoneHandler;
    private Rigidbody2D _rb;
    private AudioSource _audioSource;
    private InstantiateMoney _money;
    private DamageFlash _damageFlash;
    private ScreenShaker _screenShaker;

    private Coroutine _telegraphCoroutine;

    private Coroutine _retreatCoroutine;
    private Vector3 _centerPosition;

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;
    private ParticleSystem _playerWeaponLastSliceAttackParticleInstance;

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
        _audioSource = GetComponent<AudioSource>();
        _money = FindAnyObjectByType<InstantiateMoney>();
        _damageFlash = GetComponent<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();

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

    public void ApplyDamage(int damage)
    {
        if (_isInvincible) return;

        var damageApplied = Model.TakeDamage(Mathf.Abs(damage));

        if (Model.IsDead)
        {
            _money.SetReward(Model.Reward);
            _money.InstantiateMon(transform.position);
        }

        if (damageApplied)
        {
            PlayHitSound(_hitClip);
            _damageFlash.CallDamageFlash();

            _animation.SetTriggerHit();
            _rb.linearVelocity = Vector2.zero;

            var direction = damage / Mathf.Abs(damage);
            _screenShaker.Shake();
            SpawnDamageParticles(direction);

            if (_playerAttack.AttackSeriesCount == 4)
            {
                KnockBack(direction, _lastPlayerAttackForce);
                SpawnPlayerLastAttackParticles();
            }
            else if (_playerAttack.AttackSeriesCount < 4)
            {
                SpawnPlayerAttakParticles(direction);
            }
        }
    }

    public void ApplyChargeDamage(int damage)
    {
        ApplyDamage(damage);
        var direction = damage / Mathf.Abs(damage);
        KnockBack(direction, _lastPlayerAttackForce);
    }

    private void PlayHitSound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void KnockBack(int direction, float forceAttack)
    {
        if (_isKnockback)
            StopCoroutine(WaitForKnockBack());

        _rb.linearVelocity = new Vector2(direction * forceAttack, _rb.linearVelocity.y);
        StartCoroutine(WaitForKnockBack());
    }

    public void FacePlayerOnSpawn(bool playerIsRight)
    {
        if (playerIsRight != FacingRight)
            FacingRight = _move.Turn(FacingRight);
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

    #region Particles

    private void SpawnDamageParticles(int direction)
    {
        var vectorDirection = new Vector2(direction, 0);
        var spawnRotation = Quaternion.FromToRotation(Vector2.right, vectorDirection);
        var spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);

        _damageParticleInstance = Instantiate(_damageParticle, transform.position, spawnRotation);
    }

    private void SpawnPlayerAttakParticles(int direction)
    {
        var vectorDirection = new Vector2(direction, 0);
        var spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);

        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation, transform);
        _playerWeaponSimpleSliceAttackParticleInstance = Instantiate(_playerWeapomSimpleSliceParticle, transform.position, Quaternion.identity);
    }

    private void SpawnPlayerLastAttackParticles()
    {
        _playerWeaponLastSliceAttackParticleInstance = Instantiate(_playerWeaponLastSliceParticle, transform.position, Quaternion.identity);
    }

    #endregion

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
        if (_isRetreating || !_attack.CanAttack(_attackCooldown) || _telegraphCoroutine != null)
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

        Destroy(transform.parent.gameObject);
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

            if (shouldFaceRight != FacingRight)
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
