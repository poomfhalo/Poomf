using UnityEngine;
using System.Collections;

public class uxp_GunShake : MonoBehaviour {

	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;

	// How long the object should shake for.
	public float shakeGun = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeGunAmount = 0.7f;
	public float decreaseGunFactor = 1.0f;

	Vector3 originalPos;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	void Update()
	{

			if (shakeGun > 0) {
				camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeGunAmount;

				shakeGun -= Time.deltaTime * decreaseGunFactor;
			} else {
				shakeGun = 0f;
				camTransform.localPosition = originalPos;


		}
	}
}