using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Attack : MonoBehaviour
{
	public float dmgValue = 1;
	public GameObject throwableObject;
	public Transform attackCheck;
	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;
	public bool canAttack = true;
	public bool isTimeToCheck = false;

	public GameObject cam;

    private int maxAttackSeriesCount = 4;
	private int attackSeriesCount = 0;
    public float attackSeriesTimeout = 0.75f;
    private bool isAttackSeriesActive = false;
    private bool isFrozenInSeries = false;
    private float lastAttackTime = 0f;

	private CharacterController2D characterController2D;
    private RigidbodyConstraints2D originalConstraints;

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        characterController2D = GetComponent<CharacterController2D>();
    }

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		var gamepad = Gamepad.current;
        bool attackPressed = Input.GetKeyDown(KeyCode.X) ||
                             (gamepad != null && gamepad.xButton.wasPressedThisFrame);

        if (isAttackSeriesActive && (Time.time - lastAttackTime > attackSeriesTimeout))
        {
            dmgValue = 1;
            attackSeriesCount = 0;
            isAttackSeriesActive = false;
            UnfreezePlayer();
            Debug.Log("Серия прервалась");
        }

        if (attackPressed && canAttack)
		{
            if (!isAttackSeriesActive)
            {
                isAttackSeriesActive = true;
                attackSeriesCount = 0;
                StartCoroutine(FreezeWhileInSeriesAttack());
                Debug.Log("Серия началась");
            }

            lastAttackTime = Time.time;

            attackSeriesCount++;
            if (attackSeriesCount >= maxAttackSeriesCount)
            {
                dmgValue = 3;
                attackSeriesCount = 0;
                isAttackSeriesActive = false;
                Debug.Log("Серия законилась");
            }

            canAttack = false;
			animator.SetBool("IsAttacking", true);
			StartCoroutine(AttackCooldown(0.25f, 1f));
		}

        #region DISTANCE ATTACK
        // Может использоваться для оружий, но пока не надо

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //	GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity) as GameObject;
        //	Vector2 direction = new Vector2(transform.localScale.x, 0);
        //	throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction;
        //	throwableWeapon.name = "ThrowableWeapon";
        //}
        #endregion
    }

    IEnumerator AttackCooldown(float duration, float durationAfterSeries)
	{
        if (dmgValue == 1)
        {
            yield return new WaitForSeconds(duration);
            characterController2D.canMove = true;
        }
        else                                                                                                                  
		{
            yield return new WaitForSeconds(duration);
            characterController2D.canMove = true;
            yield return new WaitForSeconds(durationAfterSeries - duration);
            dmgValue = 1;
        }

        if (!isAttackSeriesActive)
            characterController2D.canMove = true;

        canAttack = true;
    }

    IEnumerator FreezeWhileInSeriesAttack()
    {
        if (isFrozenInSeries)
            yield break;

        isFrozenInSeries = true;
        originalConstraints = m_Rigidbody2D.constraints;
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        characterController2D.canMove = false;

        yield return new WaitWhile(() => isAttackSeriesActive);

        m_Rigidbody2D.rotation = 0;
        m_Rigidbody2D.constraints = originalConstraints;
        characterController2D.canMove = true;
        isFrozenInSeries = false;
    }

    private void UnfreezePlayer()
    {
        if (!isFrozenInSeries)
            return;

        m_Rigidbody2D.rotation = 0f;
        m_Rigidbody2D.constraints = originalConstraints;
        characterController2D.canMove = true;
        isFrozenInSeries = false;
    }

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
				cam.GetComponent<CameraFollow>().ShakeCamera();
			}
		}
	}
}
