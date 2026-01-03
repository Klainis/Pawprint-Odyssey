using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);

        continueButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ClosePauseMenu();
        });

        optionsButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OpenOptionsMenu();
        });

        quitButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.QuitToMainMenu();
        });
    }
}
