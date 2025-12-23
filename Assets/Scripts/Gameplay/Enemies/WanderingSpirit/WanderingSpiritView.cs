using System.Collections;
using UnityEngine;

public class WanderingSpiritView : MonoBehaviour
{
    public EnemyModel Model { get; private set; }

    [Header("Main params")]
    [SerializeField] private EnemyData data;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private float lastPlayerAttackForce = 12f;
    [SerializeField] private float playerAttackForce = 7f;
    [SerializeField] private bool isInvincible = false;

    [Header("Acceleration")]
    [SerializeField] private float acceleratedSpeed = 5f;
    [SerializeField] private float playerDetectDist = 5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;
    [SerializeField] private ParticleSystem _playerWeaponSliceParticle;
    private InstantiateMoney _money;

    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;
    private ParticleSystem _playerWeaponSliceParticleInstance;

    private Rigidbody2D rigidBody;
    private WSAnimation wsAnimation;
    private WSAttack wsAttack;
    private WSMove wsMove;
    private DamageFlash _damageFlash;
    private ScreenShaker _screenShaker;

    private bool isHitted = false;
    private bool isAccelerated = false;
    private bool facingRight = true;

    public float PlayerDetectDist { get { return playerDetectDist; } }
    public Rigidbody2D RigidBody { get { return rigidBody; } }
    public bool IsHitted { get { return isHitted; } }
    public bool IsAccelerated { get { return isAccelerated; } set { isAccelerated = value; } }
    public bool FacingRight { get { return facingRight; } set { facingRight = value; } }

    private void Awake()
    {
        Model = new EnemyModel(data.Life, data.Speed, data.Damage);

        //_playerAttack = GameObject.Find("Player").GetComponent<Attack>();
        playerAttack = InitializeManager.Instance.player?.GetComponent<PlayerAttack>();
        rigidBody = GetComponent<Rigidbody2D>();
        wsAnimation = GetComponent<WSAnimation>();
        wsAttack = GetComponent<WSAttack>();
        wsMove = GetComponent<WSMove>();
        _money = FindAnyObjectByType<InstantiateMoney>();
        _damageFlash = GetComponent<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();
    }

    private void FixedUpdate()
    {
        if (Model.IsDead)
        {
            StartCoroutine(DestroySelf());
            return;
        }
        else
            wsMove.Move(isAccelerated, acceleratedSpeed);
    }

    public void ApplyDamage(int damage)
    {
        if (isInvincible) return;

        var damageApplied = Model.TakeDamage(Mathf.Abs(damage));

        if (damageApplied)
        {
            _damageFlash.CallDamageFlash();

            wsAnimation.SetBoolHit(true);
            StartCoroutine(HitTime(1f));
            rigidBody.linearVelocity = Vector2.zero;

            var direction = damage / Mathf.Abs(damage);
            _screenShaker.Shake();
            SpawnDamageParticles(direction);

            if (playerAttack.AttackSeriesCount == 3)
            {
                //_rigidBody.AddForce(new Vector2(direction * _lastPlayerAttackForce, _rigidBody.linearVelocity.y), ForceMode2D.Impulse);
                KnockBack(direction, lastPlayerAttackForce);
            }
            else if (playerAttack.AttackSeriesCount < 3)
            {
                //_rigidBody.AddForce(new Vector2(direction * _playerAttackForce, _rigidBody.linearVelocity.y), ForceMode2D.Impulse);
                KnockBack(direction, playerAttackForce);
            }
        }
    }

    private void KnockBack(int direction, float forceAttack)
    {
        rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        Vector2 directionVector = new Vector2(direction, rigidBody.linearVelocity.y);

        rigidBody.AddForce(directionVector * forceAttack, ForceMode2D.Impulse);
    }

    private void SpawnDamageParticles(int direction)
    {
        Vector2 vectorDirection = new Vector2(direction, 0);
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, vectorDirection);
        Quaternion spawnPlayerAttackRotation = Quaternion.FromToRotation(Vector2.right, -vectorDirection);

        _damageParticleInstance = Instantiate(_damageParticle, transform.position, spawnRotation);

        _playerWeaponParticleInstance = Instantiate(_playerWeaponParticle, transform.position, spawnPlayerAttackRotation, transform);
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
        wsMove.OnWallHit += HandleWallHit;
        wsAttack.OnPlayerDetected += HandlePlayerDetected;
    }

    private void OnDisable()
    {
        wsMove.OnWallHit -= HandleWallHit;
        wsAttack.OnPlayerDetected -= HandlePlayerDetected;
    }

    private void HandleWallHit()
    {
        isAccelerated = false;
        facingRight = wsMove.Turn(facingRight);
    }

    private void HandlePlayerDetected()
    {
        isAccelerated = true;
    }

    private void OnDestroy()
    {
        _money.InstantiateMon(transform.position);
    }

    private IEnumerator DestroySelf()
    {
        isInvincible = true;
        ChangeLayer("DeadEnemy");
        ChangeTag("isDead");

        wsAnimation.SetTriggerDead();
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
        rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private IEnumerator HitTime(float time)
    {
        isHitted = true;
        //_isInvincible = true;
        yield return new WaitForSeconds(time);
        isHitted = false;
        //_isInvincible = false;
    }
}
