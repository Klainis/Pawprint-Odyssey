using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private GameObject health;
    [SerializeField] private Transform canvas;

    private SpiritGuideView sgView;
    private Image bossHealthBar;
    private GameObject bossHealth;

    private void Awake()
    {
        canvas = InitializeManager._instance.canvas.transform;
    }

    void Start()
    {
        canvas = gameObject.transform.Find("Canvas");
        sgView = GetComponent<SpiritGuideView>();
    }

    public void InstantiateBossHealth()
    {
        if (GameObject.FindWithTag("BossHealth") || sgView.Model.IsDead)
            return;

        bossHealth = Instantiate(health, canvas);
        bossHealthBar = bossHealth.transform.Find("BossHealthBar").GetComponent<Image>();
    }

    public void HitBoss()
    {
        bossHealthBar.fillAmount = (float)sgView.Model.Life / (float)sgView.MaxLifeForReading;
    }

    public void DestroyBossHealthSlider()
    {
        Destroy(bossHealth);
    }
}
