using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class Attack : MonoBehaviour
{
    [Header("Основные параметры атаки")]
    [SerializeField] private int dmgValue = 1;
    [SerializeField] private float attackSeriesTimeout = 0.9f; // время, за которое можно нажать след. удар в серии
    [SerializeField] private int maxAttackSeriesCount = 3;

    [SerializeField] private InputActionReference attackAction;

    const float attackCheckRadius = 1.1f;

    Gamepad gamepad;
    private Animator animator;
    private CharacterController2D playerController;
    private Transform attackCheck;
    private Rigidbody2D rb;
    private GameObject enemy;

    private bool attackPressed;
    private float lastAttackTime;
    public int attackSeriesCount { get; private set; } = 0;
    private bool isAttacking = false;
    private bool canAttack = true;

    [SerializeField] private UnityEvent<GameObject> getMana; 

    private void OnDrawGizmos()
    {
        if (attackCheck != null)
        {
            Gizmos.color = Color.brown;
            Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
        }
    }

    void Awake()
    {
        gamepad = Gamepad.current;

        attackCheck = transform.Find("AttackCheck");

        animator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        animator.applyRootMotion = false;

        if (attackAction != null && attackAction.action != null)
        {
            attackPressed = attackAction.action.WasPressedThisFrame();
        }

        if (attackSeriesCount > 0 && (Time.time - lastAttackTime > attackSeriesTimeout))
        {
            ResetCombo();
        }

        if (attackPressed && !isAttacking && canAttack)
        {
            lastAttackTime = Time.time;
            attackSeriesCount++;

            animator.SetBool("IsAttacking", true);

            if (attackSeriesCount == 1)
            {
                dmgValue = 1;
                animator.SetTrigger("Attack1");
                //AddForceForAttack();
            }
            else if (attackSeriesCount == 2)
            {
                dmgValue = 1;
                animator.SetTrigger("Attack2");
                //AddForceForAttack();
            }
            else if (attackSeriesCount == 3)
            {
                dmgValue = 3;
                animator.SetTrigger("Attack3");
                //AddForceForAttack();
            }

            isAttacking = true;
        }

    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        canAttack = false;

        if (attackSeriesCount >= maxAttackSeriesCount)
        {
            StartCoroutine(AttackCooldown(0.3f));
            Debug.Log("Серия завершена");
            ResetCombo();
        }
        else
            StartCoroutine(AttackCooldown(0.03f));
    }

    private void ResetCombo()
    {
        attackSeriesCount = 0;
        isAttacking = false;
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
    }

    IEnumerator AttackCooldown(float durationAfterSeries)
    {

        yield return new WaitForSeconds(durationAfterSeries);
        canAttack = true;
    }

    public void AttackDamage()
	{
		dmgValue = Mathf.Abs(dmgValue);
		var collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
		for (int i = 0; i < collidersEnemies.Length; i++)
		{
            GameObject enemy = collidersEnemies[i].gameObject;
            GameObject objectEnvironment = collidersEnemies[i].gameObject;

            if (enemy.CompareTag("Enemy") || objectEnvironment.CompareTag("SoulCrystal"))
			{
                float damageToApply = dmgValue;

                if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
				{
                    damageToApply = -damageToApply;
                }

                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);

                if (getMana != null && enemy.tag != "isDead")
                    getMana.Invoke(enemy);
                //cam.GetComponent<CameraFollow>().ShakeCamera();
            }
		}
	}
}
