using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    private int maxMana;
    private int mana;
    public int manaForReading { get; private set; }

    [SerializeField] private Image manaBar;

    private void Start()
    {
        mana = Data.currentMana;
        maxMana = Data.maxMana;
    }
    
    private void FixedUpdate()
    {
        if (mana > maxMana)
            mana = maxMana;
        if (mana <= 0)
            mana = 0;

        manaForReading = mana;
        Data.currentMana = mana;
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
}
