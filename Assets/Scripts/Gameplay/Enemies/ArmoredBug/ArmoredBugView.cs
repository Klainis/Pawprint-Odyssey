using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class ArmoredBugView : MonoBehaviour
{
    #region SerializeFields

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private PlayerAttack _playerAttack;
    [SerializeField] private float _lastPlayerAttackForce = 20f;
    [SerializeField] private float _playerAttackForce = 7f;
    [SerializeField] private bool _isInvincible = false;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private AudioClip _shieldHitClip;

    //[Header("Attack")]
    //[SerializeField] private float _playerDetectDist = 5f;
    //[SerializeField] private float _attackDist = 1f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _telegraphTime = 0.25f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponSliceParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;
    
    #endregion

    #region Variables

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;
    private ParticleSystem _playerWeaponSliceParticleInstance;

    private AudioSource _audioSource;
    private Rigidbody2D _rb;
    private ArmoredBugAnimation _animation;
    private ArmoredBugAttack _attack;
    private ArmoredBugMove _move;
    private DamageFlash[] _damageFlash;
    private ScreenShaker _screenShaker;
    private InstantiateMoney _money;

    private Coroutine _telegraphCoroutine;
    private RigidbodyConstraints2D _defaultConstraints;

    private bool _isHitted = false;
    private bool _isAccelerated = false;
    private bool _damageApplied = false;
    private bool _isKnockback = false;

    #endregion

    #region Properties

    public EnemyModel Model { get; private set; }
    //public float PlayerDetectDist { get { return _playerDetectDist; } }
    //public float AttackDist { get { return _attackDist; } }
    public Rigidbody2D RigidBody { get { return _rb; } }
    public bool IsHitted { get { return _isHitted; } }
    public bool IsAccelerated { get { return _isAccelerated; } set { _isAccelerated = value; } }
    public bool FacingRight { get; private set; } = false;
    public bool IsTargeting { get; set; } = false;

    #endregion

    #region Common Methods

    private void Awake()
    {
        Model = new EnemyModel(_data.Life, _data.Speed, _data.Damage, _data.Reward);

        _playerAttack = InitializeManager.Instance.player?.GetComponent<PlayerAttack>();

        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _animation = GetComponent<ArmoredBugAnimation>();
        _attack = GetComponent<ArmoredBugAttack>();
        _move = GetComponent<ArmoredBugMove>();
        _damageFlash = GetComponentsInChildren<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();
        _money = FindAnyObjectByType<InstantiateMoney>();

        _defaultConstraints = _rb.constraints;
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
            if (IsTargeting)
            {
                //Debug.Log("Есть игрок");
                _animation.SetBoolMove(true);
                _move.Move();
            }
            else
            {
                //Debug.Log("Потеряли игрока");
                _animation.SetBoolMove(false);
            }
        }
    }

    #endregion

    public void ApplyDamage(int damage)
    {
        if (_isInvincible) return;

        var direction = damage / Mathf.Abs(damage);

        Debug.Log($"Damage with direction: {damage}");
        Debug.Log($"Direction: {direction}");
        Debug.Log($"FacingRight: {FacingRight}");

        if ((direction > 0 && FacingRight) ||
            (direction < 0 && !FacingRight))
        {
            _damageApplied = Model.TakeDamage(Mathf.Abs(damage));
        }
        else
        {
            _damageApplied = false;
            //_animation.SetTriggerBlockHit();
            PlayHitSound(_shieldHitClip);
            _screenShaker.Shake();
            SpawnBlockedAttackParticles(direction);
        }

        if (Model.IsDead)
        {
            _money.SetReward(Model.Reward);
            _money.InstantiateMon(transform.position);
        }

        if (_damageApplied)
        {
            PlayHitSound(_hitClip);
            foreach (var damageFlash in _damageFlash)
            {
                damageFlash.CallDamageFlash();
            }

            _animation.SetBoolHit(true);
            StartCoroutine(HitTime(0.5f));
            _rb.linearVelocity = Vector2.zero;

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
        if (_isInvincible) return;

        _damageApplied = Model.TakeDamage(Mathf.Abs(damage));

        if (Model.IsDead)
        {
            _money.SetReward(Model.Reward);
            _money.InstantiateMon(transform.position);
        }

        if (_damageApplied)
        {
            var direction = damage / Mathf.Abs(damage);
            foreach (var damageFlash in _damageFlash)
            {
                damageFlash.CallDamageFlash();
            }
            _animation.SetBoolHit(true);
            StartCoroutine(HitTime(0.5f));
            _rb.linearVelocity = Vector2.zero;
            _screenShaker.Shake();
            SpawnDamageParticles(direction);
            KnockBack(direction, _lastPlayerAttackForce);
            SpawnPlayerLastAttackParticles();
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
        _playerWeaponSliceParticleInstance = Instantiate(_playerWeaponSliceParticle, transform.position, Quaternion.identity);
    }

    private void SpawnBlockedAttackParticles(int direction)
    {
        var vectorDirection = new Vector2(direction, 0);
        var spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);

        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation, transform);
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
        _move.OnWallHit += HandleWallHit;

        _attack.OnPlayerLeftHitDetected += HandlePlayerLeftHitDetected;
        _attack.OnPlayerRightHitDetected += HandlePlayerRightHitDetected;

        _attack.OnPlayerLeftDetected += HandlePlayerLeftDetected;
        _attack.OnPlayerRightDetected += HandlePlayerRightDetected;
    }

    private void OnDisable()
    {
        _move.OnWallHit -= HandleWallHit;

        _attack.OnPlayerLeftHitDetected -= HandlePlayerLeftHitDetected;
        _attack.OnPlayerRightHitDetected -= HandlePlayerRightHitDetected;

        _attack.OnPlayerLeftDetected -= HandlePlayerLeftDetected;
        _attack.OnPlayerRightDetected -= HandlePlayerRightDetected;
    }

    private void HandleWallHit()
    {
        _isAccelerated = false;
        FacingRight = _move.Turn(FacingRight);
    }

    private void HandlePlayerLeftHitDetected() => StartTelegraph(false);
    private void HandlePlayerRightHitDetected() => StartTelegraph(true);

    private void HandlePlayerLeftDetected() => FacingRight = _move.Turn(true);
    private void HandlePlayerRightDetected() => FacingRight = _move.Turn(false);

    private void StartTelegraph(bool faceRight)
    {
        if (!_attack.CanAttack(_attackCooldown))
            return;

        if (_telegraphCoroutine != null)
            StopCoroutine(_telegraphCoroutine);

        //if (faceRight != FacingRight)
        //    FacingRight = _move.Turn(FacingRight);

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
        _attack.IsAttacking = true;
        _animation.SetBoolMove(false);
        _animation.SetBoolAttack(true);

        _rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(_telegraphTime);

        _rb.constraints = _defaultConstraints;
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);

        _attack.UpdateLastAttackTime();
        _attack.IsAttacking = false;
        _animation.SetBoolAttack(false);
        _telegraphCoroutine = null;
    }

    private IEnumerator HitTime(float time)
    {
        _isHitted = true;
        yield return new WaitForSeconds(time);
        _isHitted = false;
        _animation.SetBoolHit(false);
    }

    private IEnumerator DestroySelf()
    {
        _isInvincible = true;
        ChangeLayer("DeadEnemy");
        ChangeTag("isDead");

        //_bugAnimation.SetTriggerDead();
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }

    #endregion
}
