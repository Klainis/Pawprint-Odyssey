using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    private int maxMana;
    private int mana;

    [SerializeField] private Image manaBar;

    private void Start()
    {
        mana = Data.currentMana;
        maxMana = Data.maxMana;
        manaBar.fillAmount = (float)mana / (float)maxMana;
    }

    public void SpendMana()
    {
        mana -= 25;
        if (mana <= 0)
            mana = 0;

        Data.currentMana = mana;

        manaBar.fillAmount = (float)mana / (float)maxMana;
    }

    public void GetMana()
    {
        mana += 4;
        if (mana > maxMana)
            mana = maxMana;

        Data.currentMana = mana;

        manaBar.fillAmount = (float)mana / (float)maxMana;
    }
}
