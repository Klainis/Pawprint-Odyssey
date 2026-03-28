using UnityEngine;

public class BulletHandler : MonoBehaviour
{
	private int damage = 1;

    void Start()
    {
		damage = transform.parent.GetComponent<ThornyPlant>().Damage;
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.CompareTag("Player"))
		{
            collision.gameObject.GetComponent<PlayerView>().ApplyDamage(damage, transform.position, gameObject);
			Destroy(gameObject);
		}
		else if (!collision.gameObject.CompareTag("Player"))
			Destroy(gameObject);
	}
}
