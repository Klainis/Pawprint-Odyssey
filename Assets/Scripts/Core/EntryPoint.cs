using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private GameObject heart;
    [SerializeField] private GameObject manaBar;
    [SerializeField] private GameObject crystalCounter;
    [SerializeField] private GameObject deadManager;

    //private GameObject mainCamera;
    //private GameObject player;
    //private Transform initialPosition;
    //private GameObject canvas;
    //private GameObject heartsList;
    //private GameObject manaBar;
    //private GameObject crystalCounter;
    //private GameObject deadManager;

    private ReceivingClaw receivingClaw;
    private Heart heartScript;
    private Mana mana;
    public Image manaBarImage {  get; private set; }
    private Canvas componentCanvas;
    private CanvasScaler componentCanvasScaler;

    [SerializeField] private string startSceneName = "F_Room_Tutorial";

    public static EntryPoint _instance { get; private set; }

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

        deadManager = Instantiate(deadManager);
        DontDestroyOnLoad(deadManager);

        gameManager = Instantiate(gameManager);
        DontDestroyOnLoad(gameManager);

        initializeManager = Instantiate(initializeManager);
        DontDestroyOnLoad(initializeManager);

        canvas = Instantiate(canvas);
        SetCanvasParamets();
        DontDestroyOnLoad(canvas);

        eventSystem = Instantiate(eventSystem);
        DontDestroyOnLoad(eventSystem);

        InitializePlayer();

        manaBar = Instantiate(manaBar, canvas.transform);
        DontDestroyOnLoad(manaBar);

        crystalCounter = Instantiate(crystalCounter, canvas.transform);
        DontDestroyOnLoad(crystalCounter);

        //Нужна проверка получен ли Коготь у игрока
        receivingClaw = player.GetComponent<ReceivingClaw>();
        receivingClaw.enabled = false;

        EnableMana();

        hearts = Instantiate(hearts, canvas.transform);
        DontDestroyOnLoad(hearts);

        StartHearts();
    }

    private void InitializePlayer()
    {
        player = Instantiate(player);
        SetInitialPosition();
        DontDestroyOnLoad(player);

        InitializeManager._instance.player = player;
    }

    private void SetInitialPosition()
    {
        player.transform.position = initialPosition.position;
    }

    private void EnableMana()
    {
        manaBarImage = manaBar.transform.Find("Bar").GetComponent<Image>();

        //Нужна проверка активен ли у игрока компонент маны
        mana = player.GetComponent<Mana>();
        mana.manaBar = manaBarImage;
        mana.enabled = false;   

        //Проверка на то есть ли сейчас манабар  у игрока. Если есть, то установить значение из json
        // Сейчас ScOb, сделать проверку через json
        if (mana.enabled)
            manaBar.SetActive(true);
        else
            manaBar.SetActive(false);
    }

    private void SetCanvasParamets()
    {
        componentCanvas = canvas.GetComponent<Canvas>();
        componentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        componentCanvas.worldCamera = mainCamera.GetComponent<Camera>();
    }

    private void StartHearts()
    {
        heartScript = player.GetComponent<Heart>();

        heartScript.hearts = hearts;
        heartScript.StartHearts();
        //Логика назначения текущего HP при запуске игры. Через json
    }

    private void StartMana()
    {
        //Логика назначения текущей маны при запуске игры. Через json

        //Mana.cs
        //manaBar.fillAmount = (float)Data.currentMana / (float)Data.maxMana;
    }

    private bool isGetClaw()
    {
        throw new NotImplementedException();
    }
}
