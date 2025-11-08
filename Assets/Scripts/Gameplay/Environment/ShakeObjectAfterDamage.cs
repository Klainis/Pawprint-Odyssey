using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShakeObjectAfterDamage : MonoBehaviour
{
	[Header("Data")] 
    [SerializeField] private EnvironmentData environmentData;
    [Space(5)]

	[HideInInspector] public float shakeDuration;
	private float shakeMagnitude;
	private float dampingSpeed;

	Vector3 initialPosition;
	void Awake()
	{
		initialPosition = transform.localPosition;
	}

	void Start()
    {
        shakeDuration = 0;
		shakeMagnitude = environmentData.shakeMagnitude;
		dampingSpeed = environmentData.dampingSpeed;
    }
    private void Update()
    {
        if (shakeDuration <= 0)
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }

    public void Shake()
    {
        transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

        shakeDuration -= Time.deltaTime * dampingSpeed;
    }
}
