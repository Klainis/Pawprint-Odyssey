using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class InitializeManager : MonoBehaviour
{
    public static InitializeManager _instance { get; private set; }

    public GameObject player;
    public Canvas canvas;


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
}
