using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SafeGroundSaver : MonoBehaviour
{
    private static SafeGroundSaver instance;
    public static SafeGroundSaver Instance { get { return instance; } }

    [SerializeField] private float saveFrequency = 1f;

    public Vector3 SafeGroundLocation { get; private set; } = Vector2.zero;

    private Coroutine _safeGroundCoroutine;
    private string _sceneName;
    private string _newSceneName;

    private GroundCheckForSaveGround _groundCheck;

    private void Awake()
    {
        instance = this;
        _groundCheck = GetComponent<GroundCheckForSaveGround>();
        SafeGroundLocation = transform.position;
    }

    private void Start()
    {
        _sceneName = _newSceneName = SceneManager.GetActiveScene().name;

        _safeGroundCoroutine = StartCoroutine(SaveGroundLocation());
    }

    public void SetNewSafeGroundLocation()
    {
        if (_groundCheck.IsSaveGround())
        {
            SafeGroundLocation = transform.position;
        }
        else
        {
            var safeLocation = GameObject.FindGameObjectWithTag("SafeGroundLocation");

            if (safeLocation != null)
            {
                SafeGroundLocation = safeLocation.transform.position;
            }
        }

    }

    private IEnumerator SaveGroundLocation()
    {
        var elapsedTime = 0f;
        while (elapsedTime < saveFrequency)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (_groundCheck.IsSaveGround())
        {
            SafeGroundLocation = transform.position;
        }

        _safeGroundCoroutine = StartCoroutine(SaveGroundLocation());
    }
}
