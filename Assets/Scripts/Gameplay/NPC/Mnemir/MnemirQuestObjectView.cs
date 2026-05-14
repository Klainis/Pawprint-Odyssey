using UnityEngine;
using UnityEngine.SceneManagement;

public class MnemirQuestObjectView : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);

        CheckIfCollected();
    }

    private void FixedUpdate()
    {
        if (PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects.Contains(SceneManager.GetActiveScene().name))
            Destroy(gameObject);
    }

    private void CheckIfCollected()
    {
        if (PlayerView.Instance.PlayerModel.HasQuestMnemir)
        {
            if (!PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects.Contains(SceneManager.GetActiveScene().name))
            {
                gameObject.SetActive(true);
            }
        }
    }
}
