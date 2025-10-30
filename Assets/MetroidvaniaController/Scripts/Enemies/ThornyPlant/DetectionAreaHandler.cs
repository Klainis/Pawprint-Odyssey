using System.Collections;
using UnityEngine;

public class DetectionAreaHandler : MonoBehaviour
{
    private ThornyPlant thornyPlant;

    void Awake()
    {
        thornyPlant = transform.parent.GetComponent<ThornyPlant>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.CompareTag("Player");
        if (player && thornyPlant.IsHidden)
            thornyPlant.ChangeForm(false);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.gameObject.CompareTag("Player");
        if (player && !thornyPlant.IsHidden)
            StartCoroutine(WaitAndChangeForm());
    }

    IEnumerator WaitAndChangeForm()
    {
        while (thornyPlant.IsShooting)
            yield return null;
        thornyPlant.ChangeForm(true);
    }
}
