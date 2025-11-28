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
            var heart = Instantiate(heartPrefab, heartsInstance.transform.GetChild(0).transform);
            heart.transform.localPosition = new Vector3(8 + 55 * cnt, -43, 0);
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
        var cnt = heartsList.Count + 1;
        while (heartsList.Count < playerView.PlayerModel.Life)
        {
            var heart = Instantiate(heartPrefab, heartsInstance.transform.GetChild(0).transform);
            heart.transform.localPosition = new Vector3(8 + 55 * cnt, -43, 0);
            heartsList.Add(heart);
            cnt++;
        }
    }
}
