using UnityEngine;
using System.Collections;



public class ShakeObject : MonoBehaviour
{
	[SerializeField] float shakeCooldown;
	[SerializeField] float shake_decay = 0.002f;
	[SerializeField] float shake_intensity = .3f;
	private Vector3 originPosition;
	private Quaternion originRotation;
	private float temp_shake_intensity = 0;
	private bool canShake = true;
	private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && canShake)
        {
			Shake();
			StartCoroutine("Shaking");
			canShake = false;
		}		
    }
    void Update()
	{
		if (temp_shake_intensity > 0)
		{
			transform.position = originPosition + Random.insideUnitSphere * temp_shake_intensity;
			transform.rotation = new Quaternion(
				originRotation.x + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
				originRotation.y + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
				originRotation.z + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
				originRotation.w + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f);
			temp_shake_intensity -= shake_decay;
		}
	}
	void Shake()
	{
		originPosition = transform.position;
		originRotation = transform.rotation;
		temp_shake_intensity = shake_intensity;
	}
	IEnumerator Shaking()
    {
		yield return new WaitForSeconds(shakeCooldown);
		canShake = true;
    }
}