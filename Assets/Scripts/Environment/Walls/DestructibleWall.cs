using UnityEngine;
using UnityEngine.Events;

public class DestructibleWall : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnvironmentData environmentData;
    [Space(5)]
    private ShakeObjectAfterDamage shakeObjectAfterDamage;

    private int life;

    private void Awake()
    {
        shakeObjectAfterDamage = GetComponent<ShakeObjectAfterDamage>();
    }

    private void Start()
    {
        life = environmentData.wallLife;
    }
    private void Update()
    {
        if (life <= 0)
        {
            Destroy(gameObject);
        }
        else if (shakeObjectAfterDamage.shakeDuration > 0)
        {
            shakeObjectAfterDamage.Shake();
        }
    }

    public void ApplyDamage()
    {
        life -= 1;
        shakeObjectAfterDamage.shakeDuration = environmentData.shakeDuration;
    }
}
