using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EntryPoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PlayerData playerData;

    [Header("PREFABS (Assets)")]
    [SerializeField] private WallsManager wallsManagerPrefab;
    [SerializeField] private CrystalsManager crystalsManagerPrefab;
    [SerializeField] private InitializeManager initializeManagerPrefab;
    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private GameObject mainCameraPrefab;
    [SerializeField] private GameObject globalValuePrefab;
    [SerializeField] private GameObject globalLightPrefab;
    [SerializeField] private GameObject deadManagerPrefab;
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject heartsPrefab;
    [SerializeField] private GameObject manaBarPrefab;
    [SerializeField] private GameObject crystalCounterPrefab;
    [SerializeField] private GameObject moneyCounterPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject pauseMenuCanvasPrefab;
    [SerializeField] private GameObject transitionFadePrefab;
    [SerializeField] private InputActionAsset newInputSystem;
    [SerializeField] private EventSystem eventSystemPrefab;
    [SerializeField] private Transform initialPosition;
    [SerializeField] private GameObject mapCanvasPrefab;
    [SerializeField] private GameObject mapManagerPrefab;
    [SerializeField] private GameObject moneyManagerPrefab;

    // INSTANCES (Runtime objects)
    private WallsManager _wallsManagerInstance;
    private CrystalsManager _crystalsManagerInstance;
    private InitializeManager _initializeManagerInstance;
    private GameManager _gameManagerInstance;
    private GameObject _mainCameraInstance;
    private GameObject _globalValueInstance;
    private GameObject _globalLightInstance;
    private GameObject _deadManagerInstance;
    private GameObject _canvasInstance;
    private GameObject _transitionCanvasInstance;
    private GameObject _heartsInstance;
    private GameObject _manaBarInstance;
    private GameObject _crystalCounterInstance;
    private GameObject _moneyCounterInstance;
    private GameObject _playerInstance;
    private GameObject _pauseMenuCanvasInstance;
    private EventSystem _eventSystemInstance;
    private GameObject _mapCanvasInstance;
    private GameObject _mapManagerInstance;
    private GameObject _moneyManagerInstance;

    private PiercingClaw piercingClaw;
    private PlayerHeart playerHeart;
    private PlayerMana playerMana;
    private TransitionFade fadeScript;
    public Image manaBarImage { get; private set; }
    private Canvas componentCanvas;
    private Canvas componentTransitionCanvas;
    private CanvasScaler componentCanvasScaler;

    private static EntryPoint instance;
    public static EntryPoint Instance { get { return instance; } }
    public InputActionAsset NewInputSystem { get { return newInputSystem; } }

    private bool playerInitialized = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private async void Start()
    {
        CreateObjects();
        // loadingScreen.Show();
        await Initialize();

        await SceneManager.LoadSceneAsync(PlayerView.Instance.PlayerModel.CurrentScene);
        fadeScript.FadeIn();

        //InstallDependencySpiritGuide();
    }

    private async UniTask Initialize()
    {
        newInputSystem.Enable();
    }

    private void CreateObjects()
    {
        if (mainCameraPrefab != null)
        {
            _mainCameraInstance = Instantiate(mainCameraPrefab);
            DontDestroyOnLoad(_mainCameraInstance);
        }

        if (globalValuePrefab != null)
        {
            _globalValueInstance = Instantiate(globalValuePrefab);
            DontDestroyOnLoad(_globalValueInstance);
        }

        if (globalLightPrefab != null)
        {
            _globalLightInstance = Instantiate(globalLightPrefab);
            DontDestroyOnLoad(_globalLightInstance);
        }

        if (deadManagerPrefab != null)
        {
            _deadManagerInstance = Instantiate(deadManagerPrefab);
            DontDestroyOnLoad(_deadManagerInstance);
        }

        if (GameManager.Instance == null)
        {
            if (gameManagerPrefab != null)
            {
                _gameManagerInstance = Instantiate(gameManagerPrefab);
                // DontDestroyOnLoad уже есть внутри Awake у GameManager
            }
        }

        if (wallsManagerPrefab != null)
        {
            _wallsManagerInstance = Instantiate(wallsManagerPrefab);
            DontDestroyOnLoad(_wallsManagerInstance);
        }

        if (mapManagerPrefab != null)
        {
            _mapManagerInstance = Instantiate(mapManagerPrefab);
            DontDestroyOnLoad(_mapManagerInstance);
        }

        if (crystalsManagerPrefab != null)
        {
            _crystalsManagerInstance = Instantiate(crystalsManagerPrefab);
            DontDestroyOnLoad(_crystalsManagerInstance);
        }

        if (moneyManagerPrefab != null)
        {
            _moneyManagerInstance = Instantiate(moneyManagerPrefab);
            DontDestroyOnLoad(_moneyManagerInstance);
        }

        if (initializeManagerPrefab != null)
        {
            _initializeManagerInstance = Instantiate(initializeManagerPrefab);
            DontDestroyOnLoad(_initializeManagerInstance);
            InitializeCanvas();
        }

        if (canvasPrefab != null)
        {
            _transitionCanvasInstance = Instantiate(canvasPrefab);
            SetTransitionCanvasParamets();
            DontDestroyOnLoad(_transitionCanvasInstance);
            InitializeFade();
        }

        if (mapCanvasPrefab != null)
        {
            _mapCanvasInstance = Instantiate(mapCanvasPrefab);
            _mapCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_mapCanvasInstance);
            GameManager.Instance.SetMapCanvasInstance(_mapCanvasInstance);
            MapManager.Instance.SetMapCanvasInstance(_mapCanvasInstance);
        }

        if (pauseMenuCanvasPrefab != null)
        {
            _pauseMenuCanvasInstance = Instantiate(pauseMenuCanvasPrefab);
            _pauseMenuCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_pauseMenuCanvasInstance);
            GameManager.Instance.SetPauseMenuCanvasInstance(_pauseMenuCanvasInstance);
        }

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            _eventSystemInstance = Instantiate(eventSystemPrefab);
            DontDestroyOnLoad(_eventSystemInstance);
        }

        InitializeDataFromSave();

        InitializePlayerUI();
    }

    public void DestroyAllSessionObjects()
    {
        if (_mainCameraInstance != null) Destroy(_mainCameraInstance);
        if (_globalValueInstance != null) Destroy(_globalValueInstance);
        if (_deadManagerInstance != null) Destroy(_deadManagerInstance);
        if (_wallsManagerInstance != null) Destroy(_wallsManagerInstance);
        if (_mapManagerInstance != null) Destroy(_mapManagerInstance);
        if (_crystalsManagerInstance != null) Destroy(_crystalsManagerInstance);
        if (_initializeManagerInstance != null) Destroy(_initializeManagerInstance);
        if (_transitionCanvasInstance != null) Destroy(_transitionCanvasInstance);
        if (_eventSystemInstance != null) Destroy(_eventSystemInstance.gameObject);
        if (_pauseMenuCanvasInstance != null) Destroy(_pauseMenuCanvasInstance);
        if (_canvasInstance != null) Destroy(_canvasInstance);
        if (_playerInstance != null) Destroy(_playerInstance);
        if (_mapCanvasInstance != null) Destroy(_mapCanvasInstance);

        Destroy(gameObject);
    }

    private void InitializeCanvas()
    {
        _canvasInstance = Instantiate(canvasPrefab);
        SetCanvasParamets();
        DontDestroyOnLoad(_canvasInstance);

        if (InitializeManager.Instance != null)
            InitializeManager.Instance.canvas = _canvasInstance.GetComponent<Canvas>();
    }

    private void InitializeFade()
    {
        if (transitionFadePrefab != null && _transitionCanvasInstance != null)
        {
            var transitionFadeObj = Instantiate(transitionFadePrefab, _transitionCanvasInstance.transform);

            var startFade = transitionFadeObj.GetComponent<CanvasGroup>();
            startFade.alpha = 1f;

            fadeScript = transitionFadeObj.GetComponent<TransitionFade>();
            fadeScript.enabled = true;
            fadeScript.canvasGroup = startFade;
        }
    }

    private void InitializeSoulCrystalCounter()
    {
        _crystalCounterInstance = Instantiate(crystalCounterPrefab, _canvasInstance.transform);
        DontDestroyOnLoad(_crystalCounterInstance);

        var text = _crystalCounterInstance.GetComponent<TMP_Text>();
        InitializeManager.Instance.soulCrystalText = text;
    }

    private void InitializeMoneyCounter()
    {
        _moneyCounterInstance = Instantiate(moneyCounterPrefab, _canvasInstance.transform);
        DontDestroyOnLoad(_moneyCounterInstance);

        var text = _moneyCounterInstance.GetComponent<TMP_Text>();
        InitializeManager.Instance.moneyText = text;
    }

    public void InitializeDataFromSave()
    {
        if (playerPrefab != null)
        {
            _playerInstance = Instantiate(playerPrefab);
            DontDestroyOnLoad(_playerInstance);
        }

        var playerView = _playerInstance.GetComponent<PlayerView>();
        var isLoaded = SaveSystem.TryLoad();

        if (isLoaded)
        {
            Debug.Log($"EntryPoint: Игра загружена из профиля {SaveSystem.CurrentProfileIndex}.");
        }
        else
        {
            Debug.Log($"EntryPoint: Сохранение из профиля {SaveSystem.CurrentProfileIndex} не найдено. Новая игра.");

            if (playerData != null)
            {
                var playerModel = PlayerModel.CreateFromPlayerData(playerData);
                playerView.PlayerModel = playerModel;
            }
            else
                Debug.LogError("CRITICAL: PlayerData не назначен в инспекторе EntryPoint!");

            SetInitialScene();
            SetInitialPosition();
            SaveSystem.Save();
            SaveSystem.AutoSave();
        }

        var receivingClawScript = _playerInstance.GetComponent<ReceivingClaw>();
        receivingClawScript.enabled = true;
        var collider = _playerInstance.GetComponent<BoxCollider2D>();
        collider.enabled = true;

        piercingClaw = _playerInstance.GetComponent<PiercingClaw>();
        if (piercingClaw) piercingClaw.enabled = false;

        InitializeManager.Instance.player = _playerInstance;
        playerInitialized = true;
    }

    public void InitializePlayerUI()
    {
        if (_canvasInstance != null)
        {
            if (_canvasInstance.transform.childCount > 0)
                for (var childIndex = 0; childIndex < _canvasInstance.transform.childCount; childIndex++)
                    Destroy(_canvasInstance.transform.GetChild(childIndex).gameObject);

            if (manaBarPrefab != null)
            {
                _manaBarInstance = Instantiate(manaBarPrefab, _canvasInstance.transform);
                DontDestroyOnLoad(_manaBarInstance);
                InitializeManager.Instance.manaBar = _manaBarInstance;
                EnableMana();
            }

            InitializeSoulCrystalCounter();
            InitializeMoneyCounter();

            if (heartsPrefab != null)
            {
                _heartsInstance = Instantiate(heartsPrefab, _canvasInstance.transform);
                DontDestroyOnLoad(_heartsInstance);
                if (playerInitialized)
                    StartHearts();
            }
        }
    }

    private void SetInitialPosition()
    {
        // Координаты начальной комнаты (только если нет сохраненной позиции)
        _playerInstance.transform.position = new Vector3(playerData.curPosX, playerData.curPosY, 0f);
        PlayerView.Instance.PlayerModel.SetCurrentPosition(playerData.curPosX, playerData.curPosY);
    }

    private void SetInitialScene()
    {
        GameManager.Instance.currentScene = "F_Room_Tutorial";
        Debug.Log(PlayerView.Instance.PlayerModel.CurrentScene);
    }

    public void SetPositionFromSave(Vector3 pos)
    {
        _playerInstance.transform.position = pos;
    }

    private void EnableMana()
    {
        manaBarImage = _manaBarInstance.transform.Find("Bar").GetComponent<Image>();

        playerMana = _playerInstance.GetComponent<PlayerMana>();
        playerMana.SetManaBarImage(manaBarImage);
        playerMana.enabled = false;

        if (playerMana.enabled)
            _manaBarInstance.SetActive(true);
        else
            _manaBarInstance.SetActive(false);
    }

    private void SetCanvasParamets()
    {
        componentCanvas = _canvasInstance.GetComponent<Canvas>();
        componentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        componentCanvas.worldCamera = _mainCameraInstance.GetComponent<Camera>();
        componentCanvas.sortingOrder = 0;
    }

    private void SetTransitionCanvasParamets()
    {
        componentTransitionCanvas = _transitionCanvasInstance.GetComponent<Canvas>();
        componentTransitionCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        componentTransitionCanvas.worldCamera = _mainCameraInstance.GetComponent<Camera>();
        componentTransitionCanvas.sortingOrder = 100;
    }

    private void StartHearts()
    {
        playerHeart = _playerInstance.GetComponent<PlayerHeart>();

        playerHeart.SetHeartsInstance(_heartsInstance);
        playerHeart.StartHearts();
    }
}
