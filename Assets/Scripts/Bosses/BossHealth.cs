using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    private SpiritGuide spiritGuide;
    private Slider bossHealthSlider;
    private GameObject bossHealth;
    [SerializeField] private GameObject health;
    [SerializeField] private Transform canvas;
    void Start()
    {
        spiritGuide = GetComponent<SpiritGuide>();
    }

    void Update()
    {
        
    }

    public void InstantiateBossHealth()
    {
        if (GameObject.FindWithTag("BossHealth") || spiritGuide.lifeForReading <= 0)
        {
            return;
        }

        bossHealth = Instantiate(health, canvas);
        bossHealthSlider = bossHealth.GetComponent<Slider>();
        bossHealthSlider.maxValue = spiritGuide.maxLifeForReading;
        bossHealthSlider.value = spiritGuide.lifeForReading;
        Debug.Log(bossHealthSlider.maxValue);
    }

    public void HitBoss()
    {
        bossHealthSlider.value = spiritGuide.lifeForReading;
        Debug.Log(spiritGuide.lifeForReading);
    }

    public void DestroyBossHealthSlider()
    {
        Debug.Log("Destroy Health Slider");
        Destroy(bossHealth);
    }
}
