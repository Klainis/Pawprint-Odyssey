using UnityEngine;

public class CollapseTrigger : MonoBehaviour
{
    private CollapsingPlatform collapsingPlatform;

    private void Awake()
    {
        collapsingPlatform = GetComponentInParent<CollapsingPlatform>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.CompareTag("Player");
        var enemy = collision.gameObject.CompareTag("Enemy");
        if ((player || enemy) && !collapsingPlatform.IsHidden)
            StartCoroutine(collapsingPlatform.WaitAndChangeState());
    }
}
