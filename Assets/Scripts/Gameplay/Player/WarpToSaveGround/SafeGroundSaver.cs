using System.Collections;
using UnityEngine;

public class SafeGroundSaver : MonoBehaviour
{
    private static SafeGroundSaver instance;
    public static SafeGroundSaver Instance { get { return instance; } }

    [SerializeField] private float saveFrequency = 1f;

    public Vector2 SafeGroundLocation { get; private set; } = Vector2.zero;

    private Coroutine safeGroundCoroutine;
    private GroundCheckForSaveGround groundCheck;

    private void Awake()
    {
        instance = this;
        groundCheck = GetComponent<GroundCheckForSaveGround>();
        SafeGroundLocation = transform.position;
    }

    private void Start()
    {
        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());
    }

    private IEnumerator SaveGroundLocation()
    {
        var elapsedTime = 0f;
        while (elapsedTime < saveFrequency)
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
