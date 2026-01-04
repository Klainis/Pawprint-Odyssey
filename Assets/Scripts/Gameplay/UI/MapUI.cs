using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [SerializeField] private Button closeMapButton;

    //[Header("Currency Text")]
    //[SerializeField] private TMP_Text _crystalText;

    private void Start()
    {
        closeMapButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.CloseMap();
        });
    }

    //private void OnEnable()
    //{
    //    _crystalText.text = GameMenuUI.Instance.MapCrystalText.text;
    //}
}
