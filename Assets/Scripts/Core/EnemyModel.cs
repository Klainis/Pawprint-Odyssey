using System;

public class EnemyModel
{
    public int Life { get; private set; }
    public float Speed { get; }
    public int Damage { get; }

    public EnemyModel(int life, float speed, int damage)
    {
        Life = life;
        Speed = speed;
        Damage = damage;
    }

    public void TakeDamage(int damage)
    {
        Life = Math.Max(0, Life - damage);
    }

    public bool IsDead { get { return Life <= 0; } }
}
