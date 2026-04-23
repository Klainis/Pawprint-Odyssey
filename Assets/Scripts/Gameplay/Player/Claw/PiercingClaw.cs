using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PiercingClaw : MonoBehaviour
{
    [SerializeField] private GameObject clawSprite;

    private static PiercingClaw _instance;

    private Vector2 clawSize = new(4f, 0.6f);

    private PlayerView playerView;
    private PlayerAnimation playerAnimation;
    private PlayerMana playerMana;
    private PlayerMove playerMove;
    private Transform attackCheck;
    private Rigidbody2D _rigidbody;

    private bool canAttack = true;
    private bool isAttacking = false;
    private bool clawPressed;

    public static PiercingClaw Instance { get { return _instance; } }
    public bool IsAttacking { get { return isAttacking; } }

    private void OnDrawGizmos()
    {
        if (attackCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(attackCheck.position, clawSize);
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        playerView = GetComponent<PlayerView>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerMana = GetComponent<PlayerMana>();
        playerMove = GetComponent<PlayerMove>();
        _rigidbody = GetComponent<Rigidbody2D>();

        attackCheck = transform.Find("ClawAttackCheck");
        clawSprite.SetActive(false);
    }

    private void Update()
    {
        if (isAttacking)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.gravityScale = 0f;
        }

        playerAnimation.ApplyRootMotion(false);
 
        //if (clawAction != null && clawAction.action != null && playerMove.IsGrounded)
        //    clawPressed = clawAction.action.WasPressedThisFrame();

        if (PlayerInput.Instance.PlayerClawEd && canAttack && playerView.PlayerModel.Mana >= 25)
        {
            clawSprite.SetActive(true);
            isAttacking = true;

            //Debug.Log("Claw");
            playerMana.SpendMana("Claw", 1);

            playerAnimation.SetBoolClaw(true);
            canAttack = false;
            StartCoroutine(AttackCooldown(1f));
        }
    }

    public void ClawDamage()
    {
        var collidersEnemies = Physics2D.OverlapBoxAll(attackCheck.position, clawSize, 0f);
        for (var i = 0; i < collidersEnemies.Length; i++)
        {
            var damageToApply = playerView.PlayerModel.ClawDamage;
            if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                damageToApply = -damageToApply;

            if (collidersEnemies[i].gameObject.CompareTag("Enemy"))
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply, SendMessageOptions.DontRequireReceiver);
            if (collidersEnemies[i].gameObject.CompareTag("ClawObject"))
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", new object[2] { true, damageToApply });
        }
    }

    private IEnumerator AttackCooldown(float durationAfterSeries)
    {
        var clawEnd = 0.4f;
        yield return new WaitForSeconds(clawEnd);
        playerAnimation.SetBoolClaw(false);
        clawSprite.SetActive(false);
        isAttacking = false;
        yield return new WaitForSeconds(durationAfterSeries - clawEnd);
        canAttack = true;
    }
}
