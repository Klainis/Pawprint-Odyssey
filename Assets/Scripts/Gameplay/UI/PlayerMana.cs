using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    private PlayerView playerView;
    private EntryPoint entryPoint;
    private Image manaBar;

    private readonly int manaToSpend = 25;
    private readonly int manaToGet = 4;

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
        entryPoint = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
        manaBar = entryPoint.manaBarImage;
    }

    private void Start()
    {
        manaBar.fillAmount = (float)playerView.PlayerModel.Mana / playerView.PlayerModel.MaxMana;
    }

    public void SetManaBarImage(Image img)
    {
        manaBar = img;
    }

    public void SpendMana()
    {
        playerView.PlayerModel.ChangeAmountOfMana(playerView.PlayerModel.Mana - manaToSpend);
        manaBar.fillAmount = (float)playerView.PlayerModel.Mana / playerView.PlayerModel.MaxMana;
    }

    public void GetMana()
    {
        playerView.PlayerModel.ChangeAmountOfMana(playerView.PlayerModel.Mana + manaToGet);
        manaBar.fillAmount = (float)playerView.PlayerModel.Mana / playerView.PlayerModel.MaxMana;
    }
}
