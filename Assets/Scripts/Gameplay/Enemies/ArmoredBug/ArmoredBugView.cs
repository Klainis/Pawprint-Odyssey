using System.Collections;
using UnityEngine;

public class ArmoredBugView : MonoBehaviour
{

    #region Variables

    public EnemyModel Model { get; private set; }

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private PlayerAttack _playerAttack;
    [SerializeField] private float _lastPlayerAttackForce = 12f;
    [SerializeField] private float _playerAttackForce = 7f;
    [SerializeField] private bool _isInvincible = false;

    [Header("Acceleration")]
    [SerializeField] private float _acceleratedSpeed = 5f;
    [SerializeField] private float _playerDetectDist = 5f;
    [SerializeField] private float _telegraphTime = 0.25f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponSliceParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;
    private ParticleSystem _playerWeaponSliceParticleInstance;

    private Rigidbody2D _rigidBody;
    private ArmoredBugAnimation _bugAnimation;
    private ArmoredBugAttack _bugAttack;
    private ArmoredBugMove _bugMove;
    private DamageFlash _damageFlash;
    private ScreenShaker _screenShaker;
    private InstantiateMoney _money;

    private Coroutine _telegraphCoroutine;
    private Color _defaultColor;
    private RigidbodyConstraints2D _defaultConstraints;

    private bool _isHitted = false;
    private bool _isAccelerated = false;
    private bool _facingRight = true;
    private bool damageApplied = false;
    
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

        //_playerAttack = GameObject.Find("Player").GetComponent<Attack>();
        _playerAttack = InitializeManager.Instance.player?.GetComponent<PlayerAttack>();

        _rigidBody = GetComponent<Rigidbody2D>();
        _bugAnimation = GetComponent<ArmoredBugAnimation>();
        _bugAttack = GetComponent<ArmoredBugAttack>();
        _bugMove = GetComponent<ArmoredBugMove>();
        _damageFlash = GetComponent<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();
        _money = FindAnyObjectByType<InstantiateMoney>();

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
        else
            _bugMove.Move(_isAccelerated, _acceleratedSpeed);

        //Debug.Log(_facingRight);
        //Debug.Log(damageApplied);
    }

    #endregion

    public void ApplyDamage(int damage)
    {
        if (_isInvincible) return;

        var direction = damage / Mathf.Abs(damage);

        if ((direction > 0 && !_facingRight) ||
            (direction < 0 && _facingRight))
        {
            damageApplied = Model.TakeDamage(Mathf.Abs(damage));
        }
        else
        {
            damageApplied = false;
            _screenShaker.Shake();
            SpawnBlockedAttackParticles(direction);
            //«вук удара по броне
        }

        if (Model.IsDead)
        {
            _money.SetReward(Model.Reward);
            _money.InstantiateMon(transform.position);
        }

        if (damageApplied)
        {
            _damageFlash.CallDamageFlash();

            _bugAnimation.SetBoolHit(true);
            StartCoroutine(HitTime(1f));
            _rigidBody.linearVelocity = Vector2.zero;

            _screenShaker.Shake();
            SpawnDamageParticles(direction);

            if (_playerAttack.AttackSeriesCount == 3)
            {
                //_rigidBody.AddForce(new Vector2(direction * _lastPlayerAttackForce, _rigidBody.linearVelocity.y), ForceMode2D.Impulse);
                KnockBack(direction, _lastPlayerAttackForce);
                SpawnPlayerLastAttackParticles();
            }
            else if (_playerAttack.AttackSeriesCount < 3)
            {
                //_rigidBody.AddForce(new Vector2(direction * _playerAttackForce, _rigidBody.linearVelocity.y), ForceMode2D.Impulse);
                KnockBack(direction, _playerAttackForce);
                SpawnPlayerAttakParticles(direction);
            }
        }
    }

    private void KnockBack(int direction, float forceAttack)
    {
        _rigidBody.linearVelocity = new Vector2(0, _rigidBody.linearVelocity.y);
        var directionVector = new Vector2(direction, _rigidBody.linearVelocity.y);

        _rigidBody.AddForce(directionVector * forceAttack, ForceMode2D.Impulse);
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
        _bugMove.OnWallHit += HandleWallHit;

        _bugAttack.OnPlayerLeftDetected += HandlePlayerLeftDetected;
        _bugAttack.OnPlayerRightDetected += HandlePlayerRightDetected;
    }

    private void OnDisable()
    {
        _bugMove.OnWallHit -= HandleWallHit;

        _bugAttack.OnPlayerLeftDetected -= HandlePlayerLeftDetected;
        _bugAttack.OnPlayerRightDetected -= HandlePlayerRightDetected;
    }

    private void HandleWallHit()
    {
        _isAccelerated = false;
        _facingRight = _bugMove.Turn(_facingRight);
    }

    private void HandlePlayerLeftDetected() => StartTelegraph(false);
    private void HandlePlayerRightDetected() => StartTelegraph(true);

    private void StartTelegraph(bool faceRight)
    {
        if (_isAccelerated) return;

        if (_telegraphCoroutine != null)
            StopCoroutine(_telegraphCoroutine);

        if (faceRight != _facingRight)
            _facingRight = _bugMove.Turn(_facingRight);

        _telegraphCoroutine = StartCoroutine(AttackTelegraphRoutine());
        _isAccelerated = true;
    }

    #endregion

    #region IEnumerators

    private IEnumerator AttackTelegraphRoutine()
    {
        var renderer = GetComponent<SpriteRenderer>();

        renderer.color = Color.red;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(_telegraphTime);

        renderer.color = _defaultColor;
        _rigidBody.constraints = _defaultConstraints;

        _telegraphCoroutine = null;
    }

    private IEnumerator HitTime(float time)
    {
        _isHitted = true;
        yield return new WaitForSeconds(time);
        _isHitted = false;
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
        _rigidBody.linearVelocity = new Vector2(0, _rigidBody.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }

    #endregion
}
