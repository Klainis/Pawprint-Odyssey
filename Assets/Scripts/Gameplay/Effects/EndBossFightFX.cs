using UnityEngine;

public class EndBossFightFX : MonoBehaviour
{
    private static EndBossFightFX _instance;
    public static EndBossFightFX Instance {  get { return _instance; } }

    private enum Bosses
    {
        None,
        SpiritGuide,
        GuardianOwl
    }
    private Bosses _boss;

    [SerializeField] private string _sortingLayerName;
    [SerializeField] private GameObject _darkBackground;

    private GameObject _playerObject;
    private GameObject _spiritGuideObject;
    private GameObject _guardianOwlObject;

    private PlayerView _playerView;
    private SpiritGuideView _spiritGuideView;
    private GuardianOwlView _guardianOwlView;

    private string _initialPlayerLayer;
    private string _initialBossLayer;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;

        _playerObject = FindAnyObjectByType<PlayerView>()?.gameObject;
        _spiritGuideObject = FindAnyObjectByType<SpiritGuideView>()?.gameObject;
        _guardianOwlObject = FindAnyObjectByType<GuardianOwlView>()?.gameObject;

        if (_playerObject != null)
        {
            _playerView = _playerObject.GetComponent<PlayerView>();
            _initialPlayerLayer = _playerObject.GetComponent<SpriteRenderer>().sortingLayerName;
        }

        if (_spiritGuideObject != null)
        {
            _boss = Bosses.SpiritGuide;

            _spiritGuideView = _spiritGuideObject.GetComponent<SpiritGuideView>();

            foreach (var spriteRenderer in _spiritGuideObject.GetComponentsInChildren<SpriteRenderer>())
            {
                _initialBossLayer = spriteRenderer.sortingLayerName;
            }
        }
        else if (_guardianOwlObject != null)
        {
            _boss = Bosses.GuardianOwl;

            _guardianOwlView = _guardianOwlObject.GetComponent<GuardianOwlView>();

            foreach (var spriteRenderer in _guardianOwlObject.GetComponentsInChildren<SpriteRenderer>())
            {
                _initialBossLayer = spriteRenderer.sortingLayerName;
            }
        }
        else
        {
            _boss= Bosses.None;
            Debug.LogError("Не нашли ни одного босса в BossFight");
        }

        Debug.Log(_boss);
    }

    public void EnableEndFightFX()
    {
        _darkBackground.SetActive(true);

        GameManager.Instance.SetGameState(GlobalEnums.GameState.CUTSCENE);

        PlayerView.Instance.StopPlayer();
        PlayerView.Instance.FreezePlayerWithDisableMove(true);
        PlayerView.Instance.SetMaxMinFlashAmount(1);
        _playerObject.GetComponent<SpriteRenderer>().sortingLayerName = _sortingLayerName;

        switch(_boss)
        {
            case Bosses.SpiritGuide:
                foreach (var spriteRenderer in _spiritGuideObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteRenderer.sortingLayerName = _sortingLayerName;
                }
                _spiritGuideView.SetMaxMinFlashAmount(1);
                break;

            case Bosses.GuardianOwl:
                foreach (var spriteRenderer in _guardianOwlObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteRenderer.sortingLayerName = _sortingLayerName;
                }
                _guardianOwlView.SetMaxMinFlashAmount(1);
                break;
        }
    }

    public void DisableEndFightFX()
    {
        _darkBackground.SetActive(false);

        PlayerView.Instance.SetMaxMinFlashAmount(0);
        _playerObject.GetComponent<SpriteRenderer>().sortingLayerName = _initialPlayerLayer;

        switch (_boss)
        {
            case Bosses.SpiritGuide:
                foreach (var spriteRenderer in _spiritGuideObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteRenderer.sortingLayerName = _initialBossLayer;
                }
                _spiritGuideView.SetMaxMinFlashAmount(0);
                break;

            case Bosses.GuardianOwl:
                foreach (var spriteRenderer in _guardianOwlObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteRenderer.sortingLayerName = _initialBossLayer;
                }
                _guardianOwlView.SetMaxMinFlashAmount(0);
                break;
        }

        PlayerView.Instance.FreezePlayerWithDisableMove(false);
        GameManager.Instance.SetGameState(GlobalEnums.GameState.PLAYING);
    }
}
