using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using GlobalEnums;

public class SpiritGuideView : MonoBehaviour
{
    public EnemyModel Model { get; private set; }

    [Header("Objects")]
    [SerializeField] private GameObject _artefactObject;

    [Header("Main params")]
    [SerializeField] private EnemyData data;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float secondStageLifeCoef = 0.6f;
    [SerializeField] private float playerHitDistance = 40f;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private AudioClip _hitClip;

    [Header("Events")]
    [SerializeField] private UnityEvent<bool, bool> Hit;
    [SerializeField] private UnityEvent Die;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeapomSimpleSliceParticle;
    [SerializeField] private ParticleSystem _playerWeaponLastSliceParticle;

    private ParticleSystem _playerWeaponSimpleSliceAttackParticleInstance;
    private ParticleSystem _playerWeaponLastSliceAttackParticleInstance;

    [Space(5)]
    [SerializeField] private FightDoor _fightDoor;

    private const float groundedRadius = 0.2f;

    private Rigidbody2D rigidBody;

    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;
    private GameObject _artefactInstance;

    private AudioSource _audioSource;

    private SGAnimation sgAnimation;
    private SGAttack sgAttack;
    private SGMove sgMove;
    private ScreenShaker _screenShaker;
    private DamageFlash[] _damageFlash;
    private StunAudioController _stunAudioController;

    private int maxLifeForReading;

    private float secondStageLifeAmount;
    private float direction;

    private bool isSecondStage = false;
    private bool isHitted = false;
    private bool isAccelerated = false;
    private bool moveDisabled = false;
    private bool facingRight = false;
    private bool deathDone = false;

    public Rigidbody2D RigidBody { get { return rigidBody; } }
    public int MaxLifeForReading { get { return maxLifeForReading; } }
    public bool IsHitted { get { return isHitted; } }
    public bool IsAccelerated { get { return isAccelerated; } set { isAccelerated = value; } }
    public bool MoveDisabled { get { return moveDisabled; } set { moveDisabled = value; } }
    public bool FacingRight { get { return facingRight; } }

    private void Awake()
    {
        Model = new EnemyModel(data.Life, data.Speed, data.Damage, data.Reward);

        maxLifeForReading = Model.Life;
        secondStageLifeAmount = Model.Life * secondStageLifeCoef;
     
        rigidBody = GetComponent<Rigidbody2D>();
        
        sgAnimation = GetComponent<SGAnimation>();
        sgAttack = GetComponent<SGAttack>();
        sgMove = GetComponent<SGMove>();
        _damageFlash = GetComponentsInChildren<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();
        _audioSource = GetComponent<AudioSource>();
        _stunAudioController = GetComponent<StunAudioController>();
    }

    private void FixedUpdate()
    {
        if (deathDone)
            return;
        if (Model.IsDead)
        {
            deathDone = true;
            PlayerView.Instance.PlayerModel.SetSpiritGuideKilled();
            SaveSystem.AutoSave();
            Die.Invoke();
            StartCoroutine(DestroySelf());
            _fightDoor.CloseDoor(false);
            GameManager.Instance.SetGameState(GameState.PLAYING);
            return;
        }

        var playerHits = sgAttack.GetPlayerHits(playerHitDistance, facingRight);
        var isGrounded = CheckIfGrounded();
        if (!isSecondStage && !isAccelerated && isGrounded && !MoveDisabled)
            sgAttack.RamAttack(playerHits);
        if (isSecondStage && !isAccelerated && !MoveDisabled)
            sgAttack.RandomAttack(facingRight);

        sgMove.Move(isAccelerated, sgAttack.AcceleratedSpeed);
    }

    private bool CheckIfGrounded()
    {
        var groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundedRadius, groundLayer);
        return groundHit.collider != null;
    }

    public void ApplyDamage(int damage)
    {
        if (isInvincible) return;

        // sgAnimation.SetBoolHit(true);
        var damageApplied = Model.TakeDamage(Mathf.Abs(damage));
        if (damageApplied)
        {
            PlayHitSound(_hitClip);
            SetFlashAmount();

            _screenShaker.Shake();

            direction = damage / Mathf.Abs(damage);
            SpawnDamageParticles(direction);

            if (PlayerAttack.Instance.AttackSeriesCount >= 3)
            {
                SpawnPlayerLastAttackParticles();
            }
            else if (PlayerAttack.Instance.AttackSeriesCount < 3)
            {
                SpawnPlayerAttakParticles(direction);
            }

            Hit.Invoke(false, true);
            if (Model.Life <= secondStageLifeAmount)
                isSecondStage = true;
            StartCoroutine(HitTime());
        }
    }

    public void SetMaxMinFlashAmount(float value)
    {
        foreach (var damageFlash in _damageFlash)
        {
            damageFlash.CallMaxMinFlashAmount(value);
        }
    }

    public void SetFlashAmount()
    {
        foreach (var damageFlash in _damageFlash)
        {
            damageFlash.CallDamageFlash();
        }
    }

    private void PlayHitSound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    public void ApplyChargeDamage(int damage)
    {
        ApplyDamage(damage);
    }

    private void SpawnDamageParticles(float direction)
    {
        var vectorDirection = new Vector2(direction, 0);
        var spawnRotation = Quaternion.FromToRotation(Vector2.right, vectorDirection);
        _damageParticleInstance = Instantiate(_damageParticle, transform.position, spawnRotation);
    }

    private void SpawnPlayerAttakParticles(float direction)
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
        sgMove.OnWallHit += HandleWallHit;
        //sgAttack.OnPlayerDetected += HandlePlayerDetected;
    }

    private void OnDisable()
    {
        sgMove.OnWallHit -= HandleWallHit;
        //sgAttack.OnPlayerDetected -= HandlePlayerDetected;
    }

    private void HandleWallHit()
    {
        isAccelerated = false;
        facingRight = sgMove.Turn(facingRight);
        sgAttack.CheckRamSeriesCountAndPause();
    }

    public void TurnToPlayer()
    {
        facingRight = sgMove.Turn(facingRight);
    }

    private void HandlePlayerDetected()
    {
        isAccelerated = true;
    }

    private IEnumerator HitTime()
    {
        isHitted = true;
        isInvincible = true;
        yield return new WaitForSeconds(0.1f);
        isHitted = false;
        isInvincible = false;
    }

    private IEnumerator DestroySelf()
    {
        isInvincible = true;
        ChangeTag("isDead");
        ChangeLayer("DeadEnemy");

        _stunAudioController.TriggerStun();

        Time.timeScale = 0.15f;

        sgAnimation.SetDeathAnimation();

        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePosition;

        EndBossFightFX.Instance.EnableEndFightFX();
        SpawnDamageParticles(direction);
        SpawnPlayerLastAttackParticles();

        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;
        EndBossFightFX.Instance.DisableEndFightFX();

        yield return new WaitForSeconds(2f);

        _artefactInstance = Instantiate(_artefactObject, transform.position, Quaternion.identity);
        var artefactRB = _artefactInstance.GetComponent<Rigidbody2D>();
        artefactRB.AddForce(new Vector2(50f, 50f));

        PlayerView.Instance.FreezePlayerWithDisableMove(false);

        yield return new WaitForSeconds(1f);

        PimenView.Instance.gameObject.GetComponent<PimenTalk>().WinFirstBoss();

        //Destroy(gameObject);
    }
}
