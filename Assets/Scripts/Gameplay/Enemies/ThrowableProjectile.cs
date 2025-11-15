using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableProjectile : MonoBehaviour
{
	public Vector2 direction;
	public bool hasHit = false;
	public float speed = 15f;
	public GameObject owner;
	private Rigidbody2D rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
		if (!hasHit)
            rigidBody.linearVelocity = direction * speed;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.GetComponent<PlayerView>().ApplyDamage(2, transform.position);
			Destroy(gameObject);
		}
		else if (owner != null && collision.gameObject != owner && collision.gameObject.tag == "Enemy")
		{
			collision.gameObject.SendMessage("ApplyDamage", Mathf.Sign(direction.x) * 2f);
			Destroy(gameObject);
		}
		else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Player"))
			Destroy(gameObject);
	}
}
