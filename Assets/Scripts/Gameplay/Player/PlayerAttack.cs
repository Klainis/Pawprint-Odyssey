using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    #region SerializeFields

    [Header("Parameters")]
    [SerializeField] private float attackSeriesTimeout = 0.9f;
    [SerializeField] private int maxAttackSeriesCount = 3;

    //[Header("Particles")]
    //[SerializeField] private ParticleSystem _attackParticle;

    #endregion

    #region Variables

    private static PlayerAttack instance;
    private PlayerView playerView;
    private PlayerAnimation playerAnimation;
    private PlayerMana playerMana;
    private Transform attackCheck;

    const float attackCheckRadius = 1.1f;
    private float lastAttackTime;
    private int attackSeriesCount = 0;
    private bool isAttacking = false;
    private bool canAttack = true;

    #endregion

    #region Properties

    public static PlayerAttack Instance { get { return instance; } }
    public int AttackSeriesCount { get { return attackSeriesCount; } private set { attackSeriesCount = value; } }
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public bool IsAttacking { get { return isAttacking; } }
    public bool SpendMana { get; set; } = true;

    #endregion

    #region Common Methods

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;

        playerMana = GetComponent<PlayerMana>();
        playerView = GetComponent<PlayerView>();
        playerAnimation = GetComponent<PlayerAnimation>();

        attackCheck = transform.Find("AttackCheck");
    }

    private void Update()
    {
        playerAnimation.ApplyRootMotion(false);

        if (attackSeriesCount > 0 && (Time.time - lastAttackTime > attackSeriesTimeout))
            ResetCombo();

        if (PlayerInput.Instance.DamageDashActive && playerMana.isActiveAndEnabled && PlayerView.Instance.PlayerModel.Mana >= 10)
        {
            if (SpendMana == true)
            {
                playerMana.SpendMana("DamageDash", 1);
                SpendMana = false;
            }
        }
    }

    #endregion

    public void Attack()
    {
        if (isAttacking || !canAttack) return;

        lastAttackTime = Time.time;
        attackSeriesCount++;

        playerAnimation.SetBoolIsAttacking(true);

        if (attackSeriesCount == 1)
        {
            playerView.PlayerModel.SetDamage(1);
            playerAnimation.SetTriggerAttack(1);
        }
        else if (attackSeriesCount == 2)
        {
            playerView.PlayerModel.SetDamage(1);
            playerAnimation.SetTriggerAttack(2);
        }
        else if (attackSeriesCount == 3)
        {
            playerView.PlayerModel.SetDamage(3);
            playerAnimation.SetTriggerAttack(3);
        }

        isAttacking = true;
    }

    public void Attack4()
    {
        if (isAttacking || !canAttack) return;

        lastAttackTime = Time.time;
        attackSeriesCount++;
        maxAttackSeriesCount = 4;

        playerAnimation.SetBoolIsAttacking(true);

        switch (attackSeriesCount)
        {
            case 1:
                playerView.PlayerModel.SetDamage(1);
                break;
            case 2:
                playerView.PlayerModel.SetDamage(1);
                break;
            case 3:
                playerView.PlayerModel.SetDamage(3);
                break;
            case 4:
                playerView.PlayerModel.SetDamage(4);
                break;
        }
        if (attackSeriesCount > 0 && attackSeriesCount <= maxAttackSeriesCount)
            playerAnimation.SetTriggerAttack(attackSeriesCount);

        isAttacking = true;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        canAttack = false;

        if (attackSeriesCount >= maxAttackSeriesCount)
        {
            StartCoroutine(AttackCooldown(0.3f));
            //Debug.Log("Серия завершена");
            ResetCombo();
        }
        else
            StartCoroutine(AttackCooldown(0.03f));
    }

    private void ResetCombo()
    {
        attackSeriesCount = 0;
        isAttacking = false;
        for (var i = 1; i < maxAttackSeriesCount + 1; i++)
            playerAnimation.ResetTriggerAttack(i);
    }

    public void AttackDamage() //Вызывается в середние анимаци атаки
    {
        var collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        for (var i = 0; i < collidersEnemies.Length; i++)
        {
            var enemy = collidersEnemies[i].gameObject;
            var objectEnvironment = collidersEnemies[i].gameObject;

            var damageToApply = playerView.PlayerModel.Damage;

            if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                damageToApply = -damageToApply;

            if (enemy.CompareTag("Enemy"))
            {
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply, SendMessageOptions.DontRequireReceiver);

                if (!enemy.CompareTag("isDead") && !objectEnvironment.CompareTag("Object"))
                    playerMana.GetMana();
            }

            if (objectEnvironment.CompareTag("Object"))
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", new object[2] { false, damageToApply });
        }
    }

    public void AttackDashDamage()
    {
        if (playerMana.isActiveAndEnabled && PlayerView.Instance.PlayerModel.Mana >= 10)
        {

            var collidersEnemies = Physics2D.OverlapCircleAll(transform.position, attackCheckRadius);
            for (var i = 0; i < collidersEnemies.Length; i++)
            {
                var enemy = collidersEnemies[i].gameObject;
                var objectEnvironment = collidersEnemies[i].gameObject;

                playerView.PlayerModel.SetDamage(1);
                var damageToApply = playerView.PlayerModel.Damage;

                if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                    damageToApply = -damageToApply;

                if (enemy.CompareTag("Enemy"))
                {
                    collidersEnemies[i].gameObject.SendMessage("ApplyDamage", damageToApply);
                }
            }
        }

    }

    private IEnumerator AttackCooldown(float durationAfterSeries)
    {
        yield return new WaitForSeconds(durationAfterSeries);
        canAttack = true;
    }
}
