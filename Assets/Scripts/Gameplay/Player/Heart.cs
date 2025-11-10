using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private PlayerData Data;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform canvas;
    public GameObject hearts;
    private List<GameObject> heartsList = new List<GameObject>();

    private void Start()
    {
        StartHearts();
    }

    public void StartHearts()
    {
        while (heartsList.Count < Data.maxLife)
        {
            foreach (Transform heart in hearts.transform)
            {
                heartsList.Add(heart.gameObject);
            }
        }
        Debug.Log(heartsList.Count);

        //if (heartsList.Count != Data.maxLife)
        //{
        //    for (int i = heartsList.Count - 1; i >= 0; i--)
        //    {
        //        if (i > (Data.currentLife - 1))
        //        {
        //            Destroy(heartsList[i]);
        //            heartsList.RemoveAt(i);
        //        }
        //    }
        //}
    }

    public void RemoveHearts(int damage)
    {
        Data.currentLife -= damage;
        if (Data.currentLife < 1)
            Data.isDead = true;

        for (int i = heartsList.Count - 1; i >= 0; i--)
        {
            if (i > (Data.currentLife - 1))
            {
                Destroy(heartsList[i]);
                heartsList.RemoveAt(i);
            }
        }

    }

    public void Heal()
    {
        int missingHearts = Data.maxLife - heartsList.Count;

        for (int i = 0; i < missingHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, canvas);
            heartsList.Add(heart);
        }

        Data.currentLife = Data.maxLife;
    }
}
