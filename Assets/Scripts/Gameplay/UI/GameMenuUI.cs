using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    private static GameMenuUI instance;
    public static GameMenuUI Instance { get { return instance; } }

    [SerializeField] private Toggle _mapButton;
    [SerializeField] private Toggle _abilityButton;

    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _normalColor;

    //[Header("Select Color")]
    //[SerializeField] private Color _selectColor;
    //[SerializeField] private Color _unSelectColor;

    private List<Toggle> _windowButtons = new ();

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

        _mapButton.onValueChanged.AddListener((value) =>
        {
            if (_mapButton.isOn)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.OpenMap();
            }
        });
        _abilityButton.onValueChanged.AddListener((value) =>
        {
            if (_abilityButton.isOn)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.OpenAbilitiesTree();
            }
        });
    }

    private void Start()
    {
        _windowButtons.Add(_mapButton);
        _windowButtons.Add(_abilityButton);
    }

    public void SwapToRightWindow()
    {
        //_windowButtons[_windowNumber].gameObject.GetComponent<Image>().color = _normalColor;
        _windowNumber++;

        if (_windowNumber >= _windowButtons.Count)
        {
            _windowNumber = 0;
        }

        EventSystem.current.SetSelectedGameObject(_windowButtons[_windowNumber].gameObject);
        //_windowButtons[_windowNumber].gameObject.GetComponent<Image>().color = _selectedColor;

        //ColorBlock colorBlock1 = _windowButtons[_windowNumber - 1].colors;
        //colorBlock1.normalColor = _unSelectColor;
        //_windowButtons[_windowNumber].colors = colorBlock1;

        //ColorBlock colorBlock2 = _windowButtons[_windowNumber].colors;
        //colorBlock2.normalColor = _selectColor;
        //_windowButtons[_windowNumber].colors = colorBlock2;

        _windowButtons[_windowNumber].isOn = true;
    }

    public void SwapToLeftWindow()
    {
        //_windowButtons[_windowNumber].gameObject.GetComponent<Image>().color = _normalColor;
        _windowNumber--;

        if (_windowNumber < 0)
        {
            _windowNumber = _windowButtons.Count - 1;
        }

        EventSystem.current.SetSelectedGameObject(_windowButtons[_windowNumber].gameObject);
        //_windowButtons[_windowNumber].gameObject.GetComponent<Image>().color = _selectedColor;

        //ColorBlock colorBlock1 = _windowButtons[_windowNumber + 1].colors;
        //colorBlock1.normalColor = _unSelectColor;
        //_windowButtons[_windowNumber].colors = colorBlock1;

        //ColorBlock colorBlock2 = _windowButtons[_windowNumber].colors;
        //colorBlock2.normalColor = _selectColor;
        //_windowButtons[_windowNumber].colors = colorBlock2;

        _windowButtons[_windowNumber].isOn = true;
    }

    void OnEnable()
    {
        if (_mapButton == null)
        {
            Debug.Log("Map Button is NULL!");
            return;
        }

        EventSystem.current.SetSelectedGameObject(_mapButton.gameObject);
        _mapButton.isOn = true;
    }
}
