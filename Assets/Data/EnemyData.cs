using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private int maxLife;
    [SerializeField] private float speed;
    [SerializeField] private int damage;

    public int MaxLife { get { return maxLife; } }
    public float Speed { get { return speed; } }
    public int Damage { get { return damage; } }
}
