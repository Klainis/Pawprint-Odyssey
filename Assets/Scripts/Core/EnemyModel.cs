using System;

public class EnemyModel
{
    public int Life { get; private set; }
    public float Speed { get; }
    public int Damage { get; }
    public bool IsDead { get { return Life <= 0; } }

    public EnemyModel(int life, float speed, int damage)
    {
        Life = life;
        Speed = speed;
        Damage = damage;
    }

    public bool TakeDamage(int damage)
    {
        if (damage <= 0)
            return false;
        Life = Math.Max(0, Life - damage);
        return true;
    }
}
