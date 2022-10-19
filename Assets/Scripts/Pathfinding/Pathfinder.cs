//#define DEBUG_INFO

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] int _gridWidthResolution = 5;
    [SerializeField] int _gridHeightResolution = 5;
    [SerializeField] float _gridNodeSize = 1;
    [Space]
    [SerializeField] bool _showGrid = true;

    private Grid _grid;
    private Node _startNode;
    private Node _endNode;

    void Awake()
    {
        _grid = new Grid(_gridWidthResolution, _gridHeightResolution, transform.position.y, _gridNodeSize, _showGrid);
    }
    public void StartPathfinding(Rigidbody agent, Vector3 endPos, float speed, float stoppingNodeDistance, float rotationSpeed)
    {
        _startNode = GetClosestNodeToPosition(agent.position);
        _endNode = GetClosestNodeToPosition(endPos);

        List<Node> path = Algorithm.BFSAlgorithm(_grid, _startNode, _endNode);

        if (path != null)
        {
            //Print path
            for (int i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i - 1].GetPosition(), path[i].GetPosition(), Color.green, 3);
            }

            StartCoroutine(StartNavigation(agent, path, speed, stoppingNodeDistance, rotationSpeed));
#if DEBUG_INFO
            Debug.Log("Algorithm done, drawing path...");
#endif
        }
        else Debug.LogWarning("Algorithm is not valid");
    }

    private Node GetClosestNodeToPosition(Vector3 pos)
    {
        float minDistance = Mathf.Infinity;
        Node closest = null;

        for (int i = 0; i < _gridWidthResolution; i++)
        {
            for (int j = 0; j < _gridHeightResolution; j++)
            {
                //Distance between two points c2 = (xA − xB)2 + (yA − yB)2
                //There's no need to do the square root for this function (expensive operation for CPU)

                Node currentNode = _grid.GetNodeAt(i, j);
                Vector3 nodePos = currentNode.GetPosition();
                float distanceSquaredToPos = Mathf.Pow(pos.x - nodePos.x, 2) + Mathf.Pow(pos.z - nodePos.z, 2);
                if (currentNode != null && distanceSquaredToPos < minDistance)
                {
                    closest = currentNode;
                    minDistance = distanceSquaredToPos;
                }
            }
        }
        return closest;
    }

    public void StartPathfinding(Rigidbody agent, Transform endPos, float speed, float stoppingNodeDistance, float rotationSpeed)
    {
        StartPathfinding(agent, endPos.position, speed, stoppingNodeDistance, rotationSpeed);
    }

    IEnumerator StartNavigation(Rigidbody agent, List<Node> path, float speed, float stoppingNodeDistance, float rotationSpeed)
    {
        Quaternion targetRotation = agent.transform.localRotation;
        while (path.Count > 0)
        {
            while (Vector3.Distance(agent.position, path[0].GetPosition()) > stoppingNodeDistance)
            {
                //Move towards next node
                Vector3 movementDir = path[0].GetPosition() - agent.position;
                agent.velocity = movementDir.normalized * speed;

                //Finished rotation
                if (Quaternion.Angle(agent.rotation, targetRotation) < 0.01f)
                {
                    Vector3 nodePos = path[0].GetPosition();
                    Vector3 agentPos = agent.position;
                    nodePos.y = 0;
                    agentPos.y = 0;

                    Vector3 targetLook = (nodePos - agentPos);
                    Debug.Log(targetLook);

                    if (Mathf.Abs(targetLook.x) > Mathf.Abs(targetLook.z)) targetLook.z = 0;
                    else targetLook.x = 0;

                    targetLook = targetLook.normalized;
                    Debug.Log(targetLook);

                    targetRotation = Quaternion.LookRotation(targetLook, Vector3.up);
                    
                }
                Quaternion desiredRotation = Quaternion.RotateTowards(agent.transform.localRotation, targetRotation, Mathf.PI);
                agent.transform.localRotation = desiredRotation;
                yield return null;
            }

            path.RemoveAt(0);
            yield return null;
        }
        agent.velocity = Vector3.zero;
    }
}
