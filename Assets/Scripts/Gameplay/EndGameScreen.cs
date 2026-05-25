using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScreen : MonoBehaviour
{
    public void StartEndingScene()
    {
        EndGameManager.Instance.LoadEndingScene();
    }
}
