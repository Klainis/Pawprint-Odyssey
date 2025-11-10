using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class InitializeManager : MonoBehaviour
{
    public static InitializeManager _instance { get; private set; }

    [HideInInspector] public GameObject player;

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
