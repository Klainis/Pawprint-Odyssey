using UnityEngine;

public class BulletHandler : MonoBehaviour
{
	private Rigidbody2D rb;

	private int damage;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

		damage = transform.parent.GetComponent<ThornyPlant>().Damage;
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.CompareTag("Player"))
		{
            collision.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damage, transform.position);
			Destroy(gameObject);
		}
		else if (!collision.gameObject.CompareTag("Player"))
		{
			Destroy(gameObject);
		}
	}
}
