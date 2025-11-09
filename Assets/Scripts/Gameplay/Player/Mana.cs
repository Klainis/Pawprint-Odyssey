using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    //private int maxMana;
    //private int mana;

    [SerializeField] private Image manaBar;
    private EntryPoint m_EntryPoint;
    public void Awake()
    {
        m_EntryPoint = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
        manaBar = m_EntryPoint.manaBarImage;
    }
    private void Start()
    {
        
        manaBar.fillAmount = (float)Data.currentMana / (float)Data.maxMana;
    }

    public void SpendMana()
    {
        Data.currentMana -= 25;
        if (Data.currentMana <= 0)
            Data.currentMana = 0;

        manaBar.fillAmount = (float)Data.currentMana / (float)Data.maxMana;
    }

    public void GetMana()
    {
        Data.currentMana += 4;
        if (Data.currentMana > Data.maxMana)
            Data.currentMana = Data.maxMana;

        manaBar.fillAmount = (float)Data.currentMana / (float)Data.maxMana;
    }
}
