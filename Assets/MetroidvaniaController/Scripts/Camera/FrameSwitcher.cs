using UnityEngine;

public class FrameSwitcher : MonoBehaviour
{
    public GameObject activeFrame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (activeFrame != null)
                activeFrame.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (activeFrame != null)
                activeFrame.SetActive(false);
        }
    }
}
