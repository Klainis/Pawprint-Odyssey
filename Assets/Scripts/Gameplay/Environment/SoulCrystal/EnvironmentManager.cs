using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    private static EnvironmentManager instance;
    public static EnvironmentManager Instance { get { return instance; } }

    public CrystalsExistence CrystalsExistenceInstance { get; set; }
    public MoneyObjectExistence MoneyObjectExistenceInstance { get; set; }

    private void Awake()
    {
        instance = this;

        if (CrystalsExistenceInstance == null)
            CrystalsExistenceInstance = CrystalsExistence.CreateEmpty();

        if (MoneyObjectExistenceInstance == null)
            MoneyObjectExistenceInstance = MoneyObjectExistence.CreateEmpty();
    }

    public static void DestroyBrokenCrystals()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (enemy.name == "SoulCrystal")
            {
                var destroyCrystals = enemy.GetComponent<DestroyBrokenCrystals>();
                destroyCrystals.DestroyCrystal();
            }
        }
    }

    public static void DestroyBrokenMoneyObject()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (enemy.name == "DestructibleMoneyObject")
            {
                var destroyCrystals = enemy.GetComponent<DestroyBrokenMoneyObject>();
                destroyCrystals.DestroyMoneyObject();
            }
        }
    }
}
