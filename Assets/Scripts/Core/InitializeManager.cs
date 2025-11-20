using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;

public class InitializeManager : MonoBehaviour
{
    public static InitializeManager _instance { get; private set; }

    public GameObject player;
    public Canvas canvas;
    public TMP_Text soulCrystalText;
    public GameObject bossHealth;
    public GameObject manaBar;


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
