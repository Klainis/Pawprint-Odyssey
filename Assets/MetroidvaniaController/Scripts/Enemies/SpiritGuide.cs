using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class SpiritGuide : MonoBehaviour {

    [Header("Основные параметры")]
    [SerializeField] private float life = 10;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 2f;
	[SerializeField] private bool isInvincible = false;
    [SerializeField] private LayerMask turnLayerMask;
    [SerializeField] private LayerMask playerLayer;

	[Header("Механика 'Таран'")]
    [SerializeField] private float acceleratedSpeed = 10f;
    [SerializeField] private float ramTelegraphTime = 0.25f;

    [Header("Механика 'Зона света'")]
    [SerializeField] private float zoneRadius = 1.5f;
    [SerializeField] private float zoneTelegraphTime = 0.25f;

    private Rigidbody2D rb;
    private Animator animator;

	private bool isObstacle;
	private Transform wallCheck;
	private bool facingRight = true;
	private bool isHitted = false;
    private bool isAccelerated = false;


    void Awake () {
		wallCheck = transform.Find("WallCheck");
		rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (life <= 0) {
            StartCoroutine(DestroyEnemy());
            return;
        }

		isObstacle = Physics2D.OverlapCircle(wallCheck.position, .2f, turnLayerMask);

        if (isHitted || Mathf.Abs(rb.linearVelocity.y) > 0.5f)
            return;

        Vector2 direction = facingRight ? Vector2.left : Vector2.right;
        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, direction, 100f, playerLayer);
        if (playerHit.collider != null && isAccelerated == false)
        {
            StartCoroutine(RamTelegraph(ramTelegraphTime));
            isAccelerated = true;
        }
        if (isObstacle)
        {
            isAccelerated = false;
            Turn();
        }

        var moveSpeed = isAccelerated ? acceleratedSpeed : speed;
        var moveDir = facingRight ? -1 : 1;
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
	}

    private void Turn()
    {
        if (facingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = !facingRight;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = !facingRight;
            //turnCoefficient = 1; используется в Charecter Controller как коэффициент == tranform.localscale.x
        }
    }

    public void ApplyDamage(float damage) {
		if (!isInvincible) 
		{
			//animator.SetBool("Hit", true);
			life -= Mathf.Abs(damage);
            StartCoroutine(HitTime(0.1f));
        }
	}

	void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
			if (life > 0)
			{
                collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damage, transform.position);
            }
        }
    }

    IEnumerator RamTelegraph(float time)
    {
        //animator.SetBool("RamTelegraph", true);
        var normalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(time);
        rb.constraints = normalConstraints;
    }

    IEnumerator HitTime(float time)
	{
		isHitted = true;
		isInvincible = true;
		yield return new WaitForSeconds(time);
		isHitted = false;
		isInvincible = false;
	}

	IEnumerator DestroyEnemy()
	{
        //animator.SetTrigger("Dead");
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        Vector3 rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
		rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
}
