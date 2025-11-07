using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    public int life { get; private set; }
    private int maxLife;

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private List<GameObject> hearts;

    private void Awake()
    {
        Debug.Log($"[Heart Awake] isDead={Data.isDead}");
        //if (Data.isDead)
        //{
        //    Dead();
        //}

        life = Data.currentLife;
        maxLife = Data.maxLife;

        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (i > (life - 1))
            {
                Destroy(hearts[i]);
                hearts.RemoveAt(i);
            }
        }
    }

    void FixedUpdate()
    {
        Data.currentLife = life;
    }

    public void RemoveHearts(int damage)
    {
        life -= damage;

        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (i > (life - 1))
            {
                Destroy(hearts[i]);
                hearts.RemoveAt(i);
            }
        }
    }

    public void Heal()
    {
        int missingHearts = maxLife - hearts.Count;

        for (int i = 0; i < missingHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, canvas);
            hearts.Add(heart);
        }

        life = maxLife;
    }

    //private void Dead()
    //{
    //    Data.currentLife = Data.maxLife;
    //    Data.isDead = false;
    //}
}
