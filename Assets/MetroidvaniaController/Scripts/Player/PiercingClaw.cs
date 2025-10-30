using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PiercingClaw : MonoBehaviour
{
    [Header("Основные параметры атаки")]
    [SerializeField] private int dmgValue = 1;

    [SerializeField] private InputActionReference clawAction;

    private Vector2 clawSize = new Vector2(4f, 0.2f);

    private Animator animator;
    private CharacterController2D playerController;
    private Transform attackCheck;
    private Rigidbody2D rb;
    private GameObject enemy;
    private Gamepad gamepad;
    private GameObject clawSprite;

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

    void Awake()
    {
        attackCheck = transform.Find("ClawAttackCheck");
        clawSprite = GameObject.Find("Claw");

        gamepad = Gamepad.current;

        animator = GetComponent<Animator>();
        //playerController = GetComponent<CharacterController2D>();
        //rb = GetComponent<Rigidbody2D>();

        //enemy = FindAnyObjectByType<GameObject>();
    }

    void Update()
    {
        animator.applyRootMotion = false;
 
        if (clawAction != null && clawAction.action != null)
        {
            clawPressed = clawAction.action.WasPressedThisFrame();

        }

        if (clawPressed && canAttack)
        {
            Debug.Log("Умение");

            dmgValue = 7;
            animator.SetTrigger("Claw");
            canAttack = false;
            StartCoroutine(AttackCooldown(1f));
        }

    }

    IEnumerator AttackCooldown(float durationAfterSeries)
    {
        yield return new WaitForSeconds(durationAfterSeries);
        canAttack = true;
    }

    public void ClawDamage()
    {
        dmgValue = Mathf.Abs(dmgValue);
        Collider2D[] collidersEnemies = Physics2D.OverlapBoxAll(attackCheck.position, clawSize, 0f);
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
                //cam.GetComponent<CameraFollow>().ShakeCamera();
            }
        }
    }
}
