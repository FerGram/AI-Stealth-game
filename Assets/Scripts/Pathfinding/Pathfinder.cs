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

    public List<Node> GetPath(Rigidbody agent, Transform endPos)
    {
        return GetPath(agent, endPos.position);
    }
    public List<Node> GetPath(Rigidbody agent, Vector3 endPos)
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
            return path;
        }
        else Debug.LogWarning("Algorithm is not valid");
        return null;
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

    
}
