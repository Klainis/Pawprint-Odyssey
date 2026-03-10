using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    private static PlayerMana instance;
    public static PlayerMana Instance { get; set; }

    private PlayerView playerView;
    private EntryPoint entryPoint;
    private Image manaBar;

    private readonly int manaToSpend = 25;
    private readonly int manaToSpendDamageDash = 10;
    private readonly int manaToGet = 4;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        playerView = GetComponent<PlayerView>();
    }

    private void Start()
    {
        manaBar.fillAmount = (float)playerView.PlayerModel.Mana / playerView.PlayerModel.MaxMana;
    }

    public void SetManaBarImage(Image img)
    {
        manaBar = img;
    }

    public void SpendMana(string ability)
    {
        switch (ability)
        {
            case "Claw":
                playerView.PlayerModel.SetMana(playerView.PlayerModel.Mana - manaToSpend);
                manaBar.fillAmount = (float)playerView.PlayerModel.Mana / playerView.PlayerModel.MaxMana;
                break;
            case "DamageDash":
                playerView.PlayerModel.SetMana(playerView.PlayerModel.Mana - manaToSpendDamageDash);
                manaBar.fillAmount = (float)playerView.PlayerModel.Mana / playerView.PlayerModel.MaxMana;
                break;
        }
    }

    public void GetMana()
    {
        playerView.PlayerModel.SetMana(playerView.PlayerModel.Mana + manaToGet);
        manaBar.fillAmount = (float)playerView.PlayerModel.Mana / playerView.PlayerModel.MaxMana;
    }

    public void FullMana()
    {
        playerView.PlayerModel.FullMana();
        manaBar.fillAmount = (float)playerView.PlayerModel.Mana / playerView.PlayerModel.MaxMana;
    }
}
