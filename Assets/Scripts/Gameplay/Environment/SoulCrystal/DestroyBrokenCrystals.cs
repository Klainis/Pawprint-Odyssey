using UnityEngine;

public class DestroyBrokenCrystals : MonoBehaviour
{
    [SerializeField] private string _crystalID;

    private EnvironmentManager crystalsManager;

    public string CrystalID { get { return _crystalID; } }

    private void Awake()
    {
        crystalsManager = EnvironmentManager.Instance;

        if (crystalsManager.CrystalsExistenceInstance != null)
            DestroyCrystal();
    }

    public void DestroyCrystal()
    {
        if (crystalsManager.CrystalsExistenceInstance.IsCrystalBroken(_crystalID))
        {
            Destroy(gameObject);
        }
    }

    public void AddInDestroyCrystalList()
    {
        crystalsManager.CrystalsExistenceInstance.BreakCrystal(_crystalID);
    }
}
