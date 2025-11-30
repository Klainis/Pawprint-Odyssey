using UnityEngine;

public class CrystalsManager : MonoBehaviour
{
    private static CrystalsManager instance;
    public static CrystalsManager Instance { get { return instance; } }

    public CrystalsExistence CrystalsExistenceInstance { get; set; }

    private void Awake()
    {
        instance = this;

        if (CrystalsExistenceInstance == null)
            CrystalsExistenceInstance = CrystalsExistence.CreateEmpty();
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
}
