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
	public Collider[] targetsInViewRadius;
	private Collider[] cakeInRadius;    
	private void Update()
    {
		FindVisibleTargets();
    }   
	void FindVisibleTargets()
	{
		//cakeInRadius = Physics.OverlapSphere(transform.position, viewRadius, cakeMask);
		//if (cakeInRadius.Length != 0)
		//{
		//	Transform target = cakeInRadius[0].transform;			
		//	if (GameObject.Find("GameManager").GetComponent<ManageTime>().cakeInPlace == false && !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerModification>().transportingCake)
  //          {
		//		if (TryGetComponent(out CocineroController controller))
		//		{
		//			controller.gameObject.GetComponent<CocineroController>().putingCake = true;
		//		}				
		//	}			
		//}
		targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 racoonRealPosition = new Vector3(target.position.x, target.position.y + 0.5f, target.position.z);
			Vector3 raycastPosition = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
			Vector3 dirToTarget = (racoonRealPosition - raycastPosition).normalized;

			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(raycastPosition, racoonRealPosition);		
				RaycastHit hit;
				
				Debug.DrawLine(raycastPosition, racoonRealPosition);					

				if (Physics.Raycast(raycastPosition, dirToTarget, out hit, dstToTarget))
				{
					
					if (hit.collider.gameObject.name == targetsInViewRadius[0].name || hit.collider.gameObject.tag == "Cake")
                    {
						CanSeePlayer();
					}
                    else
                    {
						CannotSeePlayer();
						Debug.Log("Rasho laser");
					}
				}
				else
				{
					CannotSeePlayer();
					

				}
			}
            else
            {
				CannotSeePlayer();
			}
		}
		if(targetsInViewRadius.Length == 0)
        {
			CannotSeePlayer();
		}		
	}
	private void CanSeePlayer()
    {
		if (TryGetComponent(out CocineroController controller))
		{
			controller.canSeePlayer = true;
		}
	}
	private void CannotSeePlayer()
    {
		if (TryGetComponent(out CocineroController controller))
		{
			controller.canSeePlayer = false;
		}
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