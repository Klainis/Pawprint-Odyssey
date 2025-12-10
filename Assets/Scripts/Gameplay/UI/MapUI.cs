using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [SerializeField] private Button closeMapButton;

    private void Start()
    {
        closeMapButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.CloseMap();
        });
    }
}
