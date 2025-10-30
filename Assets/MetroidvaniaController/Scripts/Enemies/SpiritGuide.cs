using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritGuide : MonoBehaviour {

    [Header("Основные параметры")]
    [SerializeField] private float life = 10;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float secondStageLifeCoef = 0.6f;
	[SerializeField] private bool isInvincible = false;
    [SerializeField] private LayerMask turnLayerMask;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    [Header("Механика 'Таран'")]
    [SerializeField] private float acceleratedSpeed = 10f;
    [SerializeField] private float ramTelegraphTime = 0.25f;
    [SerializeField] private float ramPauseBetweenSeries = 3f;

    [Header("Механика 'Зона света'")]
    [SerializeField] private GameObject lightZone;
    [SerializeField] private float lightZoneCooldown = 3f;
    [SerializeField] private float lightZoneChance = 0.6f;
    [SerializeField] private float lightZoneTime = 1f;
    [SerializeField] private float lightZoneTelegraphTime = 0.5f;

    const float groundedRadius = 0.2f;

    private Animator animator;
    private Rigidbody2D rb;
	private Transform wallCheck;
    private Transform groundCheck;
    private Transform pivotTop;
    private Transform pivotBottom;

    private int maxRamSeriesCount = 3;
    private int ramSeriesCount = 0;
    private float nextLightZoneTime = 0f;
    private float secondStageLifeAmount;
	private bool isObstacle;
    private bool facingRight = true;
	private bool isHitted = false;
    private bool isAccelerated = false;
    private bool moveDisabled = false;
    private bool isSecondStage = false;
    private bool inLightZone = false;

    void Awake () {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
		
        wallCheck = transform.Find("WallCheck");
        groundCheck = transform.Find("GroundCheck");
        pivotTop = transform.Find("PivotTop");
        pivotBottom = transform.Find("PivotBottom");

        secondStageLifeAmount = life * secondStageLifeCoef;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

		if (life <= 0) {
            StartCoroutine(DestroyEnemy());
            return;
        }
        if (moveDisabled || isHitted || Mathf.Abs(rb.linearVelocity.y) > 0.5f)
            return;

        var direction = facingRight ? Vector2.left : Vector2.right;
        var playerHitTop = Physics2D.Raycast(pivotTop.position, direction, 35, playerLayer);
        var playerHitBottom = Physics2D.Raycast(pivotBottom.position, direction, 35, playerLayer);
        var playerHits = new List<RaycastHit2D> { playerHitTop, playerHitBottom };
        var groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundedRadius, groundLayer);
        
        if (!isSecondStage && !isAccelerated && groundHit.collider != null)
        {
            for (var i = 0; i < playerHits.Count; i++)
            {
                if (playerHits[i].collider != null)
                {
                    StartCoroutine(RamTelegraph());
                    isAccelerated = true;
                    ramSeriesCount += 1;
                    break;
                }
            }
        }

        if (isSecondStage && !isAccelerated)
        {
            var wallHitLeft = Physics2D.Raycast(pivotBottom.position, Vector2.left, 6, groundLayer);
            var wallHitRight = Physics2D.Raycast(pivotBottom.position, Vector2.right, 6, groundLayer);
            if (wallHitLeft.collider == null && wallHitRight.collider == null && Time.time >= nextLightZoneTime)
            {
                if (Random.value <= lightZoneChance)
                {
                    StartCoroutine(LightZoneRoutine());
                    nextLightZoneTime = Time.time + lightZoneCooldown;
                }
                else if (!inLightZone)
                {
                    StartCoroutine(RamTelegraph());
                    isAccelerated = true;
                }
            }
        }

		isObstacle = Physics2D.OverlapCircle(wallCheck.position, 0.1f, turnLayerMask);
        if (isObstacle)
        {
            isAccelerated = false;
            Turn();
            if (ramSeriesCount == maxRamSeriesCount)
            {
                ramSeriesCount = 0;
                StartCoroutine(RamPause());
            }
        }

        var moveSpeed = isAccelerated ? acceleratedSpeed : speed;
        var moveDir = facingRight ? -1 : 1;
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
	}

    private void Turn()
    {
        if (facingRight)
        {
            var rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = !facingRight;
        }
        else
        {
            var rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
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
            if (life <= secondStageLifeAmount)
                isSecondStage = true;
            StartCoroutine(HitTime());
        }
	}

	void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
			if (life > 0)
			{
                collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damage, transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (life > 0)
            {
                collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damage, transform.position);
            }
        }
    }

    IEnumerator RamTelegraph()
    {
        //animator.SetBool("RamTelegraph", true);

        var renderer = GetComponent<SpriteRenderer>();
        var normalColor = renderer.color;
        renderer.color = UnityEngine.Color.red;

        var normalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(ramTelegraphTime);
        rb.constraints = normalConstraints;

        renderer.color = normalColor;
    }

    IEnumerator RamPause()
    {
        moveDisabled = true;
        var normalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(ramPauseBetweenSeries);
        rb.constraints = normalConstraints;
        moveDisabled = false;
    }

    IEnumerator LightZoneRoutine()
    {
        //animator.SetBool("LightZoneTelegraph", true);

        var renderer = GetComponent<SpriteRenderer>();
        var normalColor = renderer.color;
        renderer.color = UnityEngine.Color.lightYellow;

        inLightZone = true;
        var normalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(lightZoneTelegraphTime);
        lightZone.SetActive(true);
        yield return new WaitForSeconds(lightZoneTime);
        lightZone.SetActive(false);
        rb.constraints = normalConstraints;
        inLightZone = false;

        renderer.color = normalColor;
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
        //animator.SetTrigger("Dead");
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        var rotator = new Vector3(transform.rotation.x, transform.rotation.y, -90f);
        transform.rotation = Quaternion.Euler(rotator);
        yield return new WaitForSeconds(0.25f);
		rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
}
