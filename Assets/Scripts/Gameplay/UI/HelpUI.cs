using UnityEngine;

public class HelpUI : MonoBehaviour
{
    [SerializeField] private ShowEducation _showEducation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _showEducation.gameObject.SetActive(true);
            _showEducation.FadeIn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_showEducation.gameObject.activeSelf) _showEducation.FadeOut();
        }
    }
}
