using GlobalEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GlobalEnums;
using Unity.AppUI.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    public GameState GameState { get; private set; }

    private string _currentScene;

    public string currentScene { get { return _currentScene; } set { _currentScene = value; } }

    private TransitionDestination destination;
    private TransitionDestination[] destinations;
    private AudioListener Sounds;
    private BossHealth bossHealth;
    private GameObject _playerCached;
    private GameObject _pauseMenuCanvasInstance;
    private GameObject _optionsMenuCanvasInstance;
    private GameObject _controlsMenuCanvasInstance;
    private GameObject _controlsGamepadMenuCanvasInstance;
    private GameObject _controlsKeyboardMenuCanvasInstance;
    private GameObject _mapCanvasInstance;
    private GameObject _gameMenuCanvasInstance;
    private GameObject _abilitiesTreeCanvasInstance;


    private readonly string mainMenuSceneName = "MainMenu";
    private readonly string savesMenuSceneName = "SavesMenu";
    private readonly string entryPointSceneName = "EntryPoint";

    private bool isTransitioning;
    private bool mapOpened = false;
    private bool inPauseMenu = false;
    private bool inGameMenu = false;
    private bool gamePaused = false;

    public bool InPauseMenu { get { return inPauseMenu; } }
    public bool InGameMenu { get { return inGameMenu; } }

    public enum MenuState { None, Pause, Options, Controls, GamepadControls, KeyboardControls }

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
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SetGameState(GameState.PLAYING);

        Sounds = GameObject.FindAnyObjectByType<AudioListener>();
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
            EntryPoint.Instance.InitializeDataFromSave();
            EntryPoint.Instance.InitializePlayerUI();
            SceneManager.LoadSceneAsync(PlayerView.Instance.PlayerModel.CurrentScene);
            SetGameState(GameState.PLAYING);
        }
    }

    #region Transitions

    public void BeginSceneTransition(string targetScene, string entryGate)
    {
        if (isTransitioning) return;

        _currentScene = targetScene;
        //Player.GetComponent<PlayerView>().PlayerModel.SetCurrentScene(targetScene); // ��������� ����� ������ ��� ����������, � �� ��� ��������!!!
        Debug.Log($"GameManager: ������� �� ����� {targetScene}");
        SetGameState(GameState.ENTERING_LEVEL);
        //����� ����������
        StartCoroutine(DoSceneTransition(targetScene, entryGate));
    }

    private IEnumerator DoSceneTransition(string targetScene, string entryGate)
    {
        isTransitioning = true;
        TransitionFade._instance.FadeOut();
        AudioListener.pause = true;

        yield return new WaitForSeconds(1f);

        MapManager.Instance.SetMapIcon(targetScene);
        MapManager.Instance.OpenRoom(targetScene);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetScene);
        while (!loadOp.isDone)
            yield return null;

        destination = FindRoomsEntryPoint(entryGate);
        _playerCached = null;
        var currentPlayer = Player;

        if (destination == null)
        {
            Debug.Log($"GameManager: ��� ����� ���������� � ����� {targetScene}");
            yield return null;
        }
        if (currentPlayer == null)
        {
            Debug.Log($"GameManager: ����� �� ������ � ����� {targetScene}");
            yield return null;
        }
        currentPlayer.transform.position = destination.transform.position;

        //yield return screenFader.FadeIn();
        isTransitioning = false;
        SetGameState(GameState.EXITING_LEVEL);

        TransitionFade._instance.FadeIn();
        AudioListener.pause = false;
    }

    private TransitionDestination FindRoomsEntryPoint(string tag)
    {
        destinations = GameObject.FindObjectsByType<TransitionDestination>(FindObjectsSortMode.None);
        foreach (var destinationPoint in destinations)
        {
            if (destinationPoint.destinationTag == tag)
                return destinationPoint;
        }
        Debug.Log("GameManager: ����� ���������� �� ������� ");
        return null;
    }

    #endregion

    #region Menu

    public void OpenSavesMenu()
    {
        SceneManager.LoadScene(savesMenuSceneName);
    }

    public void FromSavesMenuToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;

        // SaveSystem.Save();

        if (EntryPoint.Instance != null)
            EntryPoint.Instance.DestroyAllSessionObjects();

        SceneManager.LoadScene(mainMenuSceneName);
        Destroy(gameObject);
    }

    public void StartGameFromProfile(int profileNumber)
    {
        SaveSystem.CurrentProfileIndex = profileNumber;
        Debug.Log($"GameManager: ������ ������� ����������: {profileNumber}");

        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        SceneManager.LoadScene(entryPointSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("GameManager: ���� ���������");
        Application.Quit();
    }

    private void OpenPauseMenu()
    {
        if (inGameMenu)
            return;

        inPauseMenu = true;
        SetGameState(GameState.PAUSE_MENU);
        PauseGame();
    }

    private void ClosePauseMenu()
    {
        if (inGameMenu)
            return;

        inPauseMenu = false;
        UnpauseGame();
        SetGameState(GameState.PLAYING);
    }

    public void SetMenu(MenuState newState)
    {
        Debug.Log(newState);

        if (newState == MenuState.Pause)
        {
            _pauseMenuCanvasInstance.SetActive(true);
            OpenPauseMenu();
        }
        if (newState == MenuState.None)
        {
            _pauseMenuCanvasInstance.SetActive(false);
            ClosePauseMenu();
        }

        _pauseMenuCanvasInstance.SetActive(newState == MenuState.Pause);
        _optionsMenuCanvasInstance.SetActive(newState == MenuState.Options);
        _controlsMenuCanvasInstance.SetActive(newState == MenuState.Controls);
        _controlsGamepadMenuCanvasInstance.SetActive(newState == MenuState.GamepadControls);
        _controlsKeyboardMenuCanvasInstance.SetActive(newState == MenuState.KeyboardControls);
    }

    public void SetPauseMenuCanvasInstance(GameObject obj)
    {
        _pauseMenuCanvasInstance = obj;
    }

    public void SetOptionsMenuCanvasInstance(GameObject obj)
    {
        _optionsMenuCanvasInstance = obj;
    }

    public void SetControlsMenuCanvasInstance(GameObject obj)
    {
        _controlsMenuCanvasInstance = obj;
    }

    public void SetControlsGamepadMenuCanvasInstance(GameObject obj)
    {
        _controlsGamepadMenuCanvasInstance = obj;
    }

    public void SetControlsKeyboardMenuCanvasInstance(GameObject obj)
    {
        _controlsKeyboardMenuCanvasInstance = obj;
    }

    #endregion

    #region Pause & Unpause

    private void PauseGame()
    {
        if (gamePaused)
            return;

        gamePaused = true;
        //SetGameState(GameState.PAUSED);

        Time.timeScale = 0;

        var inputSystem = EntryPoint.Instance.NewInputSystem;
        if (inputSystem != null)
        {
            var playerMap = inputSystem.FindActionMap("Player");
            if (playerMap != null)
                playerMap.Disable();
        }
    }

    private void UnpauseGame()
    {
        if (!gamePaused)
            return;

        gamePaused = false;
        //SetGameState(GameState.PLAYING);

        Time.timeScale = 1;

        var inputSystem = EntryPoint.Instance.NewInputSystem;
        if (inputSystem != null)
        {
            var playerMap = inputSystem.FindActionMap("Player");
            if (playerMap != null)
                playerMap.Enable();
        }
    }

    #endregion

    #region Map

    public void OpenMap()
    {
        CloseAbilitiesTree();
        _mapCanvasInstance.SetActive(true);
    }

    public void CloseMap()
    {
        _mapCanvasInstance.SetActive(false);
    }

    public void SetMapCanvasInstance(GameObject obj)
    {
        _mapCanvasInstance = obj;
    }

    #endregion

    #region Abilities Tree

    public void OpenAbilitiesTree()
    {
        CloseMap();
        _abilitiesTreeCanvasInstance.SetActive(true);
        _abilitiesTreeCanvasInstance.GetComponent<AbilitiesTreeUIManager>().SetSelectedAfterOpenWindow();
    }

    public void CloseAbilitiesTree()
    {
        _abilitiesTreeCanvasInstance.SetActive(false);
    }

    public void SetAbilitiesTreeCanvasInstance(GameObject obj)
    {
        _abilitiesTreeCanvasInstance = obj;
    }

    #endregion

    #region GameMenu

    public void OpenGameMenu()
    {
        if (inPauseMenu)
            return;

        inGameMenu = true;
        SetGameState(GameState.GAME_MENU);
        PauseGame();
        _gameMenuCanvasInstance.SetActive(true);
    }

    public void CloseGameMenu()
    {
        if (inPauseMenu)
            return;

        inGameMenu = false;
        _gameMenuCanvasInstance.SetActive(false);
        CloseMap();
        CloseAbilitiesTree();
        UnpauseGame();
        SetGameState(GameState.PLAYING);
    }

    public void SetGameMenuCanvasInstance(GameObject obj)
    {
        _gameMenuCanvasInstance = obj;
    }

    #endregion
}
