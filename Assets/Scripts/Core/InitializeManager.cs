using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;

public class InitializeManager : MonoBehaviour
{
    private static InitializeManager instance;
    public static InitializeManager Instance { get { return instance; } }

    public GameObject player;
    public Canvas canvas;
    public TMP_Text soulCrystalText;
    public GameObject bossHealth;
    public GameObject manaBar;

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
}
