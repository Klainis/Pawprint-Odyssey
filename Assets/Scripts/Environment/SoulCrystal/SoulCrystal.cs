using UnityEngine;
using UnityEngine.Events;

public class SoulCrystal : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] private EnvironmentData environmentData;
    [Space(5)]
    private ShakeObjectAfterDamage shakeObjectAfterDamage;

    private int life;

    [SerializeField] private UnityEvent crystalCountEvent;

    private void Awake()
    {
        shakeObjectAfterDamage = GetComponent<ShakeObjectAfterDamage>();
    }

    private void Start()
    {
        life = environmentData.crystalLife;
    }
    private void Update()
    {
        if (life <= 0)
        {
            shakeObjectAfterDamage.Shake();
            crystalCountEvent.Invoke();
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
