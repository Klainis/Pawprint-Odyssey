using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private int life = 5;
    public int lifeForReading {  get; private set; }
    private int lifeCounter;

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
            if (i > life)
            {
                Destroy(hearts[i]);
            }
        }
    }
}
