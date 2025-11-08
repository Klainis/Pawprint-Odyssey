using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    private SpiritGuide spiritGuide;
    private Image bossHealthBar;
    private GameObject bossHealth;
    [SerializeField] private GameObject health;
    [SerializeField] private Transform canvas;
    void Start()
    {
        spiritGuide = GetComponent<SpiritGuide>();
    }

    public void InstantiateBossHealth()
    {
        if (GameObject.FindWithTag("BossHealth") || spiritGuide.lifeForReading <= 0)
        {
            return;
        }

        bossHealth = Instantiate(health, canvas);
        bossHealthBar = bossHealth.transform.Find("BossHealthBar").GetComponent<Image>();
    }

    public void HitBoss()
    {
        bossHealthBar.fillAmount = (float)spiritGuide.lifeForReading / (float)spiritGuide.maxLifeForReading;
    }

    public void DestroyBossHealthSlider()
    {
        Destroy(bossHealth);
    }
}
