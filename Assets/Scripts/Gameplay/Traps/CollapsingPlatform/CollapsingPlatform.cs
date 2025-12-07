using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class CollapsingPlatform : MonoBehaviour
{
    [SerializeField] private float timeBeforeHide = 1.5f;
    [SerializeField] private float timeToReveal = 1.5f;

    private SpriteShapeRenderer spriteRenderer;
    private Collider2D platformCollider;
    private Collider2D collapseTriggerCollider;

    private bool isHidden = false;

    public bool IsHidden { get { return isHidden; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteShapeRenderer>();
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
