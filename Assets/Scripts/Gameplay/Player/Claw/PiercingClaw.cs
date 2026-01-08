using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PiercingClaw : MonoBehaviour
{
    [SerializeField] private GameObject clawSprite;

    private Vector2 clawSize = new(4f, 0.6f);

    private PlayerView playerView;
    private PlayerAnimation playerAnimation;
    private PlayerMana playerMana;
    private PlayerMove playerMove;
    private Transform attackCheck;

    private bool canAttack = true;
    private bool clawPressed;

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
        playerMove = GetComponent<PlayerMove>();

        attackCheck = transform.Find("ClawAttackCheck");
        clawSprite.SetActive(false);
    }

    private void Update()
    {
        playerAnimation.ApplyRootMotion(false);
 
        //if (clawAction != null && clawAction.action != null && playerMove.IsGrounded)
        //    clawPressed = clawAction.action.WasPressedThisFrame();

        if (PlayerInput.Instance.PlayerClawEd && canAttack && playerView.PlayerModel.Mana >= 25)
        {
            clawSprite.SetActive(true);

            //Debug.Log("Claw");
            PlayerMana.Instance.SpendMana("Claw");

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
