using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [Header("Distanation Scene")]
    [SerializeField] private string targetScene;
    [SerializeField] private string entryGate;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            gameManager._instance.BeginSceneTransition(targetScene, entryGate);
    }
}
