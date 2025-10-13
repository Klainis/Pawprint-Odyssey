using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [Header("Основные параметры атаки")]
    [SerializeField] private int dmgValue = 1;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackSeriesTimeout = 0.9f; // время, за которое можно нажать след. удар в серии
    [SerializeField] private int maxAttackSeriesCount = 3;
    [SerializeField] Camera cam;

    private float lastAttackTime;
    private int attackSeriesCount = 0;
    private bool isAttacking = false;
    private bool canAttack = true;

    void Update()
    {
        animator.applyRootMotion = false;

        var gamepad = Gamepad.current;
        bool attackPressed = Input.GetKeyDown(KeyCode.X) ||
                             (gamepad != null && gamepad.xButton.wasPressedThisFrame);

        // Сброс, если игрок замешкался
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

    //  public void EndOfSeries()
    //  {
    //      canAttack = false;
    //      lastAttack = true;
    //      StartCoroutine(AttackCooldown(0.15f, 0.7f));     
    //      Debug.Log("Серия законилась");

    //}

    public void DoDashDamage()
	{
		dmgValue = Mathf.Abs(dmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
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
