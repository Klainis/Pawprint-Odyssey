using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private int life;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private int reward;

    public int Life { get { return life; } }
    public float Speed { get { return speed; } }
    public int Damage { get { return damage; } }
    public int Reward { get { return reward; } }
}
