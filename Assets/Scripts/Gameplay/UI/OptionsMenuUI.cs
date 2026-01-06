using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour
{
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button backButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);

        controlsButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetMenu(GameManager.MenuState.Controls);
        });

        backButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetMenu(GameManager.MenuState.Pause);
        });
    }
}
