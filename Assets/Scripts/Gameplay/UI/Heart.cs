using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private PlayerData Data;
    [SerializeField] public GameObject heartPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject heartsPrefab;

    private PlayerView playerView;

    private List<GameObject> heartsList = new();

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
    }

    public void RemoveHearts(int damage)
    {
        playerView.PlayerModel.TakeDamage(damage);

        while (heartsList.Count > playerView.PlayerModel.Life)
        {
            Destroy(heartsList.Last());
            heartsList.Remove(heartsList.Last());
        }
    }

    public void AddHearts()
    {
        while (heartsList.Count < playerView.PlayerModel.MaxLife)
        {
            var heart = Instantiate(heartPrefab, canvas);
            heartsList.Add(heart);
            playerView.PlayerModel.Heal(1);
        }
    }
}
