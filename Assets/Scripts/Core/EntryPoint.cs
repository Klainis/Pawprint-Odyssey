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
    [SerializeField] private GameObject mainCamera;
    //[SerializeField] private GameObject cinemachineCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform initialPosition;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject hearts;
    [SerializeField] private GameObject manaBar;
    [SerializeField] private GameObject crystalCounter;
    [SerializeField] private GameObject deadManager;

    //[SerializeField] private GameObject spiritGuide;
    //[SerializeField] private GameObject wanderingSpirit;
    //[SerializeField] private GameObject spikes;
    //[SerializeField] private GameObject soulCrystal;
    //[SerializeField] private GameObject roomBounds;

    private ReceivingClaw receivingClaw;
    private Heart heart;
    private Mana mana;
    public Image manaBarImage {  get; private set; }
    private Canvas componentCanvas;
    private CanvasScaler componentCanvasScaler;

    private BossHealth bossHealth;
    

    private string startSceneName = "TestRoom1";
    


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

        SceneManager.LoadScene(startSceneName);

        //InstallDependencySpiritGuide();
    }

    //private void InstallDependencySpiritGuide()
    //{
    //    spiritGuide
    //}

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

        canvas = Instantiate(canvas);
        SetCanvasParamets();
        DontDestroyOnLoad(canvas);

        eventSystem = Instantiate(eventSystem);
        DontDestroyOnLoad(eventSystem);

        hearts = Instantiate(hearts, canvas.transform);
        DontDestroyOnLoad(hearts);

        player = Instantiate(player);
        SetInitialPosition();
        DontDestroyOnLoad(player);

        //Нужна проверка получен ли Коготь у игрока
        receivingClaw = player.GetComponent<ReceivingClaw>();
        receivingClaw.enabled = false;

        EnableMana();

        crystalCounter = Instantiate(crystalCounter, canvas.transform);
        DontDestroyOnLoad(crystalCounter);


        StartHearts();
    }

    private void SetInitialPosition()
    {
        player.transform.position = initialPosition.position;
    }

    private void EnableMana()
    {
        manaBar = Instantiate(manaBar, canvas.transform);
        DontDestroyOnLoad(manaBar);

        manaBarImage = manaBar.transform.Find("Bar").GetComponent<Image>();

        //Нужна проверка активен ли у игрока компонент маны
        mana = player.GetComponent<Mana>();
        mana.enabled = false;   

        //Проверка на то есть ли сейчас манабар  у игрока
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
        heart = player.GetComponent<Heart>();

        heart.StartHearts();
        //Логика назначения текущего HP при запуске игры. Через json

        //Heart.cs
        //if (hearts.Count != Data.maxLife)
        //{
        //    for (int i = hearts.Count - 1; i >= 0; i--)
        //    {
        //        if (i > (Data.currentLife - 1))
        //        {
        //            Destroy(hearts[i]);
        //            hearts.RemoveAt(i);
        //        }
        //    }
        //}
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
