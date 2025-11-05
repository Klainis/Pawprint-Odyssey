using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private int life = 5;
    [SerializeField] private int maxLife = 5;
    public int lifeForReading {  get; private set; }
    private int lifeCounter;

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private List<GameObject> hearts;

    void Start()
    {
        lifeCounter = life;
    }

    void FixedUpdate()
    {
        lifeForReading = life; 
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
                Debug.Log("HEAL");
                GameObject heart = Instantiate(heartPrefab, canvas);
                hearts.Add(heart);
            }
        }

        life = maxLife;
    }
}
