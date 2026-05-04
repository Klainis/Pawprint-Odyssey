using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GlobalEnums;
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
    [SerializeField] private GameObject cameraManagerPrefab;
    [SerializeField] private GameObject cameraFollowObjectPrefab;
    [SerializeField] private GameObject globalVolumePrefab;
    [SerializeField] private GameObject globalLightPrefab;
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject heartsPrefab;
    [SerializeField] private GameObject mapEducationUIPrefab;
    [SerializeField] private GameObject manaBarPrefab;
    [SerializeField] private GameObject crystalCounterPrefab;
    [SerializeField] private GameObject moneyCounterPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject pimenPrefab;
    [SerializeField] private GameObject pauseMenuCanvasPrefab;
    [SerializeField] private GameObject optionsMenuCanvasPrefab;
    [SerializeField] private GameObject controlsMenuCanvasPrefab;
    [SerializeField] private GameObject controlsGamepadMenuCanvasPrefab;
    [SerializeField] private GameObject controlsKeyboardMenuCanvasPrefab;
    [SerializeField] private GameObject transitionFadePrefab;
    [SerializeField] private InputActionAsset newInputSystem;
    [SerializeField] private EventSystem eventSystemPrefab;
    [SerializeField] private Transform initialPosition;
    [SerializeField] private GameObject mapCanvasPrefab;
    [SerializeField] private GameObject mapManagerPrefab;
    [SerializeField] private GameObject gameMenuCanvasPrefab;
    [SerializeField] private GameObject abilitiesTreeCanvasPrefab;
    [SerializeField] private GameObject moneyManagerPrefab;
    [SerializeField] private GameObject musicHandlerPrefab;

    // INSTANCES (Runtime objects)
    private GameObject _musicHandlerInstance;
    private WallsManager _wallsManagerInstance;
    private CrystalsManager _crystalsManagerInstance;
    private InitializeManager _initializeManagerInstance;
    private GameManager _gameManagerInstance;
    private GameObject _mainCameraInstance;
    private GameObject _cameraManagerInstance;
    private GameObject _cameraFollowObjectInstance;
    private GameObject _globalVolumeInstance;
    private GameObject _globalLightInstance;
    private GameObject _canvasInstance;
    private GameObject _transitionCanvasInstance;
    private GameObject _mapEducationUIInstance;
    private GameObject _heartsInstance;
    private GameObject _manaBarInstance;
    private GameObject _crystalCounterInstance;
    private GameObject _moneyCounterInstance;
    private GameObject _playerInstance;
    private GameObject _pimenInstance;
    private GameObject _pauseMenuCanvasInstance;
    private GameObject _optionsMenuCanvasInstance;
    private GameObject _controlsMenuCanvasInstance;
    private GameObject _controlsGamepadMenuCanvasInstance;
    private GameObject _controlsKeyboardMenuCanvasInstance;
    private EventSystem _eventSystemInstance;
    private GameObject _mapCanvasInstance;
    private GameObject _gameMenuCanvasInstance;
    private GameObject _abilitiesTreeCanvasInstance;
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
    public GameObject GlobalVolumeInstance { get { return _globalVolumeInstance; } }
    public GameObject MusicHandlerInstance { get { return _musicHandlerInstance; } }

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

        if (!PlayerView.Instance.PlayerModel.StartCutSceneShowed)
        {
            await SceneManager.LoadSceneAsync("StartCutScene", LoadSceneMode.Single);
            GameManager.Instance.SetCutsceneState();
            await Task.Delay(14000);
        }

        await SceneManager.LoadSceneAsync(PlayerView.Instance.PlayerModel.CheckPointScene);
        _playerInstance.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GameManager.Instance.SetGameState(GameState.PLAYING);

        SaveSystem.Save();
        SaveSystem.AutoSave();

        fadeScript.StartGameFadeIn();

        //InstallDependencySpiritGuide();
    }

    private async UniTask Initialize()
    {
        newInputSystem.Enable();
    }

    private void CreateObjects()
    {
        if (cameraManagerPrefab != null)
        {
            _cameraManagerInstance = Instantiate(cameraManagerPrefab);
            DontDestroyOnLoad(_cameraManagerInstance);
        }

        if (mainCameraPrefab != null)
        {
            _mainCameraInstance = Instantiate(mainCameraPrefab/*, _cameraManagerInstance.transform*/);
            DontDestroyOnLoad(_mainCameraInstance);
        }

        if (globalVolumePrefab != null)
        {
            _globalVolumeInstance = Instantiate(globalVolumePrefab);
            DontDestroyOnLoad(_globalVolumeInstance);
        }

        if (globalLightPrefab != null)
        {
            _globalLightInstance = Instantiate(globalLightPrefab);
            DontDestroyOnLoad(_globalLightInstance);
        }

        if (eventSystemPrefab != null)
        {
            _eventSystemInstance = Instantiate(eventSystemPrefab);
            DontDestroyOnLoad(_eventSystemInstance);
        }

        if (musicHandlerPrefab != null)
        {
            _musicHandlerInstance = Instantiate(musicHandlerPrefab);
            DontDestroyOnLoad(_musicHandlerInstance);
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

        //if (moneyManagerPrefab != null)
        //{
        //    _moneyManagerInstance = Instantiate(moneyManagerPrefab);
        //    DontDestroyOnLoad(_moneyManagerInstance);
        //}

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
        //Запутанный порядок инициализации с GameManager, GameMenu и его вкладками
        if (gameMenuCanvasPrefab != null)
        {
            _gameMenuCanvasInstance = Instantiate(gameMenuCanvasPrefab);
            //SetCanvasParamets(_gameMenuCanvasInstance, 30);
            //SetCanvasCameraParamets(_gameMenuCanvasInstance, 100);
            _gameMenuCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_gameMenuCanvasInstance);
            GameManager.Instance.SetGameMenuCanvasInstance(_gameMenuCanvasInstance);
        }

        if (abilitiesTreeCanvasPrefab != null)
        {
            _abilitiesTreeCanvasInstance = Instantiate(abilitiesTreeCanvasPrefab, _gameMenuCanvasInstance.transform);
            //SetCanvasParamets(_abilitiesTreeCanvasInstance, 50);
            //SetCanvasCameraParamets(_abilitiesTreeCanvasInstance, 120);
            _abilitiesTreeCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_abilitiesTreeCanvasInstance);
            GameManager.Instance.SetAbilitiesTreeCanvasInstance(_abilitiesTreeCanvasInstance);
        }

        if (mapCanvasPrefab != null)
        {
            _mapCanvasInstance = Instantiate(mapCanvasPrefab, _gameMenuCanvasInstance.transform);
            //SetCanvasParamets(_mapCanvasInstance, 50);
            //SetCanvasCameraParamets(_mapCanvasInstance, 120);
            _mapCanvasInstance.SetActive(true);
            DontDestroyOnLoad(_mapCanvasInstance);
            GameManager.Instance.SetMapCanvasInstance(_mapCanvasInstance);
            MapManager.Instance.SetMapCanvasInstance(_mapCanvasInstance);
        }

        if (pauseMenuCanvasPrefab != null)
        {
            _pauseMenuCanvasInstance = Instantiate(pauseMenuCanvasPrefab);
            SetCanvasParamets(_pauseMenuCanvasInstance, 50);
            _pauseMenuCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_pauseMenuCanvasInstance);
            GameManager.Instance.SetPauseMenuCanvasInstance(_pauseMenuCanvasInstance);
        }

        if (optionsMenuCanvasPrefab != null)
        {
            _optionsMenuCanvasInstance = Instantiate(optionsMenuCanvasPrefab);
            SetCanvasParamets(_optionsMenuCanvasInstance, 50);
            _optionsMenuCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_optionsMenuCanvasInstance);
            GameManager.Instance.SetOptionsMenuCanvasInstance(_optionsMenuCanvasInstance);
        }

        if (controlsMenuCanvasPrefab != null)
        {
            _controlsMenuCanvasInstance = Instantiate(controlsMenuCanvasPrefab);
            SetCanvasParamets(_controlsMenuCanvasInstance, 50);
            _controlsMenuCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_controlsMenuCanvasInstance);
            GameManager.Instance.SetControlsMenuCanvasInstance(_controlsMenuCanvasInstance);
        }

        if (controlsGamepadMenuCanvasPrefab != null)
        {
            _controlsGamepadMenuCanvasInstance = Instantiate(controlsGamepadMenuCanvasPrefab);
            SetCanvasParamets(_controlsGamepadMenuCanvasInstance, 50);
            _controlsGamepadMenuCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_controlsGamepadMenuCanvasInstance);
            GameManager.Instance.SetControlsGamepadMenuCanvasInstance(_controlsGamepadMenuCanvasInstance);
        }

        if (controlsKeyboardMenuCanvasPrefab != null)
        {
            _controlsKeyboardMenuCanvasInstance = Instantiate(controlsKeyboardMenuCanvasPrefab);
            SetCanvasParamets(_controlsKeyboardMenuCanvasInstance, 50);
            _controlsKeyboardMenuCanvasInstance.SetActive(false);
            DontDestroyOnLoad(_controlsKeyboardMenuCanvasInstance);
            GameManager.Instance.SetControlsKeyboardMenuCanvasInstance(_controlsKeyboardMenuCanvasInstance);
        }

        InitializePlayerDataFromSave();

        if (cameraFollowObjectPrefab != null)
        {
            _cameraFollowObjectInstance = Instantiate(cameraFollowObjectPrefab);
            DontDestroyOnLoad(_cameraFollowObjectInstance);
        }

        InitializePimen();

        InitializePlayerUI();
    }

    public void DestroyAllSessionObjects()
    {
        if (_wallsManagerInstance != null) Destroy(_wallsManagerInstance.gameObject);
        if (_crystalsManagerInstance != null) Destroy(_crystalsManagerInstance.gameObject);
        if (_initializeManagerInstance != null) Destroy(_initializeManagerInstance.gameObject);
        if (_cameraManagerInstance != null) Destroy(_cameraManagerInstance.gameObject);
        if (_eventSystemInstance != null) Destroy(_eventSystemInstance.gameObject);

        if (_globalVolumeInstance != null) Destroy(_globalVolumeInstance);
        if (_globalLightInstance != null) Destroy(_globalLightInstance);
        if (_mainCameraInstance != null) Destroy(_mainCameraInstance);
        if (_cameraFollowObjectInstance != null) Destroy(_cameraFollowObjectInstance.gameObject);
        if (_mapManagerInstance != null) Destroy(_mapManagerInstance);
        if (_playerInstance != null) Destroy(_playerInstance);
        if (_pimenInstance != null) Destroy(_pimenInstance);
        if (_musicHandlerInstance != null) Destroy(_musicHandlerInstance);

        if (_transitionCanvasInstance != null) Destroy(_transitionCanvasInstance);
        if (_canvasInstance != null) Destroy(_canvasInstance);

        if (_mapCanvasInstance != null) Destroy(_mapCanvasInstance);
        if (_gameMenuCanvasInstance != null) Destroy(_gameMenuCanvasInstance);
        if (_abilitiesTreeCanvasInstance != null) Destroy(_abilitiesTreeCanvasInstance);

        if (_pauseMenuCanvasInstance != null) Destroy(_pauseMenuCanvasInstance);
        if (_optionsMenuCanvasInstance != null) Destroy(_optionsMenuCanvasInstance);
        if (_controlsMenuCanvasInstance != null) Destroy(_controlsMenuCanvasInstance);
        if (_controlsGamepadMenuCanvasInstance != null) Destroy(_controlsGamepadMenuCanvasInstance);
        if (_controlsKeyboardMenuCanvasInstance != null) Destroy(_controlsKeyboardMenuCanvasInstance);

        Destroy(gameObject);
    }

    private void InitializeCanvas()
    {
        _canvasInstance = Instantiate(canvasPrefab);
        SetCanvasParamets(_canvasInstance, 0);
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

    public void InitializePlayerDataFromSave()
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
            //SaveSystem.Save();
            //SaveSystem.AutoSave();
            _playerInstance.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // Ниебический костыль, надо будет как то потом поправить
        }

        var receivingClawScript = _playerInstance.GetComponent<ReceivingClaw>();
        receivingClawScript.enabled = true;
        var collider = _playerInstance.GetComponent<BoxCollider2D>();
        collider.enabled = true;

        _playerInstance.GetComponent<PlayerChargeAttack>().enabled = playerView.PlayerModel.HasChargedAttack;

        _playerInstance.GetComponent<PlayerSoulRelease>().enabled = playerView.PlayerModel.HasSoulRelease;

        _playerInstance.GetComponent<PlayerParrying>().enabled = playerView.PlayerModel.HasParrying;


        piercingClaw = _playerInstance.GetComponent<PiercingClaw>();
        if (piercingClaw) piercingClaw.enabled = false;

        playerMana = _playerInstance.GetComponent<PlayerMana>();
        if (playerView.PlayerModel.HasDamageDash || playerView.PlayerModel.HasChargedAttack || playerView.PlayerModel.HasClaw)
        {
            playerMana.enabled = true; 
        }
        else
        {
            playerMana.enabled = false;
        }

        InitializeManager.Instance.player = _playerInstance;
        playerInitialized = true;
    }

    public void InitializePimen()
    {
        if (pimenPrefab != null)
        {
            _pimenInstance = Instantiate(pimenPrefab);
            DontDestroyOnLoad(_pimenInstance);

            PimenMove _pimenMove = _pimenInstance.GetComponent<PimenMove>();
            _pimenMove.enabled = false;
        }
        else
        {
            Debug.LogError($"Отсутствует ссылка на префаб Пимена {pimenPrefab}");
        }
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
                Debug.Log($"Entry point{_manaBarInstance}");
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

            if (mapEducationUIPrefab != null)
            {
                _mapEducationUIInstance = Instantiate(mapEducationUIPrefab, _canvasInstance.transform);
                DontDestroyOnLoad (_mapEducationUIInstance);
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
        GameManager.Instance.CurrentScene = "F_Room_Tutorial";
        Debug.Log(PlayerView.Instance.PlayerModel.CurrentScene);
    }

    public void SetPositionFromSave(Vector2 pos)
    {
        _playerInstance.transform.position = new Vector3(pos.x, pos.y, 0);
    }

    private void EnableMana()
    {
        manaBarImage = _manaBarInstance.transform.Find("Bar").GetComponent<Image>();

        playerMana = _playerInstance.GetComponent<PlayerMana>();
        playerMana.SetManaBarImage(manaBarImage);
        //playerMana.enabled = false;

        if (playerMana.enabled)
            _manaBarInstance.SetActive(true);
        else
            _manaBarInstance.SetActive(false);
    }

    private void SetCanvasParamets(GameObject canvasInstance, int sortOrder)
    {
        componentCanvas = canvasInstance.GetComponent<Canvas>();
        componentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //componentCanvas.worldCamera = _mainCameraInstance.GetComponent<Camera>();
        componentCanvas.sortingOrder = sortOrder;
    }

    private void SetCanvasCameraParamets(GameObject canvasInstance, int sortOrder)
    {
        componentCanvas = canvasInstance.GetComponent<Canvas>();
        componentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        componentCanvas.worldCamera = _mainCameraInstance.GetComponent<Camera>();
        componentCanvas.sortingOrder = sortOrder;
    }

    private void SetTransitionCanvasParamets()
    {
        componentTransitionCanvas = _transitionCanvasInstance.GetComponent<Canvas>();
        //componentTransitionCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        //componentTransitionCanvas.worldCamera = _mainCameraInstance.GetComponent<Camera>();
        componentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        componentTransitionCanvas.sortingOrder = 1000;
    }

    private void StartHearts()
    {
        playerHeart = _playerInstance.GetComponent<PlayerHeart>();

        playerHeart.SetHeartsInstance(_heartsInstance);
        playerHeart.StartHearts();
    }
}
