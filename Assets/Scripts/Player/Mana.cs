using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    private int maxMana;
    private int mana;
    private int manaAfterDead;

    [SerializeField] private Image manaBar;

    private void Awake()
    {
        //manaAfterDead = Data.manaAfterDead;

        if (Data.isDead)
        {
            Dead();
        }

        mana = Data.currentMana;
        maxMana = Data.maxMana;
        manaBar.fillAmount = (float)mana / (float)maxMana;
    }
    
    private void FixedUpdate()
    {
        //if (mana > maxMana)
        //    mana = maxMana;
        //if (mana <= 0)
        //    mana = 0;

        //Data.currentMana = mana;
    }

    public void SpendMana()
    {
        mana -= 25;
        manaBar.fillAmount = (float)mana / (float)maxMana;
    }

    public void GetMana()
    {
        mana += 4;
        manaBar.fillAmount = (float)mana / (float)maxMana;
    }

    private void Dead()
    {
        Data.currentMana = Data.manaAfterDead;
        //mana = manaAfterDead;
        Data.isDead = false;
    }
}
