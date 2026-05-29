using UnityEngine;

public class BulletHandler : MonoBehaviour
{
	private int damage = 1;

    public void SetDamage(int plantDamage)
    {
        damage = plantDamage;
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.CompareTag("Player"))
		{
            collision.gameObject.GetComponent<PlayerView>().ApplyDamage(damage, transform.position, gameObject);
			Destroy(gameObject);
		}
		else if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("EnemyBullet"))
			Destroy(gameObject);
	}
}
