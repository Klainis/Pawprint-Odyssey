using System.Collections;
using UnityEngine;

public class FogShadowView : MonoBehaviour
{
    #region SerializeFields

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private PlayerAttack _playerAttack;
    [SerializeField] private float _lastPlayerAttackForce = 20f;
    [SerializeField] private float _playerAttackForce = 7f;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private bool _isInvincible = false;

    [Header("Attack")]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _attackPos;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _telegraphTime = 0.25f;
    [SerializeField] private float _timeToHit = 1.0f;

    [Header("Chase")]
    [SerializeField] private float _followDistance = 3.0f;
    [SerializeField] private float _hoverHeight = 2.0f;
    [SerializeField] private LayerMask _obstacleLayer;

    [Header("Patrol")]
    [SerializeField] private Vector2 _patrolRange;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponLastSliceParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;

    #endregion

    #region Variables

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;
    private ParticleSystem _playerWeaponLastSliceAttackParticleInstance;

    private AudioSource _audioSource;
    private Rigidbody2D _rb;
    private FogShadowAttack _attack;
    private FogShadowMove _move;
    private FogShadowAnimation _animation;
    private FogShadowTargetZoneHandler _targetZoneHandler;
    private InstantiateMoney _money;
    private DamageFlash _damageFlash;
    private ScreenShaker _screenShaker;

    private bool _isKnockback = false;

    #endregion

    #region Properties

    public EnemyModel Model { get; private set; }
    public bool FacingRight { get; private set; } = false;
    public bool IsChasing { get; set; } = false;

    #endregion

    #region Common Methods

    private void Awake()
    {
        Model = new EnemyModel(_data.Life, _data.Speed, _data.Damage, _data.Reward);

        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _attack = GetComponent<FogShadowAttack>();
        _move = GetComponent<FogShadowMove>();
        _animation = GetComponent<FogShadowAnimation>();
        _targetZoneHandler = GetComponentInChildren<FogShadowTargetZoneHandler>();
        _money = FindAnyObjectByType<InstantiateMoney>();
        _damageFlash = GetComponent<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();

        _move.Speed = Model.Speed;
        _move.PatrolRange = _patrolRange;
        _move.FollowDistance = _followDistance;
        _move.HoverHeight = _hoverHeight;
        _move.ObstacleLayer = _obstacleLayer;

        _attack.ProjectilePrefab = _projectilePrefab;
        _attack.AttackPos = _attackPos;
        _attack.Damage = Model.Damage;
        _attack.TelegraphTime = _telegraphTime;
        _attack.AttackCooldown = _attackCooldown;
        _attack.TimeToHit = _timeToHit;
    }

    private void FixedUpdate()
    {
        if (Model.IsDead)
        {
            StartCoroutine(DestroySelf());
            return;
        }
        else if (!_attack.IsAttacking)
        {
            if (!IsChasing)
            {
                _move.Patrol();
                FacingRight = _move.UpdateFacingDirection(FacingRight);
            }
            else if (IsChasing)
            {
                _attack.StartAttackTelegraph();
                _move.Chase();
                FacingRight = _move.UpdateFacingDirection(FacingRight);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !Model.IsDead)
        {
            var playerView = collision.gameObject.GetComponent<PlayerView>();
            playerView.ApplyDamage(Model.Damage, transform.position, gameObject);
        }
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        _targetZoneHandler.TargetZoneEnter += HandleTargetZoneEnter;
    }

    private void OnDisable()
    {
        _targetZoneHandler.TargetZoneEnter -= HandleTargetZoneEnter;
    }

    private void HandleTargetZoneEnter()
    {
        _targetZoneHandler.enabled = false;
        _targetZone.SetActive(false);
        IsChasing = true;
        _move.IsChasing = IsChasing;
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

            _animation.SetBoolHit(true);
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

    private IEnumerator WaitForKnockBack()
    {
        _isKnockback = true;
        yield return new WaitForSeconds(0.2f);
        _isKnockback = false;
    }

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

    #endregion
}
