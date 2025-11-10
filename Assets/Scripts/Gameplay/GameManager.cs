using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameManager _instance { get; private set; }

    //[SerializeField] private ScreenFader screenFader;

    private TransitionDestination destination;
    private TransitionDestination[] destinations;
    private GameObject player;
    private BossHealth bossHealth;

    private bool isTransitioning;

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

    public void BeginSceneTransition(string targetScene, string entryGate)
    {
        if (isTransitioning)
            return;
        StartCoroutine(DoSceneTransition(targetScene, entryGate));
    }

    private IEnumerator DoSceneTransition(string targetScene, string entryGate)
    {
        isTransitioning = true;
        //yield return screenFader.FadeOut();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetScene);
        while (!loadOp.isDone)
            yield return null;



        destination = FindEntryPoint(entryGate);
        player = GameObject.FindGameObjectWithTag("Player");

        if (destination == null)
        {
            Debug.Log($"Нет точки назначения в сцене {targetScene}");
            yield return null;
        }
        if (player == null)
        {
            Debug.Log($"Игрок не найден в сцене в сцене {targetScene}");
            yield return null;
        }
        player.transform.position = destination.transform.position;

        //yield return screenFader.FadeIn();
        isTransitioning = false;
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
        /*ossHealth.*/
    }
}
