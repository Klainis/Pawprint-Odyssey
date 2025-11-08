using System.Collections;
using UnityEngine;

public class SafeGroundSaver : MonoBehaviour
{
    [SerializeField] private float saveFreauency = 1f;

    public Vector2 SafeGroundLocation { get; private set; } = Vector2.zero;

    private Coroutine safeGroundCoroutine;
    private GroundCheckForSaveGround groundCheck;

    private void Awake()
    {
        groundCheck = GetComponent<GroundCheckForSaveGround>();
    }

    private void Start()
    {
        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());

        SafeGroundLocation = transform.position;
    }

    private IEnumerator SaveGroundLocation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < saveFreauency)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (groundCheck.IsSaveGround())
        {
            SafeGroundLocation = transform.position;
        }

        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());
    }
}
