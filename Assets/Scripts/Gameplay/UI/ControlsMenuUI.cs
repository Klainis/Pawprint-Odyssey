using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsMenuUI : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);

        backButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.CloseControlsMenu();
        });
    }
}
