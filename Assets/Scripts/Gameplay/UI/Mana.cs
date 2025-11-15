using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    //private int maxMana;
    //private int mana;

    private PlayerView playerView;
    private EntryPoint entryPoint;
    public Image manaBar;

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
        entryPoint = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
        manaBar = entryPoint.manaBarImage;
    }

    private void Start()
    {
        manaBar.fillAmount = (float)Data.currentMana / (float)Data.maxMana;
    }

    public void SpendMana()
    {
        playerView.PlayerModel.ChangeAmountOfMana(playerView.PlayerModel.Mana - 25);
        Data.currentMana = playerView.PlayerModel.Mana;
        manaBar.fillAmount = (float)Data.currentMana / (float)Data.maxMana;
    }

    public void GetMana()
    {
        playerView.PlayerModel.ChangeAmountOfMana(playerView.PlayerModel.Mana + 4);
        Data.currentMana = playerView.PlayerModel.Mana;
        manaBar.fillAmount = (float)Data.currentMana / (float)Data.maxMana;
    }
}
