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
        if (Data.isDead)
        {
            Dead();
        }

        life = Data.currentLife;
        maxLife = Data.maxLife;
    }

    void FixedUpdate()
    {
        Data.currentLife = life;
    }

    public void RemoveHearts(int damage)
    {
        life -= damage;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i > life-1)
            {
                Destroy(hearts[i]);
                hearts.Remove(hearts[i]);
            }
        }
    }

    public void Heal()
    {
        for (int i = 0; i < (maxLife); i++)
        {
            if (i > life-1)
            {
                GameObject heart = Instantiate(heartPrefab, canvas);
                hearts.Add(heart);
            }
        }

        life = maxLife;
    }

    private void Dead()
    {
        Data.currentLife = Data.maxLife;
        Data.isDead = false;
    }
}
