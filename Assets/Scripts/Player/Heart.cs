using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private PlayerData Data;

    private int life;
    private int maxLife;
    public int lifeForReading {  get; private set; }

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private List<GameObject> hearts;

    private void Start()
    {
        life = Data.currentLife;
        maxLife = Data.maxLife;
    }

    void FixedUpdate()
    {
        lifeForReading = life;

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

    public void Dead()
    {
        life = maxLife;
    }
}
