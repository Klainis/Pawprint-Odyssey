using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private GameObject _health;
    [SerializeField] private Transform _canvas;

    private SpiritGuideView _sgView;
    private GuardianOwlView _guardianOwlView;
    private Image _bossHealthBar;
    private GameObject _bossHealth;

    private void Awake()
    {
        _canvas = InitializeManager._instance.canvas.transform;
    }

    void Start()
    {
        _sgView = GetComponent<SpiritGuideView>();
        _guardianOwlView = GetComponent<GuardianOwlView>();
    }

    public void InstantiateBossHealth()
    {
        if (GameObject.FindWithTag("BossHealth") || 
            _sgView? _sgView.Model.IsDead : false || 
            _guardianOwlView? _guardianOwlView.Model.IsDead : false)
            return;

        _bossHealth = Instantiate(_health, _canvas);
        _bossHealthBar = _bossHealth.transform.Find("BossHealthBar").GetComponent<Image>();

        Debug.Log(_bossHealthBar);
        Debug.Log(_bossHealth.transform.Find("BossHealthBar").GetComponent<Image>().gameObject.name);
    }

    public void HitBoss(bool owlBoss, bool sgBoss)
    {
        if (sgBoss)
        {
            _bossHealthBar.fillAmount = (float)_sgView.Model.Life / (float)_sgView.MaxLifeForReading;
        }
        if (owlBoss)
        {
            _bossHealthBar.fillAmount = (float)_guardianOwlView.Model.Life / (float)_guardianOwlView.MaxLifeForReading;
        }
    }

    public void DestroyBossHealthSlider()
    {
        Destroy(_bossHealth);
    }
}
