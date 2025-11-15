using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PiercingClaw : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData Data;

    [Header("Main Attack Params")]
    [SerializeField] private int dmgValue = 7;

    [SerializeField] private InputActionReference clawAction;

    private Vector2 clawSize = new Vector2(4f, 0.2f);

    private PlayerAnimation playerAnimation;
    private Mana mana;
    private Transform attackCheck;
    private Rigidbody2D rb;
    private GameObject enemy;
    private Gamepad gamepad;
    private GameObject clawSprite;

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

    void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        mana = GetComponent<Mana>();

        attackCheck = transform.Find("ClawAttackCheck");
        clawSprite = GameObject.Find("Claw");

        gamepad = Gamepad.current;
    }

    void Update()
    {
        playerAnimation.ApplyRootMotion(false);
 
        if (clawAction != null && clawAction.action != null)
            clawPressed = clawAction.action.WasPressedThisFrame();

        if (clawPressed && canAttack && Data.currentMana >= 25)
        {
            Debug.Log("claw");
            if (spendMana != null)
                spendMana.Invoke();

            playerAnimation.SetTriggerClaw();
            canAttack = false;
            StartCoroutine(AttackCooldown(1f));
        }

    }

    public void ClawDamage()
    {
        dmgValue = Mathf.Abs(dmgValue);
        var collidersEnemies = Physics2D.OverlapBoxAll(attackCheck.position, clawSize, 0f);
        for (var i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.CompareTag("Enemy"))
            {
                var damageToApply = dmgValue;
                if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                    damageToApply = -damageToApply;
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);
            }
            if (collidersEnemies[i].gameObject.CompareTag("ClawObject"))
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", true);
        }
    }

    private IEnumerator AttackCooldown(float durationAfterSeries)
    {
        yield return new WaitForSeconds(durationAfterSeries);
        canAttack = true;
    }
}
