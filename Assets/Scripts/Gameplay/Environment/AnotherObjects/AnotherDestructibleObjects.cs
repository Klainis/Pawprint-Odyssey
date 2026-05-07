using UnityEngine;

public class AnotherDestructibleObjects : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnvironmentData environmentData;
    [Space(5)]
    private ShakeObject shakeObjectAfterDamage;

    private int life;

    private void Awake()
    {
        shakeObjectAfterDamage = GetComponent<ShakeObject>();
    }

    private void Start()
    {
        life = environmentData.objectsLife;
    }
    private void Update()
    {
        if (life <= 0)
        {
            shakeObjectAfterDamage.Shake();
            Destroy(gameObject);
        }
        else if (shakeObjectAfterDamage.shakeDuration > 0)
        {
            shakeObjectAfterDamage.Shake();
        }
    }

    public void ApplyDamage(bool isClaw)
    {
        life -= 1;
        shakeObjectAfterDamage.shakeDuration = environmentData.shakeDuration;
    }
}
