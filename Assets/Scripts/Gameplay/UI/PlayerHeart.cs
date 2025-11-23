using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHeart : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject heartsPrefab;
    [SerializeField] private GameObject heartPrefab;

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
        var cnt = 1;
        while (heartsList.Count < playerView.PlayerModel.Life)
        {
            var heart = Instantiate(heartPrefab, new Vector3(25 * cnt, -51, 0), new Quaternion(), heartsPrefab.transform);
            heartsList.Add(heart);
            cnt++;
        }
    }

    public void RemoveHearts()
    {
        while (heartsList.Count > playerView.PlayerModel.Life)
        {
            Destroy(heartsList.Last());
            heartsList.Remove(heartsList.Last());
        }
    }

    public void AddHearts()
    {
        var cnt = 1;
        while (heartsList.Count < playerView.PlayerModel.Life)
        {
            var heart = Instantiate(heartPrefab, new Vector3(25 * cnt, -51, 0), new Quaternion(), heartsPrefab.transform);
            heartsList.Add(heart);
            cnt++;
        }
    }
}
