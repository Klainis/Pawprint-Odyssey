using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsMenuUI : MonoBehaviour
{
    [SerializeField] private Button gamepadControls;
    [SerializeField] private Button keyboardControls;
    [SerializeField] private Button backButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);

        gamepadControls.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetMenu(GameManager.MenuState.GamepadControls);
        });

        keyboardControls.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetMenu(GameManager.MenuState.KeyboardControls);
        });

        backButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetMenu(GameManager.MenuState.Options);
        });
    }
}
