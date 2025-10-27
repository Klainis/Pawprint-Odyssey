using UnityEngine;
using System.Collections;

public class WanderingSpirit : MonoBehaviour {

    [Header("Основные параметры")]
    [SerializeField] private float life = 10;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 1f;
	[SerializeField] private bool isInvincible = false;
    [SerializeField] private LayerMask turnLayerMask;
    [SerializeField] private LayerMask playerLayer;

	[Header("Ускорение")]
    [SerializeField] private float acceleratedSpeed = 15f;
    [SerializeField] private float playerDetectDistance = 5f;

    private Animator animator;
    private Rigidbody2D rb;
	private Transform fallCheck;
	private Transform wallCheck;

    private bool isPlat;
	private bool isObstacle;
	private bool facingRight = true;
	private bool isHitted = false;
    private bool isAccelerated = false;

    void Awake () {
		fallCheck = transform.Find("FallCheck");
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

        isPlat = Physics2D.OverlapCircle(fallCheck.position, .2f,turnLayerMask);
		isObstacle = Physics2D.OverlapCircle(wallCheck.position, .2f, turnLayerMask);

        if (isHitted || Mathf.Abs(rb.linearVelocity.y) > 0.5f)
            return;

        Vector2 direction = facingRight ? Vector2.left : Vector2.right;
        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, direction, playerDetectDistance, playerLayer);
		if (playerHit.collider != null)
		{
			isAccelerated = true;
		}
		if (!isPlat || isObstacle)
        {
            isAccelerated = false;
            Turn();
        }

        var moveSpeed = isAccelerated ? acceleratedSpeed : speed;
        var moveDir = facingRight ? -1 : 1;

        //if(!isHitted)
            //rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
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
            //turnCoefficient = 1; используется в Charecter Contriller как коэффициент == tranform.localscale.x
        }
    }

    public void ApplyDamage(float damage) {
		if (!isInvincible) 
		{
            animator.SetBool("Hit", true);
			//Debug.Log("Enemy получил урон");
			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);
			//transform.GetComponent<Animator>().SetBool("Hit", true);
			life -= damage;
			rb.linearVelocity = Vector2.zero;
			//rb.AddForce(new Vector2(direction * 500f, 0));
			StartCoroutine(HitTime(1f));
		}
	}

	void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAccelerated = false;

			if (life > 0)
			{
                collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damage, transform.position);
            }
        }
    }

    IEnumerator HitTime(float time)
	{
		isHitted = true;
		//isInvincible = true;
		yield return new WaitForSeconds(time);
		isHitted = false;
		//isInvincible = false;
	}

	IEnumerator DestroyEnemy()
	{
        animator.SetTrigger("Dead");
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        Vector3 rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
		rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
}
