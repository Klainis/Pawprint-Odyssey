using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHeart : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;

    private PlayerView playerView;
    
    private GameObject heartsInstance;

    private List<GameObject> heartsList = new();

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
    }

    public void SetHeartsInstance(GameObject prefab)
    {
        heartsInstance = prefab;
    }

    public void StartHearts()
    {
        var cnt = 1;
        while (heartsList.Count < playerView.PlayerModel.Life)
        {
            var heart = Instantiate(heartPrefab, new Vector3(-7 + 0.8f * cnt, 7, 0), Quaternion.identity, heartsInstance.transform.GetChild(0).transform);
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
            var heart = Instantiate(heartPrefab, new Vector3(-7 + 0.8f * cnt, 7, 0), Quaternion.identity, heartsInstance.transform.GetChild(0).transform);
            heartsList.Add(heart);
            cnt++;
        }
    }
}
