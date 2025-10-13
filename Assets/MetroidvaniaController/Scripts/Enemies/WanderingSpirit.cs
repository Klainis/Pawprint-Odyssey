using UnityEngine;
using System.Collections;

public class WanderingSpirit : MonoBehaviour {

    [Header("Основные параметры")]
    [SerializeField] private float life = 10;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 2f;
    [SerializeField] private LayerMask turnLayerMask;
	[SerializeField] private bool isInvincible = false;

	[Header("Ускорение")]
    [SerializeField] private float acceleratedSpeed = 10f;
    [SerializeField] private float playerDetectDistance = 5f;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
	private bool isPlat;
	private bool isObstacle;
	private Transform fallCheck;
	private Transform wallCheck;
	private bool facingRight = true;
	private bool isHitted = false;
    private bool isAccelerated = false;

    void Awake () {
		fallCheck = transform.Find("FallCheck");
		wallCheck = transform.Find("WallCheck");
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (life <= 0) {
			//transform.GetComponent<Animator>().SetBool("IsDead", true);
			StartCoroutine(DestroyEnemy());
            return;
        }

        isPlat = Physics2D.OverlapCircle(fallCheck.position, .2f, 1 << LayerMask.NameToLayer("Default"));
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
            Flip();
        }

        var moveSpeed = isAccelerated ? acceleratedSpeed : speed;
        var moveDir = facingRight ? -1 : 1;
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
	}

	void Flip (){
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void ApplyDamage(float damage) {
		if (!isInvincible) 
		{
			//Debug.Log("Enemy получил урон");
			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);
			//transform.GetComponent<Animator>().SetBool("Hit", true);
			life -= damage;
			rb.linearVelocity = Vector2.zero;
			rb.AddForce(new Vector2(direction * 500f, 100f));
			StartCoroutine(HitTime());
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
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

    IEnumerator HitTime()
	{
		isHitted = true;
		isInvincible = true;
		yield return new WaitForSeconds(0.1f);
		isHitted = false;
		isInvincible = false;
	}

	IEnumerator DestroyEnemy()
	{
		CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
		capsule.size = new Vector2(1f, 0.25f);
		capsule.offset = new Vector2(0f, -0.8f);
		capsule.direction = CapsuleDirection2D.Horizontal;
		yield return new WaitForSeconds(0.25f);
		rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
}
