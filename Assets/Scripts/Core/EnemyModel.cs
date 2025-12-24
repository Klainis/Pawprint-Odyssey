using System;

public class EnemyModel
{
    public int Life { get; private set; }
    public float Speed { get; }
    public int Damage { get; }
    public int Reward { get; } // for 1 coin
    public bool IsDead { get { return Life <= 0; } }

    public EnemyModel(int life, float speed, int damage, int reward)
    {
        Life = life;
        Speed = speed;
        Damage = damage;
        Reward = reward;
    }

    public bool TakeDamage(int damage)
    {
        if (damage <= 0)
            return false;
        Life = Math.Max(0, Life - damage);
        return true;
    }
}
