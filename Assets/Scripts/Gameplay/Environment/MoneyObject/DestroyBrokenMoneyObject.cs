using UnityEngine;

public class DestroyBrokenMoneyObject : MonoBehaviour
{
    [SerializeField] private string _moneyObjectID;

    private EnvironmentManager _environmentManager;

    public string MoneyObjectID { get { return _moneyObjectID; } }

    private void Awake()
    {
        _environmentManager = EnvironmentManager.Instance;

        if (_environmentManager.MoneyObjectExistenceInstance != null)
            DestroyMoneyObject();
    }

    public void DestroyMoneyObject()
    {
        if (_environmentManager.MoneyObjectExistenceInstance.IsMoneyObjectBroken(_moneyObjectID))
        {
            Destroy(gameObject);
        }
    }

    public void AddInDestroyMoneyObjectList()
    {
        _environmentManager.MoneyObjectExistenceInstance.BreakMoneyObject(_moneyObjectID);
    }
}
