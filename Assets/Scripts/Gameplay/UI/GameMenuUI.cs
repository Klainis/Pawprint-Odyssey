using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    private static GameMenuUI instance;
    public static GameMenuUI Instance { get { return instance; } }

    [SerializeField] private Button _mapButton;
    [SerializeField] private Button _abilityButton;

    private List<Button> _windowButtons = new ();

    private int _windowNumber = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (_mapButton == null)
        {
            Debug.Log("Map Button is NULL!");
            return;
        }
        if (_abilityButton == null)
        {
            Debug.Log("Ability Button is NULL!");
            return;
        }

        _mapButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OpenMap();
        });
        _abilityButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OpenAbilitiesTree();
        });
    }

    private void Start()
    {
        _windowButtons.Add(_mapButton);
        _windowButtons.Add(_abilityButton);
    }

    public void SwapToRightWindow()
    {
        _windowNumber++;

        if (_windowNumber >= _windowButtons.Count)
        {
            _windowNumber = 0;
        }

        EventSystem.current.SetSelectedGameObject(_windowButtons[_windowNumber].gameObject);
        _windowButtons[_windowNumber].onClick.Invoke();
    }

    public void SwapToLeftWindow()
    {
        _windowNumber--;

        if (_windowNumber <= _windowButtons.Count)
        {
            _windowNumber = _windowButtons.Count - 1;
        }

        EventSystem.current.SetSelectedGameObject(_windowButtons[_windowNumber].gameObject);
        _windowButtons[_windowNumber].onClick.Invoke();
    }

    void OnEnable()
    {
        if (_mapButton == null)
        {
            Debug.Log("Map Button is NULL!");
            return;
        }

        EventSystem.current.SetSelectedGameObject(_mapButton.gameObject);
        _mapButton.onClick.Invoke();
    }
}
