using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private PlayerData Data;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private List<GameObject> hearts;

    private void Start()
    {
        StartHearts();
    }

    public void StartHearts()
    {
        if (hearts.Count != Data.maxLife)
        {
            for (int i = hearts.Count - 1; i >= 0; i--)
            {
                if (i > (Data.currentLife - 1))
                {
                    Destroy(hearts[i]);
                    hearts.RemoveAt(i);
                }
            }
        }
    }

    public void RemoveHearts(int damage)
    {
        Data.currentLife -= damage;
        if (Data.currentLife < 1)
            Data.isDead = true;

        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (i > (Data.currentLife - 1))
            {
                Destroy(hearts[i]);
                hearts.RemoveAt(i);
            }
        }

    }

    public void Heal()
    {
        int missingHearts = Data.maxLife - hearts.Count;

        for (int i = 0; i < missingHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, canvas);
            hearts.Add(heart);
        }

        Data.currentLife = Data.maxLife;
    }
}
