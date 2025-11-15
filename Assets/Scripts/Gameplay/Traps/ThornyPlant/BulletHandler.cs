using UnityEngine;

public class BulletHandler : MonoBehaviour
{
	private int damage;

    void Start()
    {
		damage = transform.parent.GetComponent<ThornyPlant>().Damage;
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.CompareTag("Player"))
		{
            collision.gameObject.GetComponent<PlayerView>().ApplyDamage(damage, transform.position);
			Destroy(gameObject);
		}
		else if (!collision.gameObject.CompareTag("Player"))
			Destroy(gameObject);
	}
}
