using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

	private int attackSeriesCount = 0;
    public float attackSeriesTimeout = 0.6f;
    private float lastAttackTime = 0f;

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
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

        if (attackPressed && canAttack)
		{
            float timeSinceLastAttack = Time.time - lastAttackTime;
            if (timeSinceLastAttack > attackSeriesTimeout)
            {
                attackSeriesCount = 0;
                dmgValue = 1;
            }

            attackSeriesCount++;
            lastAttackTime = Time.time;

            if (attackSeriesCount >= 4)
            {
                dmgValue = 3;
                attackSeriesCount = 0;
            }

			Debug.Log(dmgValue);

            canAttack = false;
			animator.SetBool("IsAttacking", true);
			StartCoroutine(AttackCooldown());
		}

		// Может использоваться для оружий, но пока не надо

		//if (Input.GetKeyDown(KeyCode.V))
		//{
		//	GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity) as GameObject;
		//	Vector2 direction = new Vector2(transform.localScale.x, 0);
		//	throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction;
		//	throwableWeapon.name = "ThrowableWeapon";
		//}
	}

	IEnumerator AttackCooldown()
	{
        if (dmgValue == 1)
		{
            yield return new WaitForSeconds(0.25f);
        }
		else
		{
			yield return new WaitForSeconds(1f);
            dmgValue = 1;
        }
        canAttack = true;
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
