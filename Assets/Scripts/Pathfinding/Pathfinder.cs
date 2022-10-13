using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] int _gridWidthResolution = 5;
    [SerializeField] int _gridHeightResolution = 5;
    [SerializeField] float _gridNodeSize = 1;

    private Grid _grid;
    private Node _startNode;
    private Node _endNode;

    void Awake()
    {
        _grid = new Grid(_gridWidthResolution, _gridHeightResolution, _gridNodeSize);
    }

    public void StartPathfinding(Transform agent, Transform endPos, float speed, float stoppingNodeDistance)
    {
        _startNode = GetClosestNodeToPosition(agent.position);
        _endNode = GetClosestNodeToPosition(endPos.position);

        _grid.DrawGrid(true);

        List<Node> path = Algorithm.BFSAlgorithm(_grid, _startNode, _endNode);

        if (path != null)
        {
            //Print path
            for (int i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i - 1].GetPosition(), path[i].GetPosition(), Color.green, 3);
            }

            StartCoroutine(StartNavigation(agent, path, speed, stoppingNodeDistance));
            Debug.Log("Algorithm done, drawing path...");
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

    IEnumerator StartNavigation(Transform agent, List<Node> path, float speed, float stoppingNodeDistance)
    {
        while (path.Count > 0)
        {
            while (Vector3.Distance(agent.position, path[0].GetPosition()) > stoppingNodeDistance)
            {
                Vector3 movementDir = path[0].GetPosition() - agent.position;
                agent.Translate(movementDir.normalized * Time.deltaTime * speed);
                yield return null;
            }
            path.RemoveAt(0);
        }
    }
}
