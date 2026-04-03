using System.Collections;
using UnityEngine;

public class WanderingSpiritView : MonoBehaviour
{
    #region Variables

    public EnemyModel Model { get; private set; }

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private PlayerAttack _playerAttack;
    [SerializeField] private float _lastPlayerAttackForce = 20f;
    [SerializeField] private float _playerAttackForce = 7f;
    [SerializeField] private bool _isInvincible = false;
    [SerializeField] private AudioClip _hitClip;

    [Header("Attack")]
    [SerializeField] private float _jumpHeight = 1.75f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _stayAfterAttackDelay = 1.0f;
    [SerializeField] private float _playerDetectDist = 5f;
    [SerializeField] private float _telegraphTime = 0.25f;

    [Header("Not used")]
    [SerializeField] private float _acceleratedSpeed = 5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponLastSliceParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;
    private ParticleSystem _playerWeaponLastSliceAttackParticleInstance;

    private AudioSource _audioSource;
    private Rigidbody2D _rigidBody;
    private WSAnimation _wsAnimation;
    private WSAttack _wsAttack;
    private WSMove _wsMove;
    private DamageFlash _damageFlash;
    private ScreenShaker _screenShaker;
    private InstantiateMoney _money;

    private Coroutine _telegraphCoroutine;
    private Color _defaultColor;
    private RigidbodyConstraints2D _defaultConstraints;

    private bool _isHitted = false;
    private bool _isAccelerated = false;
    private bool _facingRight = true;
    private bool _isKnockback = false;

    #endregion

    #region Properties

    public float PlayerDetectDist { get { return _playerDetectDist; } }
    public Rigidbody2D RigidBody { get { return _rigidBody; } }
    public bool IsHitted { get { return _isHitted; } }
    public bool IsAccelerated { get { return _isAccelerated; } set { _isAccelerated = value; } }
    public bool FacingRight { get { return _facingRight; } set { _facingRight = value; } }

    #endregion

    #region Common Methods

    private void Awake()
    {
        Model = new EnemyModel(_data.Life, _data.Speed, _data.Damage, _data.Reward);

        _playerAttack = InitializeManager.Instance.player?.GetComponent<PlayerAttack>();
        _audioSource = GetComponent<AudioSource>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _wsAnimation = GetComponent<WSAnimation>();
        _wsAttack = GetComponent<WSAttack>();
        _wsMove = GetComponent<WSMove>();
        _money = FindAnyObjectByType<InstantiateMoney>();
        _damageFlash = GetComponent<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();

        var renderer = GetComponent<SpriteRenderer>();
        _defaultColor = renderer.color;
        _defaultConstraints = _rigidBody.constraints;
    }

    private void FixedUpdate()
    {
        if (Model.IsDead)
        {
            StartCoroutine(DestroySelf());
            return;
        }
        
        if (_isKnockback) return;

        if (!_wsAttack.IsAttacking)
        {
            _wsMove.Move(_isAccelerated, _acceleratedSpeed);
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

            _wsAnimation.SetTriggerHit();
            _rigidBody.linearVelocity = Vector2.zero;

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

        _rigidBody.linearVelocity = new Vector2(direction * forceAttack, _rigidBody.linearVelocity.y);
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

    #region Events

    private void OnEnable()
    {
        _wsMove.OnWallHit += HandleWallHit;
     
        _wsAttack.OnPlayerLeftDetected += HandlePlayerLeftDetected;
        _wsAttack.OnPlayerRightDetected += HandlePlayerRightDetected;
    }

    private void OnDisable()
    {
        _wsMove.OnWallHit -= HandleWallHit;

        _wsAttack.OnPlayerLeftDetected -= HandlePlayerLeftDetected;
        _wsAttack.OnPlayerRightDetected -= HandlePlayerRightDetected;
    }

    private void HandleWallHit()
    {
        _isAccelerated = false;
        _facingRight = _wsMove.Turn(_facingRight);
    }

    private void HandlePlayerLeftDetected() => StartTelegraph(false);
    private void HandlePlayerRightDetected() => StartTelegraph(true);

    private void StartTelegraph(bool faceRight)
    {
        if (!_wsAttack.CanAttack(_attackCooldown))
            return;

        if (!_wsAttack.CanJumpUp(_jumpHeight))
            return;

        if (_telegraphCoroutine != null)
            StopCoroutine(_telegraphCoroutine);

        if (faceRight != _facingRight)
            _facingRight = _wsMove.Turn(_facingRight);

        _telegraphCoroutine = StartCoroutine(AttackTelegraphRoutine());
    }

    #endregion

    #region IEnumerators

    private IEnumerator WaitForKnockBack()
    {
        _isKnockback = true;
        yield return new WaitForSeconds(0.2f);
        _isKnockback = false;
    }

    private IEnumerator AttackTelegraphRoutine()
    {
        var playerPos = _playerAttack.transform.position;
        _wsAttack.SetIsAttacking(true);
        _wsAnimation.SetBoolAttack(true);

        var renderer = GetComponent<SpriteRenderer>();
        renderer.color = Color.red;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(_telegraphTime);

        renderer.color = _defaultColor;
        _rigidBody.constraints = _defaultConstraints;

        if (_playerAttack != null)
            _wsAttack.JumpToPoint(playerPos, _jumpHeight);

        yield return new WaitUntil(() => _rigidBody.linearVelocity.y < 0);
        yield return new WaitUntil(() => Mathf.Abs(_rigidBody.linearVelocity.y) < 0.1f);
        _rigidBody.linearVelocity = new Vector2(0, _rigidBody.linearVelocity.y);

        _wsAttack.UpdateLastAttackTime();
        _wsAttack.SetIsAttacking(false);
        _wsAnimation.SetBoolAttack(false);
        _telegraphCoroutine = null;
    }

    private IEnumerator HitTime(float time)
    {
        var constraintsBefore = _rigidBody.constraints;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(time);
        _rigidBody.constraints = constraintsBefore;
    }

    private IEnumerator DestroySelf()
    {
        _isInvincible = true;
        ChangeLayer("DeadEnemy");
        ChangeTag("isDead");

        _wsAnimation.SetTriggerDead();
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
        _rigidBody.linearVelocity = new Vector2(0, _rigidBody.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }

    #endregion
}
