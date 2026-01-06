using UnityEngine;

public class EducationCheck : MonoBehaviour
{
    [SerializeField] private ShowEducation _showEducation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _showEducation.FadeIn();
            gameObject.SetActive(false);
        }
    }
}
