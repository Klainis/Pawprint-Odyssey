using UnityEngine;
using UnityEngine.Events;

public class GuardianOwlView : MonoBehaviour
{
    public EnemyModel Model { get; private set; }

    [Header("Main params")]
    [SerializeField] private EnemyData data;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float secondStageLifeCoef = 0.6f;
    [SerializeField] private bool isInvincible = false;

    [Header("Events")]
    [SerializeField] private UnityEvent Hit;
    [SerializeField] private UnityEvent Die;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private ParticleSystem _playerWeaponParticle;

    [Space(5)]
    [SerializeField] private FightDoor _fightDoor;

    //private const float groundedRadius = 0.2f;

    private Rigidbody2D rigidBody;
    //private Transform groundCheck;

    private ParticleSystem _damageParticleInstance;
    private ParticleSystem _playerWeaponParticleInstance;

    //private SGAnimation sgAnimation;
    //private SGAttack sgAttack;
    //private SGMove sgMove;
    private DamageFlash _damageFlash;
    private ScreenShaker _screenShaker;

    private int maxLifeForReading;
    private float secondStageLifeAmount;
    private bool isSecondStage = false;
    private bool isHitted = false;
    //private bool isAccelerated = false;
    //private bool moveDisabled = false;
    private bool facingRight = true;

    public Rigidbody2D RigidBody { get { return rigidBody; } }
    public int MaxLifeForReading { get { return maxLifeForReading; } }
    public bool IsHitted { get { return isHitted; } }
    //public bool IsAccelerated { get { return isAccelerated; } set { isAccelerated = value; } }
    //public bool MoveDisabled { get { return moveDisabled; } set { moveDisabled = value; } }
    public bool FacingRight { get { return facingRight; } }

    private void Awake()
    {
        Model = new EnemyModel(data.Life, data.Speed, data.Damage);

        maxLifeForReading = Model.Life;
        secondStageLifeAmount = Model.Life * secondStageLifeCoef;

        rigidBody = GetComponent<Rigidbody2D>();
        //groundCheck = transform.Find("GroundCheck");

        //sgAnimation = GetComponent<SGAnimation>();
        //sgAttack = GetComponent<SGAttack>();
        //sgMove = GetComponent<SGMove>();
        _damageFlash = GetComponent<DamageFlash>();
        _screenShaker = GetComponent<ScreenShaker>();
    }
}
