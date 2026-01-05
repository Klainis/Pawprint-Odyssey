using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetButtonSelected : MonoBehaviour
{
    [SerializeField] private Button _firstButton;

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(_firstButton.gameObject);
    }
}
