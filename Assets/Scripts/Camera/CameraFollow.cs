using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private float FollowSpeed = 2f;
	[SerializeField] private Transform Target;

	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	private Transform camTransform;

	// How long the object should shake for.
	[SerializeField] private float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	[SerializeField] private float shakeAmount = 0.1f;
	[SerializeField] private float decreaseFactor = 1.0f;
	private Vector3 newPosition;


    Vector3 originalPos;

	void Awake()
	{
		Cursor.visible = false;
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	private void Update()
	{
		if (Target != null)
			newPosition = Target.position;

		newPosition.z = -10;
		transform.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);

		if (shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
	}

	public void ShakeCamera()
	{
		originalPos = camTransform.localPosition;
		shakeDuration = 0.2f;
	}
}
