using GlobalEnums;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GuardianOwlView : MonoBehaviour
{
    public EnemyModel Model { get; private set; }

    [Header("Main params")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _secondStageLifeCoef = 0.6f;
    [SerializeField] private float _thirdStageLifeCoef = 0.2f;
    [SerializeField] private bool _isInvincible = false;
    [SerializeField] private GameObject _doubleJumpItem;

    [Header("Events")]
    [SerializeField] private UnityEvent<bool, bool> _Hit;
    [SerializeField] private UnityEvent _Die;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponSliceParticle;

    private ParticleSystem _playerWeaponSliceParticleInstance;

    [Space(5)]
    [SerializeField] private FightDoor _fightDoor;

    private Rigidbody2D _rigidBody;

    private BossStage BossStage;

    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;

    //private SGAnimation sgAnimation;
    private GuardianOwlAttack _guraduianOwlAttack;
    private GuardianOwlMove _guraduianOwlMove;
    private DamageFlash _damageFlash;
    private ScreenShaker _screenShaker;

    private int _maxLifeForReading;
    private int _secondStageLifeAmount;
    private int _thirdStageLifeAmount;

    private bool _isHitted = false;
    private bool _facingRight = true;
    private bool _deathDone = false;

    public Rigidbody2D RigidBody { get { return _rigidBody; } }
    public int MaxLifeForReading { get { return _maxLifeForReading; } }
    public bool IsHitted { get { return _isHitted; } }
    public bool FacingRight { get { return _facingRight; } }

    private void Awake()
    {
        Model = new EnemyModel(_data.Life, _data.Speed, _data.Damage);

        _maxLifeForReading = Model.Life;
        _secondStageLifeAmount = (int)(Model.Life * _secondStageLifeCoef);
        _thirdStageLifeAmount = (int)(Model.Life * _thirdStageLifeCoef);

        _rigidBody = GetComponent<Rigidbody2D>();

        //sgAnimation = GetComponent<SGAnimation>();
        _guraduianOwlAttack = GetComponent<GuardianOwlAttack>();
        _guraduianOwlMove = GetComponent<GuardianOwlMove>();
        _damageFlash = GetComponent<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();
    }

    private void Start()
    {
        BossStage = BossStage.STAGE_1;
    }

    private void FixedUpdate()
    {
        if (_deathDone)
            return;
        if (Model.IsDead)
        {
            _deathDone = true;
            PlayerView.Instance.PlayerModel.SetGuardianOwlKilled();
            SaveSystem.AutoSave();
            _Die.Invoke();
            StartCoroutine(DestroySelf());
            if (_fightDoor != null)
            {
                _fightDoor.CloseDoor(false);
            }
            GameManager.Instance.SetGameState(GameState.PLAYING);
            return;
        }
    }

    #region BossStage
    
    public void ChangeBossStage(BossStage newBossStage)
    {
        BossStage = newBossStage;
    }

    #endregion

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

            if (Model.Life <= _secondStageLifeAmount && Model.Life >= _thirdStageLifeAmount)
            {
                ChangeBossStage(BossStage.STAGE_2);
                Debug.Log(BossStage);
            }
            else if (Model.Life <= _thirdStageLifeAmount)
            {
                ChangeBossStage(BossStage.STAGE_3);
                Debug.Log(BossStage);
            }
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
        _playerWeaponSliceParticleInstance = Instantiate(_playerWeaponSliceParticle, transform.position, Quaternion.identity);
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
        StartCoroutine(OnEnteringBoss());
    }

    private void OnDestroy()
    {
        if (!PlayerView.Instance.PlayerModel.HasDoubleJump)
            _doubleJumpItem.SetActive(true);
    }

    private IEnumerator OnEnteringBoss()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(_guraduianOwlMove.MoveUp());
        yield return new WaitForSeconds(3);
        StartCoroutine(BehaviourLoop());
    }

    #region Patterns
    private IEnumerator BehaviourLoop()
    {
        while (!Model.IsDead)
        {
            switch (BossStage)
            {
                case BossStage.STAGE_1:
                    yield return FirstStagePattern();
                    break;
                case BossStage.STAGE_2:
                    yield return SecondStagePattern();
                    break;
                case BossStage.STAGE_3:
                    _guraduianOwlAttack.ApplyEyeAtackSpeedModifier();
                    _guraduianOwlMove.ApplySpeedModifier();
                    yield return ThirdStagePattern();
                    break;
            }
        }

    }

    private IEnumerator FirstStagePattern()
    {
        BossStage myStage = BossStage.STAGE_1;
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }


        Debug.Log("Первая стадия!!!!!!!!!!!");
        yield return _guraduianOwlMove.MoveToPlayer();
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(0.7f);


        //телеграфф
        yield return _guraduianOwlAttack.SpawnWaveAttack(3, myStage);
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(1);


        yield return _guraduianOwlMove.MoveUp();
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(0.5f);

    }

    private IEnumerator SecondStagePattern()
    {
        BossStage myStage = BossStage.STAGE_2;
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }

        Debug.Log("Вторая стадия!!!!!!!!!!!");
        yield return _guraduianOwlMove.MoveToPlayer();
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(0.7f);

        yield return _guraduianOwlAttack.SpawnWaveAttack(4, myStage);
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(1);

        yield return _guraduianOwlMove.MoveUp();
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(0.5f);

        if (Random.value > 0.5f)
        {
            //телеграфф
            yield return _guraduianOwlAttack.SpawnEyeAttack(5, myStage);
            if (ShouldInterrupt(myStage))
            {
                yield return _guraduianOwlMove.MoveUp();
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator ThirdStagePattern()
    {
        BossStage myStage = BossStage.STAGE_3;
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }

        Debug.Log("Вторая стадия!!!!!!!!!!!");
        yield return _guraduianOwlMove.MoveToPlayer();
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(0.7f);

        yield return _guraduianOwlAttack.SpawnWaveAttack(4, myStage);
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(1);

        yield return _guraduianOwlMove.MoveUp();
        if (ShouldInterrupt(myStage))
        {
            yield return _guraduianOwlMove.MoveUp();
            yield break;
        }
        yield return new WaitForSeconds(0.5f);

        if (Random.value > 0.4f)
        {
            //телеграфф
            yield return _guraduianOwlAttack.SpawnEyeAttack(5, myStage);
            if (ShouldInterrupt(myStage))
            {
                yield return _guraduianOwlMove.MoveUp();
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion

    public bool ShouldInterrupt(BossStage stage)
    {
        return Model.IsDead || BossStage != stage;
    }
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
        //yield return new WaitForSeconds(0.25f);
        //_rigidBody.linearVelocity = new Vector2(0, _rigidBody.linearVelocity.y);
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}
