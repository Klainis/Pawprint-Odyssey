using UnityEngine;

public class LastPlatform : MonoBehaviour
{
    private static LastPlatform instance;
    public Vector3 lastPosition {  get; private set; }
    private CharacterController2D playerController;
    private Transform player;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerController = GetComponent<CharacterController2D>();
        player = GetComponent<Transform>();

        lastPosition = Vector3.zero;
    }

    private void Update()
    {
        if (playerController.m_Grounded)
        {
            lastPosition = player.transform.position;
        }

        //Debug.Log(lastPosition);
    }
}