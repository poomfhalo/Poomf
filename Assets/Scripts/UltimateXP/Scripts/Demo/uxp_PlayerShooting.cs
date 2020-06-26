using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class uxp_PlayerShooting : MonoBehaviour {

	public float fireRate = 0.5f;
	float cooldown = 0;
	public float damage = 25f;
	new AudioSource audio;
	public int clipSize;
	public int ammoInClip;
	public AudioClip reload;
	public AudioClip fire;
	public AudioClip emptyClip;
	public Text ammoText;
	public uxp_GunShake camShake;
	public float reloadTime = 4;
	private bool canReload = false;



	void Start() {

		audio = GetComponent<AudioSource>();
	}

		public void Reload(){

			StartCoroutine (ReloadTime());
			audio.PlayOneShot(reload, 0.7F);

		}

		public IEnumerator ReloadTime (){

			yield return new WaitForSeconds (reloadTime);

			ammoInClip = clipSize;
		}

	// Update is called once per frame
	void Update () {
		cooldown -= Time.deltaTime;
			ammoText.text = ammoInClip.ToString();

			if (Input.GetButton ("Fire1")) {
			
				if (ammoInClip == 0) {
				canReload = true;
				} else if (ammoInClip > 0) {
					Fire ();
				}
			}
		if (canReload && Input.GetKeyDown (KeyCode.R)) {
			canReload = false;
			Reload ();
		     }
		}

	public void Fire() {
		if(cooldown > 0) {
			return;
		}

		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		Transform hitTransform;
		Vector3   hitPoint;
			ammoInClip -= 1;
		audio.PlayOneShot(fire, 0.7F);
		camShake.shakeGun = 0.0001F;

		hitTransform = FindClosestHitObject(ray, out hitPoint);

		if(hitTransform != null) {

			uxp_Health h = hitTransform.GetComponent<uxp_Health>();

			while(h == null && hitTransform.parent) {
				hitTransform = hitTransform.parent;
				h = hitTransform.GetComponent<uxp_Health>();
			}

			if(h != null) {
				h.TakeDamage( damage );
			}


		}

		cooldown = fireRate;
	}

	Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint) {

			RaycastHit[] hits = Physics.RaycastAll (ray);

			Transform closestHit = null;
			float distance = 0;
			hitPoint = Vector3.zero;

			foreach (RaycastHit hit in hits) {
				if (hit.transform != this.transform && (closestHit == null || hit.distance < distance)) {

					closestHit = hit.transform;
					distance = hit.distance;
					hitPoint = hit.point;
				}
			}

			return closestHit;

	}
}
