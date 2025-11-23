using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PiercingClaw : MonoBehaviour
{
    [SerializeField] private GameObject clawSprite;
    [SerializeField] private InputActionReference clawAction;

    private Vector2 clawSize = new(4f, 0.2f);

    private PlayerView playerView;
    private PlayerAnimation playerAnimation;
    private PlayerMana playerMana;
    private Transform attackCheck;
    private Rigidbody2D rb;
    private GameObject enemy;
    private Gamepad gamepad;

    private bool canAttack = true;
    private bool clawPressed;

    [SerializeField] private UnityEvent spendMana;

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
        playerView = GetComponent<PlayerView>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerMana = GetComponent<PlayerMana>();

        attackCheck = transform.Find("ClawAttackCheck");
        clawSprite.SetActive(false);
        gamepad = Gamepad.current;
    }

    private void Update()
    {
        playerAnimation.ApplyRootMotion(false);
 
        if (clawAction != null && clawAction.action != null)
            clawPressed = clawAction.action.WasPressedThisFrame();

        if (clawPressed && canAttack && playerView.PlayerModel.Mana >= 25)
        {
            clawSprite.SetActive(true);

            Debug.Log("Claw");
            if (spendMana != null)
                spendMana.Invoke();

            playerAnimation.SetTriggerClaw();
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
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);
            if (collidersEnemies[i].gameObject.CompareTag("ClawObject"))
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", new object[2] { true, damageToApply });
        }
    }

    private IEnumerator AttackCooldown(float durationAfterSeries)
    {
        yield return new WaitForSeconds(durationAfterSeries);
        clawSprite.SetActive(false);
        canAttack = true;
    }
}
