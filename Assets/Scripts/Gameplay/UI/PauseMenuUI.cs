using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        continueButton.onClick.AddListener(() =>
        {
            if (GameManager._instance != null)
                GameManager._instance.ClosePauseMenu();
        });

        quitButton.onClick.AddListener(() =>
        {
            if (GameManager._instance != null)
                GameManager._instance.QuitToMainMenu();
        });
    }
}
