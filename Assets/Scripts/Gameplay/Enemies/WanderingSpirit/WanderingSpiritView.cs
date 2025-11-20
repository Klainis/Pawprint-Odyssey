using System.Collections;
using UnityEngine;

public class WanderingSpiritView : MonoBehaviour
{
    public EnemyModel Model { get; private set; }

    [Header("Main params")]
    [SerializeField] private EnemyData data;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private float lastPlayerAttackForce = 400f;
    [SerializeField] private float playerAttackForce = 300f;
    [SerializeField] private bool isInvincible = false;

    [Header("Acceleration")]
    [SerializeField] private float acceleratedSpeed = 5f;
    [SerializeField] private float playerDetectDist = 5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem damageParticale;

    private ParticleSystem _damageParticleInstance;

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

        //playerAttack = GameObject.Find("Player").GetComponent<Attack>();
        playerAttack = InitializeManager._instance.player?.GetComponent<PlayerAttack>();
        Debug.Log(playerAttack == null);
        rigidBody = GetComponent<Rigidbody2D>();
        wsAnimation = GetComponent<WSAnimation>();
        wsAttack = GetComponent<WSAttack>();
        wsMove = GetComponent<WSMove>();
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
                rigidBody.AddForce(new Vector2(direction * lastPlayerAttackForce, 0));
            else if (playerAttack.AttackSeriesCount < 3)
                rigidBody.AddForce(new Vector2(direction * playerAttackForce, 0));
        }
    }

    private void SpawnDamageParticles(int direction)
    {
        Vector2 vectorDirection = new Vector2(direction, 0);
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, vectorDirection);
        _damageParticleInstance = Instantiate(damageParticale,transform.position, spawnRotation);
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

    private IEnumerator DestroySelf()
    {
        isInvincible = true;
        ChangeLayer("DeadEnemy");
        ChangeTag("isDead");

        wsAnimation.SetTriggerDead();
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -45f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
        rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    private IEnumerator HitTime(float time)
    {
        isHitted = true;
        //isInvincible = true;
        yield return new WaitForSeconds(time);
        isHitted = false;
        //isInvincible = false;
    }
}
