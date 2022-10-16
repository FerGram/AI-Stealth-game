using System;
using System.Collections.Generic;
using UnityEngine;

class AlgorithmTester : MonoBehaviour
{
    [SerializeField] int _gridWidth = 5;
    [SerializeField] int _gridHeight = 5;
    [SerializeField] float _gridNodeSize = 1;

    [SerializeField] Transform _testStartPosition;
    [SerializeField] Transform _testEndPosition;

    private Grid _grid;
    private Node _startNode;
    private Node _endNode;

    void Start()
    {
        _grid = new Grid(_gridWidth, _gridHeight, _gridNodeSize);
        _startNode = GetClosestNodeToPosition(_testStartPosition.position);
        _endNode = GetClosestNodeToPosition(_testEndPosition.position);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _grid = new Grid(_gridWidth, _gridHeight, _gridNodeSize);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartPathfinding();
        }
        _grid.DrawGrid();
    }

    //TODO: detect collisions and mark nodes as wall
    private void StartPathfinding()
    {
        //TODO: remove two lines below. Just for testing
        _startNode = GetClosestNodeToPosition(_testStartPosition.position);
        _endNode = GetClosestNodeToPosition(_testEndPosition.position);

        List<Node> path = Algorithm.BFSAlgorithm(_grid, _startNode, _endNode);
        Debug.Log("Algorithm ready");

        //Print path
        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1].GetPosition(), path[i].GetPosition(), Color.green, 5);
        }
    }

    private Node GetClosestNodeToPosition(Vector3 pos)
    {
        float minDistance = Mathf.Infinity;
        Node closest = null;

        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                //Distance between two points c2 = (xA − xB)2 + (yA − yB)2
                //There's no need to do the square root for this function (expensive operation)

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
