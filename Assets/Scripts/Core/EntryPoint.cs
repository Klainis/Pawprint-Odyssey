using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private InputActionAsset newInputSystem;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private InitializeManager initializeManager;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform initialPosition;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject hearts;
    [SerializeField] private GameObject transitionFade;
    [SerializeField] private GameObject manaBar;
    [SerializeField] private GameObject crystalCounter;
    [SerializeField] private GameObject deadManager;
    [SerializeField] private GameObject globalValue;

    [SerializeField] private PlayerData playerData;

    //private GameObject mainCamera;
    //private GameObject player;
    //private Transform initialPosition;
    //private GameObject canvas;
    //private GameObject heartsList;
    //private GameObject manaBar;
    //private GameObject crystalCounter;
    //private GameObject deadManager;

    private PiercingClaw piercingClaw;
    private PlayerHeart playerHeart;
    private PlayerMana playerMana;
    private TransitionFade fadeScript;
    public Image manaBarImage { get; private set; }
    private Canvas componentCanvas;
    private Canvas componentTransitionCanvas;
    private GameObject TransitionCanvas;
    private CanvasScaler componentCanvasScaler;

    [SerializeField] private string startSceneName = "F_Room_Tutorial";

    public static EntryPoint _instance { get; private set; }

    private bool playerInitialized = false;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private async void Start()
    {
        CreateObjects();
        // loadingScreen.Show();
        await Initialize();

        await SceneManager.LoadSceneAsync(startSceneName);
        fadeScript.FadeIn();

        //InstallDependencySpiritGuide();
    }

    private async UniTask Initialize()
    {
        newInputSystem.Enable();
    }

    private void CreateObjects()
    {
        mainCamera = Instantiate(mainCamera);
        DontDestroyOnLoad(mainCamera);

        globalValue = Instantiate(globalValue);
        DontDestroyOnLoad(globalValue);

        deadManager = Instantiate(deadManager);
        DontDestroyOnLoad(deadManager);

        gameManager = Instantiate(gameManager);
        DontDestroyOnLoad(gameManager);

        initializeManager = Instantiate(initializeManager);
        DontDestroyOnLoad(initializeManager);
        InitializeCanvas();

        TransitionCanvas = Instantiate(canvas);
        SetTransitionCanvasParamets();
        DontDestroyOnLoad(TransitionCanvas);

        eventSystem = Instantiate(eventSystem);
        DontDestroyOnLoad(eventSystem);

        InitializeFade();

        InitializePlayer();

        manaBar = Instantiate(manaBar, canvas.transform);
        DontDestroyOnLoad(manaBar);
        InitializeManager._instance.manaBar = manaBar;

        InitializeSoulCrystalCounter();

        //Нужна проверка получен ли Коготь у игрока
        piercingClaw = player.GetComponent<PiercingClaw>();
        piercingClaw.enabled = false;

        EnableMana();

        hearts = Instantiate(hearts, canvas.transform);
        DontDestroyOnLoad(hearts);

        if (playerInitialized) StartHearts();
    }

    private void InitializeCanvas()
    {
        canvas = Instantiate(canvas);
        SetCanvasParamets();
        DontDestroyOnLoad(canvas);

        InitializeManager._instance.canvas = canvas.GetComponent<Canvas>();
    }

    private void InitializeFade()
    {
        transitionFade = Instantiate(transitionFade, TransitionCanvas.transform);
        DontDestroyOnLoad(transitionFade);
        var StartFade = transitionFade.GetComponent<CanvasGroup>();
        StartFade.alpha = 1f;
        fadeScript = transitionFade.GetComponent<TransitionFade>();
        fadeScript.enabled = true;
        fadeScript.canvasGroup = StartFade;
    }

    private void InitializeSoulCrystalCounter()
    {
        crystalCounter = Instantiate(crystalCounter, canvas.transform);
        DontDestroyOnLoad(crystalCounter);

        var text = crystalCounter.GetComponent<TMP_Text>();
        InitializeManager._instance.soulCrystalText = text;
    }

    public void InitializePlayer()
    {
        player = Instantiate(player);
        var playerModel = PlayerModel.CreateFromPlayerData(playerData);
        player.GetComponent<PlayerView>().PlayerModel = playerModel;
        SetInitialPosition();
        DontDestroyOnLoad(player);

        var receivingClawScript = player.GetComponent<ReceivingClaw>();
        receivingClawScript.enabled = true;
        var collider = player.GetComponent<BoxCollider2D>();
        collider.enabled = true;

        InitializeManager._instance.player = player;
        playerInitialized = true;
    }

    private void SetInitialPosition()
    {
        // Координаты начальной комнаты
        player.transform.position = /*initialPosition.position*/new Vector3(-150f, -4f, 0f);

        // Координаты комнаты с Claw
        //player.transform.position = new Vector3(35.97942f, -46.43025f, 0f);
    }

    private void EnableMana()
    {
        manaBarImage = manaBar.transform.Find("Bar").GetComponent<Image>();

        //Нужна проверка активен ли у игрока компонент маны
        playerMana = player.GetComponent<PlayerMana>();
        playerMana.SetManaBarImage(manaBarImage);
        playerMana.enabled = false;

        //Проверка на то есть ли сейчас манабар  у игрока. Если есть, то установить значение из json
        // Сейчас ScOb, сделать проверку через json
        if (playerMana.enabled)
            manaBar.SetActive(true);
        else
            manaBar.SetActive(false);
    }

    private void SetCanvasParamets()
    {
        componentCanvas = canvas.GetComponent<Canvas>();
        componentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        componentCanvas.worldCamera = mainCamera.GetComponent<Camera>();
        componentCanvas.sortingOrder = 0;
    }

    private void SetTransitionCanvasParamets()
    {
        componentTransitionCanvas = TransitionCanvas.GetComponent<Canvas>();
        componentTransitionCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        componentTransitionCanvas.worldCamera = mainCamera.GetComponent<Camera>();
        componentTransitionCanvas.sortingOrder = 100;
    }

    private void StartHearts()
    {
        playerHeart = player.GetComponent<PlayerHeart>();

        playerHeart.SetHeartsPrefab(hearts);
        playerHeart.StartHearts();
        //Логика назначения текущего HP при запуске игры. Через json
    }

    private bool isGetClaw()
    {
        throw new NotImplementedException();
    }
}
