using UnityEngine;

public class EndingScene : MonoBehaviour
{
    public void LoadMainMenu()
    {
        GameManager.Instance.QuitToMainMenu();
    }
}
