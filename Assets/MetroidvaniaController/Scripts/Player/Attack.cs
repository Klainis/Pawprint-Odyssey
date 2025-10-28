using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [Header("Основные параметры атаки")]
    [SerializeField] private int dmgValue = 1;
    [SerializeField] private float attackSeriesTimeout = 0.9f; // время, за которое можно нажать след. удар в серии
    [SerializeField] private int maxAttackSeriesCount = 3;
    [SerializeField] Camera cam;

    const float attackCheckRadius = 0.9f;

    private Animator animator;
    private CharacterController2D playerController;
    private Transform attackCheck;
    private Rigidbody2D rb;
    private GameObject enemy;

    private float lastAttackTime;
    private int attackSeriesCount = 0;
    private bool isAttacking = false;
    private bool canAttack = true;
    private float attackForce = 500f;
    private bool isForceAttack = true;

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
        attackCheck = transform.Find("AttackCheck");

        animator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();

        //enemy = FindAnyObjectByType<GameObject>();
    }

    void Update()
    {
        animator.applyRootMotion = false;

        var gamepad = Gamepad.current;
        bool attackPressed = Input.GetKeyDown(KeyCode.X) ||
                             (gamepad != null && gamepad.xButton.wasPressedThisFrame);

        if (attackSeriesCount > 0 && (Time.time - lastAttackTime > attackSeriesTimeout))
        {
            ResetCombo();
        }

        CheckTurn();
        CheckAddForceForAttack();
        //Debug.Log(isForceAttack);

        //Debug.Log(canAttack);
        //Debug.Log(isAttacking);
        if (attackPressed && !isAttacking && canAttack)
        {

            lastAttackTime = Time.time;
            attackSeriesCount++;

            animator.SetBool("IsAttacking", true);

            if (attackSeriesCount == 1)
            {
                dmgValue = 1;
                animator.SetTrigger("Attack1");
                AddForceForAttack();
            }
            else if (attackSeriesCount == 2)
            {
                dmgValue = 1;
                animator.SetTrigger("Attack2");
                AddForceForAttack();
            }
            else if (attackSeriesCount == 3)
            {
                dmgValue = 3;
                animator.SetTrigger("Attack3");
                AddForceForAttack();
            }

            isAttacking = true;
        }

    }

    private void AddForceForAttack()
    {
        if (isForceAttack)
        {
            Debug.Log("++ Force");
            rb.AddForce(new Vector2(attackForce, 0));
        }
    }

    private void CheckAddForceForAttack()
    {
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        if (collidersEnemies != null)
        {
            for (int i = 0; i < collidersEnemies.Length; i++)
            {
                if (collidersEnemies[i].gameObject.tag == "Enemy")
                {
                    if (Math.Abs(collidersEnemies[i].transform.position.x - transform.position.x) < 1.5f)
                    {
                        isForceAttack = false;
                        Debug.Log(isForceAttack);
                    }
                    else
                    {
                        isForceAttack = true;
                    }
                }
            }
        }
        else
            isForceAttack = true;
    }

    private void CheckTurn()
    {
        if (playerController.turnCoefficient == 1 && attackForce < 0)
            attackForce = -1 * attackForce;
        else if (playerController.turnCoefficient == -1 && attackForce > 0)
            attackForce = -1 * attackForce;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        canAttack = false;

        if (attackSeriesCount >= maxAttackSeriesCount)
        {
            StartCoroutine(AttackCooldown(0.7f));
            Debug.Log("Серия завершена");
            ResetCombo();
        }
        else
            StartCoroutine(AttackCooldown(0.2f));
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

    public void DoDashDamage()
	{
		dmgValue = Mathf.Abs(dmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Enemy")
			{
                float damageToApply = dmgValue;
                if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
				{
                    damageToApply = -damageToApply;
                }
                if (Math.Abs(collidersEnemies[i].transform.position.x - transform.position.x) < 0.2f)
                {
                    isForceAttack = false;
                    Debug.Log(isForceAttack);
                }
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);
                //cam.GetComponent<CameraFollow>().ShakeCamera();
			}
		}
	}
}
