using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button controlsButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);

        backButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.CloseOptionsMenu();
        });

        controlsButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OpenControlsMenu();
        });
    }
}
