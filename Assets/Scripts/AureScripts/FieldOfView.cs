using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
	
	public float viewRadius;
	[Range(0, 360)]

	public float viewAngle;

	[SerializeField]
	public LayerMask targetMask;

	[SerializeField]
	public LayerMask obstacleMask;

	[SerializeField]
	public LayerMask cakeMask;

	public List<Transform> visibleTargets = new List<Transform>();

	public Collider[] targetsInViewRadius;

	private Collider[] cakeInRadius;

    private void Update()
    {
		FindVisibleTargets();
    }
    IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	void FindVisibleTargets()
	{
		cakeInRadius = Physics.OverlapSphere(transform.position, viewRadius, cakeMask);




		if (cakeInRadius.Length != 0)
		{
			Transform target = cakeInRadius[0].transform;
			
			if (GameObject.Find("GameManager").GetComponent<ManageTime>().cakeInPlace == false)
            {
				if (TryGetComponent(out CocineroController controller))
				{
					controller.gameObject.GetComponent<CocineroController>().putingCake = true;
				}
				
			}
	


			
		}


		visibleTargets.Clear();

		targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{

				float dstToTarget = Vector3.Distance(transform.position, target.position);

				


				Vector3 raycastPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
				
				RaycastHit hit;
				Debug.DrawLine(raycastPosition, target.position);				

				if (Physics.Raycast(raycastPosition, dirToTarget, out hit, dstToTarget))
				{
					//Debug.Log(hit.collider.gameObject.name);
					if (hit.collider.gameObject.name == targetsInViewRadius[0].name || hit.collider.gameObject.tag == "Cake")
                    {
						if (TryGetComponent(out CocineroController controller))
						{
							controller.canSeePlayer = true;
							//print("Detectado");
						}
					}
                    else
                    {
						if (TryGetComponent(out CocineroController controller))
						{
							controller.canSeePlayer = false;
							//print("No detectado");
						}
					}
				}
				else
				{
					if (TryGetComponent(out CocineroController controller))
					{
						controller.canSeePlayer = false;
						//print("No detectado");
					}
				}
			}
            else
            {
				if (TryGetComponent(out CocineroController controller))
				{
					controller.canSeePlayer = false;
					//print("No detectado");
				}
			}


		}

		if(targetsInViewRadius.Length == 0)
        {
			if (TryGetComponent(out CocineroController controller))
			{
				controller.canSeePlayer = false;
				//print("No detectado");
			}
		}



		
	}


	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	private void OnDrawGizmos()
	{
		float totalFOV = viewAngle;
		float rayRange = viewRadius;
		float halfFOV = totalFOV / 2.0f;
		Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
		Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
		Vector3 leftRayDirection = leftRayRotation * transform.forward;
		Vector3 rightRayDirection = rightRayRotation * transform.forward;
		Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
		Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
		Gizmos.DrawWireSphere(transform.position, rayRange);
		
	}

	
}