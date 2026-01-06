using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private EventSystem _eventSystem;

    private void Start()
    {
        _eventSystem = EventSystem.current;

        continueButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetMenu(GameManager.MenuState.None);
        });

        optionsButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetMenu(GameManager.MenuState.Options);
        });

        quitButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.QuitToMainMenu();
        });
    }

    private void OnEnable()
    {
        _eventSystem.SetSelectedGameObject(continueButton.gameObject);
    }
}
