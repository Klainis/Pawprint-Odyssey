using UnityEngine;

public class PlayerGhostTrail : MonoBehaviour
{
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private float _delay;

    private GameObject _ghostInstance;
    private SpriteRenderer _playerSpriteRender;
    private SpriteRenderer _ghostSpriteRender;

    private float _elapsedTime;

    private void Awake()
    {
        _playerSpriteRender = GetComponent<SpriteRenderer>();
        _ghostSpriteRender = _ghostPrefab.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _elapsedTime = _delay;
    }

    private void Update()
    {
        _elapsedTime -= Time.deltaTime;
        if (PlayerMove.Instance.IsDashing && _elapsedTime < 0)
        {
            _elapsedTime = _delay;

            var ghostSprite = _playerSpriteRender.sprite;
            _ghostSpriteRender.sprite = ghostSprite;

            InstantiateGhostTrail();
        }
    }

    public void InstantiateGhostTrail()
    {
        _ghostInstance = Instantiate(_ghostPrefab, transform.position, transform.rotation);
        Debug.Log($"Создался префаб ghost: {_ghostInstance}");
    }
}
