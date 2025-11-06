using System.Collections;
using UnityEngine;

public class SafeGroundSaver : MonoBehaviour
{
    [SerializeField] private float saveFreauency = 2f;

    public Vector2 SafeGroundLocation { get; private set; } = Vector2.zero;

    private Coroutine safeGroundCoroutine;
    private CharacterController2D playerController;

    private void Awake()
    {
        playerController = GetComponent<CharacterController2D>();
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

        if (playerController.m_Grounded)
        {
            SafeGroundLocation = transform.position;
        }

        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());
    }
}
