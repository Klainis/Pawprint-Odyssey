using System.Collections;
using UnityEngine;

public class DetectionAreaHandler : MonoBehaviour
{
    private ThornyPlant thornyPlant;

    private void Awake()
    {
        thornyPlant = transform.parent.GetComponent<ThornyPlant>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.CompareTag("Player");
        if (player && thornyPlant.IsHidden)
        {
            thornyPlant.ChangeForm(false);
            StartCoroutine(WaitAfterShowUp());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.gameObject.CompareTag("Player");
        if (player && thornyPlant.IsHidden)
            thornyPlant.ChangeForm(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
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

    IEnumerator WaitAfterShowUp()
    {
        yield return new WaitForSeconds(0.7f);
    }
}
