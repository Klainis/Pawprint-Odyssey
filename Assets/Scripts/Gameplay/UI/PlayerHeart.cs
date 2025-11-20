using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHeart : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject heartsPrefab;

    private PlayerView playerView;

    private List<GameObject> heartsList = new();

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
    }

    public void SetHeartsPrefab(GameObject prefab)
    {
        heartsPrefab = prefab;
    }

    public void StartHearts()
    {
        while (heartsList.Count < playerView.PlayerModel.MaxLife)
        {
            foreach (Transform heart in heartsPrefab.transform)
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
            var heart = Instantiate(heartsPrefab, canvas);
            heartsList.Add(heart);
            playerView.PlayerModel.Heal(1);
        }
    }
}
