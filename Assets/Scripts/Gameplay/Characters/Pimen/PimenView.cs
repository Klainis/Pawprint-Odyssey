using UnityEngine;

public class PimenView : MonoBehaviour
{
    private static PimenView _instance;
    public static PimenView Instance {  get { return _instance; } }

    [SerializeField] private float _timeToTrick = 9f;

    private PimenAnimation _pimenAnimation;

    private float _currentTimeToTrick = 0f;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;

        _pimenAnimation = GetComponent<PimenAnimation>();
    }

    private void Start()
    {
        _currentTimeToTrick = _timeToTrick;
    }

    private void Update()
    {
        if (PlayerMove.Instance != null )
        {
            if (!PlayerMove.Instance.IsMoving/* && PlayerMove.Instance.IsGrounded*/)
            {
                Debug.Log("Æäåì òðþêà");
                _currentTimeToTrick -= Time.deltaTime;

                if (_currentTimeToTrick <= 0)
                {
                    Debug.Log("Òðþê");

                    _currentTimeToTrick = _timeToTrick;
                    _pimenAnimation.SetIsTrick(true);
                }
                else if (_currentTimeToTrick > 0)
                {
                    _pimenAnimation.SetIsTrick(false);
                }
            }
            else
            {
                _currentTimeToTrick = _timeToTrick;
                _pimenAnimation.SetIsTrick(false);
            }
        }
    }
}
