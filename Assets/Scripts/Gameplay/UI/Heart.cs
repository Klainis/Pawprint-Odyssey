using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private PlayerData Data;
    [SerializeField] public GameObject heartPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject heartsPrefab;

    private PlayerView playerView;

    private List<GameObject> heartsList = new List<GameObject>();

    private void Start()
    {
        playerView = GetComponent<PlayerView>();

        StartHearts();
    }

    public void StartHearts()
    {
        while (heartsList.Count < Data.maxLife)
        {
            foreach (Transform heart in heartPrefab.transform)
                heartsList.Add(heart.gameObject);
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
        playerView.PlayerModel.TakeDamage(damage);

        for (var i = heartsList.Count - 1; i >= 0; i--)
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
        var missingHearts = Data.maxLife - heartsList.Count;

        for (var i = 0; i < missingHearts; i++)
        {
            var heart = Instantiate(heartPrefab, canvas);
            heartsList.Add(heart);
        }

        playerView.PlayerModel.ChangeLife(playerView.PlayerModel.MaxLife);
    }
}
