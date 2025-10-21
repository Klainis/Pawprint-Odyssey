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
    private CharacterController2D controller;
    private Transform attackCheck;

    private float lastAttackTime;
    private int attackSeriesCount = 0;
    private bool isAttacking = false;
    private bool canAttack = true;

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
        controller = GetComponent<CharacterController2D>();
    }

    void Update()
    {
        animator.applyRootMotion = false;

        var gamepad = Gamepad.current;
        bool attackPressed = Input.GetKeyDown(KeyCode.X) ||
                             (gamepad != null && gamepad.xButton.wasPressedThisFrame);

        if (attackSeriesCount > 0 && (Time.time - lastAttackTime > attackSeriesTimeout))
        {
            Debug.Log("Серия прервалась");
            ResetCombo();
        }

        // Обработка нажатия
        if (attackPressed && !isAttacking && canAttack)
        {

            lastAttackTime = Time.time;
            attackSeriesCount++;

            animator.SetBool("IsAttacking", true);

            if (attackSeriesCount == 1)
            {
                dmgValue = 1;
                animator.SetTrigger("Attack1");
                //Debug.Log("Первый удар");
            }
            else if (attackSeriesCount == 2)
            {
                dmgValue = 1;
                animator.SetTrigger("Attack2");
                //Debug.Log("Второй удар");
            }
            else if (attackSeriesCount == 3)
            {
                dmgValue = 3;
                animator.SetTrigger("Attack3");
                canAttack = false;
                //Debug.Log("Третий удар");
            }

            isAttacking = true;
        }

    //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
    //animator.GetCurrentAnimatorStateInfo(0).IsName("AirAttack1"))
    //    {
    //        Debug.Log("Анимация атаки реально проигрывается");
    //    }

    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;

        if (attackSeriesCount >= maxAttackSeriesCount)
        {
            StartCoroutine(AttackCooldown(1f));
            Debug.Log("Серия завершена");
            ResetCombo();
        }
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
        //characterController2D.canMove = true;
        //yield return new WaitForSeconds(durationAfterSeries - duration);
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
				collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);
                //Debug.Log("Отправили урон");
                cam.GetComponent<CameraFollow>().ShakeCamera();
			}
		}
	}
}
