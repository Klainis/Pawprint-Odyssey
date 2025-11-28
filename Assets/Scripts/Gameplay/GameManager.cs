using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using GlobalEnums;
using Cysharp.Threading.Tasks.Triggers;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance { get; private set; }
    public GameState GameState { get; private set; }

    private TransitionDestination destination;
    private TransitionDestination[] destinations;
    private AudioListener Sounds;
    private BossHealth bossHealth;
    private GameObject _playerCached;

    private readonly string mainMenuSceneName = "MainMenu";
    private readonly string savesMenuSceneName = "SavesMenu";
    private readonly string entryPointSceneName = "EntryPoint";

    private bool isTransitioning;

    private GameObject Player
    {
        get
        {
            if (_playerCached == null)
            {
                if (PlayerView.Instance != null)
                    _playerCached = PlayerView.Instance.gameObject;
                else
                    _playerCached = GameObject.FindGameObjectWithTag("Player");
            }
            return _playerCached;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        SetGameState(GameState.PLAYING);

        Sounds = GameManager.FindAnyObjectByType<AudioListener>();
    }

    public void BeginSceneTransition(string targetScene, string entryGate)
    {
        if (isTransitioning) return;

        Player.GetComponent<PlayerView>().PlayerModel.SetCurrentScene(targetScene);
        Debug.Log($"Переход на сцену {targetScene}");
        SetGameState(GameState.ENTERING_LEVEL);
        //Вызов сохранения
        StartCoroutine(DoSceneTransition(targetScene, entryGate));
    }

    private IEnumerator DoSceneTransition(string targetScene, string entryGate)
    {
        isTransitioning = true;
        TransitionFade._instance.FadeOut();
        AudioListener.pause = true;

        yield return new WaitForSeconds(1f);

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetScene);
        while (!loadOp.isDone)
            yield return null;

        destination = FindEntryPoint(entryGate);
        _playerCached = null;
        var currentPlayer = Player;

        if (destination == null)
        {
            Debug.Log($"Нет точки назначения в сцене {targetScene}");
            yield return null;
        }
        if (currentPlayer == null)
        {
            Debug.Log($"Игрок не найден в сцене в сцене {targetScene}");
            yield return null;
        }
        currentPlayer.transform.position = destination.transform.position;

        //yield return screenFader.FadeIn();
        isTransitioning = false;
        SetGameState(GameState.EXITING_LEVEL);

        TransitionFade._instance.FadeIn();
        AudioListener.pause = false;
    }

    private TransitionDestination FindEntryPoint(string tag)
    {
        destinations = GameObject.FindObjectsByType<TransitionDestination>(FindObjectsSortMode.None);
        foreach (var destinationPoint in destinations)
        {
            if (destinationPoint.destinationTag == tag)
                return destinationPoint;
        }
        Debug.Log("Destination failed: Точка назначения не найдена ");
        return null;
    }

    public void InitializeComponent()
    {
        /*BossHealth.*/
    }

    public void SetGameState(GameState newState)
    {
        GameState = newState;
    }

    public void RevivalPlayer()
    {
        if (GameState == GameState.DEAD)
        {
            Destroy(Player);
            SceneManager.LoadSceneAsync("F_Room_Tutorial"); // Загружать сцены из сохранения
            EntryPoint._instance.InitializeDataFromSave();
            SetGameState(GameState.PLAYING);
        }
    }

    #region Menu

    public void OpenSavesMenu()
    {
        SceneManager.LoadScene(savesMenuSceneName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void StartGameFromProfile(int profileNumber)
    {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        SceneManager.LoadScene(entryPointSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Игра закрылась");
        Application.Quit();
    }

    #endregion
}
