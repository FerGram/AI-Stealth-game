using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    protected void StartNavigation(Rigidbody agent, List<Node> path, float speed, float stoppingNodeDistance, float rotationSpeed)
    {
        CancelNavigation();
        if (path != null) StartCoroutine(NavigationRoutine(agent, path, speed, stoppingNodeDistance, rotationSpeed));
    }

    IEnumerator NavigationRoutine(Rigidbody agent, List<Node> path, float speed, float stoppingNodeDistance, float rotationSpeed)
    {
        Quaternion targetRotation = agent.transform.localRotation;
        while (path.Count > 0)
        {
            Vector3 agentPos = agent.position;
            Vector3 nodePos = path[0].GetPosition();

            agentPos.y = 0;
            nodePos.y = 0;

            while (Vector3.Distance(agentPos, nodePos) > stoppingNodeDistance)
            {
                //Move towards next node
                Vector3 movementDir = path[0].GetPosition() - agent.position;
                agent.velocity = movementDir.normalized * speed;
                agent.velocity = new Vector3(agent.velocity.x, 0, agent.velocity.z);

                //Finished rotation
                if (Quaternion.Angle(agent.rotation, targetRotation) < 0.01f)
                {
                    Vector3 targetLook = (nodePos - agentPos);

                    if (Mathf.Abs(targetLook.x) > Mathf.Abs(targetLook.z)) targetLook.z = 0;
                    else targetLook.x = 0;

                    targetLook = targetLook.normalized;

                    targetRotation = Quaternion.LookRotation(targetLook, Vector3.up);
                }
                Quaternion desiredRotation = Quaternion.RotateTowards(agent.transform.localRotation, targetRotation, Mathf.PI * rotationSpeed);
                agent.transform.localRotation = desiredRotation;
                yield return null;

                agentPos = agent.position;
                nodePos = path[0].GetPosition();

                agentPos.y = 0;
                nodePos.y = 0;
            }
            path.RemoveAt(0);
            yield return null;
        }
        agent.velocity = Vector3.zero;
    }

    protected void CancelNavigation()
    {
        StopCoroutine("StartNavigation");
    }
}
