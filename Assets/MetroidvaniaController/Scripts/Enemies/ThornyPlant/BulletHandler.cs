using UnityEngine;

public class BulletHandler : MonoBehaviour
{
	private Rigidbody2D rb;

	private Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
		var player = collision.gameObject.CompareTag("Player");
        if (player)
		{
			direction = rb.linearVelocity.normalized;
			collision.gameObject.SendMessage("ApplyDamage", Mathf.Sign(direction.x) * 2f);
			Destroy(gameObject);
		}
		else if (!player)
		{
			Destroy(gameObject);
		}
	}
}
