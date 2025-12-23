using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class InstantiateMoney : MonoBehaviour
{
    [SerializeField] private GameObject _moneyObject;

    private Rigidbody2D _moneyRigidbody;

    private GameObject _moneyObjectInstance;

    [SerializeField] private int _objectCount = 5;
    [SerializeField] private float _force = 6;
    public void InstantiateMon(Vector3 position)
    {
        for (int i = 1; i <= _objectCount; i++)
        {
            _moneyObjectInstance = Instantiate(_moneyObject, position, Quaternion.identity);

            float angle = Random.Range(0f, 360f);
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            _moneyRigidbody = _moneyObjectInstance.GetComponent<Rigidbody2D>();

            float randomForce = Random.Range(_force * 0.7f, _force * 1.2f);
            _moneyRigidbody.AddForce(dir * randomForce, ForceMode2D.Impulse);
            Debug.Log(randomForce);
        }
    }
}
