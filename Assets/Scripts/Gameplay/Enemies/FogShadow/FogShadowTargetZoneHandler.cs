using UnityEngine;
using System;

public class FogShadowTargetZoneHandler : MonoBehaviour
{
    public event Action TargetZoneEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TargetZoneEnter?.Invoke();
        }
    }
}
