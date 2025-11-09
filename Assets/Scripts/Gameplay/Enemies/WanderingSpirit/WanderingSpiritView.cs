using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingSpiritView : MonoBehaviour
{
    public WanderingSpiritModel Model { get; private set; }

    [Header("Main params")]
    [SerializeField] private EnemyData data;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Attack playerAttack;
    [SerializeField] private float lastPlayerAttackForce = 400f;
    [SerializeField] private float playerAttackForce = 300f;
    [SerializeField] private bool isInvincible = false;

    [Header("Acceleration")]
    [SerializeField] private float acceleratedSpeed = 5f;
    [SerializeField] private float playerDetectDist = 5f;

    private Rigidbody2D rigidBody;
    private WSAnimation wsAnimation;
    private WSMove wsMove;
    private WSAttack wsAttack;

    private bool isHitted = false;
    private bool isAccelerated = false;
    private bool facingRight = true;

    public LayerMask PlayerLayer { get { return playerLayer; } }
    public float PlayerDetectDist { get { return playerDetectDist; } }
    public Rigidbody2D RigidBody { get { return rigidBody; } }
    public bool IsHitted { get { return isHitted; } }
    public bool IsAccelerated { get { return isAccelerated; } set { isAccelerated = value; } }
    public bool FacingRight { get { return facingRight; } set { facingRight = value; } }

    private void Awake()
    {
        Model = new WanderingSpiritModel(data.Life, data.Speed, data.Damage);

        playerAttack = GameObject.Find("Player").GetComponent<Attack>();
        rigidBody = GetComponent<Rigidbody2D>();
        wsAnimation = GetComponent<WSAnimation>();
        wsMove = GetComponent<WSMove>();
        wsAttack = GetComponent<WSAttack>();
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
        if (!isInvincible)
        {
            StartCoroutine(HitTime(1.5f));
            wsAnimation.SetBoolHit(true);
            Model.TakeDamage(Mathf.Abs(damage));
            rigidBody.linearVelocity = Vector2.zero;

            var direction = damage / Mathf.Abs(damage);
            if (playerAttack.attackSeriesCount == 3)
                rigidBody.AddForce(new Vector2(direction * lastPlayerAttackForce, 0));
            else if (playerAttack.attackSeriesCount < 3)
                rigidBody.AddForce(new Vector2(direction * playerAttackForce, 0));
        }
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

        wsAnimation.SetTriggerDead();
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -45f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
        rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        yield return new WaitForSeconds(2f);
        
        ChangeLayer("DeadEnemy");
        ChangeTag("isDead");

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
