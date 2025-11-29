using GlobalEnums;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GuardianOwlView : MonoBehaviour
{
    public EnemyModel Model { get; private set; }

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float secondStageLifeCoef = 0.6f;
    [SerializeField] private bool _isInvincible = false;

    [Header("Events")]
    [SerializeField] private UnityEvent<bool, bool> _Hit;
    [SerializeField] private UnityEvent _Die;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;

    [Space(5)]
    [SerializeField] private FightDoor _fightDoor;

    //private const float groundedRadius = 0.2f;

    private Rigidbody2D _rigidBody;
    //private Transform groundCheck;

    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;

    //private SGAnimation sgAnimation;
    private GuardianOwlAttack _guraduianOwlAttack;
    //private SGMove sgMove;
    private DamageFlash _damageFlash;
    private ScreenShaker _screenShaker;

    private int _maxLifeForReading;
    private float _secondStageLifeAmount;
    private bool _isSecondStage = false;
    private bool _isHitted = false;
    //private bool isAccelerated = false;
    //private bool moveDisabled = false;
    private bool _facingRight = true;

    public Rigidbody2D RigidBody { get { return _rigidBody; } }
    public int MaxLifeForReading { get { return _maxLifeForReading; } }
    public bool IsHitted { get { return _isHitted; } }
    //public bool IsAccelerated { get { return isAccelerated; } set { isAccelerated = value; } }
    //public bool MoveDisabled { get { return moveDisabled; } set { moveDisabled = value; } }
    public bool FacingRight { get { return _facingRight; } }

    private void Awake()
    {
        Model = new EnemyModel(_data.Life, _data.Speed, _data.Damage);

        _maxLifeForReading = Model.Life;
        _secondStageLifeAmount = Model.Life * secondStageLifeCoef;

        _rigidBody = GetComponent<Rigidbody2D>();
        //groundCheck = transform.Find("GroundCheck");

        //sgAnimation = GetComponent<SGAnimation>();
        _guraduianOwlAttack = GetComponent<GuardianOwlAttack>();
        //sgMove = GetComponent<SGMove>();
        _damageFlash = GetComponent<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();
    }

    private void FixedUpdate()
    {
        if (Model.IsDead)
        {
            _Die.Invoke();
            StartCoroutine(DestroySelf());
            if (_fightDoor != null)
            {
                _fightDoor.CloseDoor(false);
            }
            GameManager._instance.SetGameState(GameState.PLAYING);
            return;
        }

        //var playerHits = _guraduianOwlAttack.GetPlayerHits(35, _facingRight);
        //var isGrounded = CheckIfGrounded();
        //if (!_isSecondStage && !isAccelerated && isGrounded)
        //    _guraduianOwlAttack.RamAttack(playerHits);
        //if (_isSecondStage && !isAccelerated)
        //    _guraduianOwlAttack.RandomAttack(_facingRight);

        //sgMove.Move(isAccelerated, _guraduianOwlAttack.AcceleratedSpeed);
    }

    //private bool CheckIfGrounded()
    //{
    //    var groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundedRadius, _groundLayer);
    //    return groundHit.collider != null;
    //}

    public void ApplyDamage(int damage)
    {
        if (_isInvincible) return;

        // sgAnimation.SetBoolHit(true);
        var damageApplied = Model.TakeDamage(Mathf.Abs(damage));
        if (damageApplied)
        {
            _damageFlash.CallDamageFlash();
            _screenShaker.Shake();
            var direction = damage / Mathf.Abs(damage);
            SpawnDamageParticles(direction);

            _Hit.Invoke(true, false);
            if (Model.Life <= _secondStageLifeAmount)
                _isSecondStage = true;
            StartCoroutine(HitTime());
        }
    }

    private void SpawnDamageParticles(int direction)
    {
        var vectorDirection = new Vector2(direction, 0);
        var spawnRotation = Quaternion.FromToRotation(Vector2.right, vectorDirection);
        _damageParticleInstance = Instantiate(_damageParticle, transform.position, spawnRotation);
        var spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);
        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation);
    }

    private void ChangeTag(string tag)
    {
        gameObject.tag = tag;
    }

    private void ChangeLayer(string layer)
    {
        gameObject.layer = LayerMask.NameToLayer(layer);
    }

    private void OnEnable()
    {
        _guraduianOwlAttack.EyeAttackCoroutine(5);
        //sgMove.OnWallHit += HandleWallHit;
        //_guraduianOwlAttack.OnPlayerDetected += HandlePlayerDetected;
    }

    //private void OnDisable()
    //{
    //    sgMove.OnWallHit -= HandleWallHit;
    //    _guraduianOwlAttack.OnPlayerDetected -= HandlePlayerDetected;
    //}

    //private void HandleWallHit()
    //{
    //    isAccelerated = false;
    //    _facingRight = sgMove.Turn(_facingRight);
    //    _guraduianOwlAttack.CheckRamSeriesCountAndPause();
    //}

    //private void HandlePlayerDetected()
    //{
    //    isAccelerated = true;
    //}

    private IEnumerator HitTime()
    {
        _isHitted = true;
        _isInvincible = true;
        yield return new WaitForSeconds(0.1f);
        _isHitted = false;
        _isInvincible = false;
    }

    private IEnumerator DestroySelf()
    {
        _isInvincible = true;
        ChangeTag("isDead");
        ChangeLayer("DeadEnemy");

        // _bugAnimation.SetTriggerDead();
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
        _rigidBody.linearVelocity = new Vector2(0, _rigidBody.linearVelocity.y);
        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}
