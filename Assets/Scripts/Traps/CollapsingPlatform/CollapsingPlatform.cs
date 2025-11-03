using System.Collections;
using UnityEngine;

public class CollapsingPlatform : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private Collider2D collapseTriggerCollider;

    private float timeBeforeHide = 1.5f;
    private float timeToReveal = 1.5f;
    private bool isHidden = false;

    public bool IsHidden { get { return isHidden; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        collapseTriggerCollider = GetComponentInChildren<CollapseTrigger>().GetComponent<Collider2D>();
    }

    public void ChangePlatformState(bool toHidden)
    {
        isHidden = toHidden;
        spriteRenderer.enabled = !toHidden;
        platformCollider.enabled = !toHidden;
        collapseTriggerCollider.enabled = !toHidden;
    }

    public IEnumerator WaitAndChangeState()
    {
        yield return new WaitForSeconds(timeBeforeHide);
        ChangePlatformState(true);

        yield return new WaitForSeconds(timeToReveal);
        while (Physics2D.OverlapBox(transform.position, platformCollider.bounds.size, 0, LayerMask.GetMask("Player")) != null)
            yield return null;
        ChangePlatformState(false);
    }
}
