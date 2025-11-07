using UnityEngine;

public class DeadResetData : MonoBehaviour
{
    [SerializeField] private PlayerData Data;
    private void Awake()
    {
        if (Data.isDead)
            Data.Reset();
    }
}
