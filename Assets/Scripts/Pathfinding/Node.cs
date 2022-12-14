using UnityEngine;
using System.Collections.Generic;


public class Node
{
    private Vector3 _nodePosition;
    private List<Node> _neighbours;
    private Node _previousNode;

    private bool _isWallNode = false;
    private GameObject _colliderObject;

    public Node(Vector3 position, float size)
    {
        _nodePosition = position;
        _neighbours = new List<Node>();

        DetermineWallNode(size);
    }

    private void DetermineWallNode(float size)
    {
        //Check if it's a wall node
        if (Physics.CheckBox(_nodePosition, new Vector3(size / 2, 0.1f, size /  2), Quaternion.identity, -1, QueryTriggerInteraction.Ignore))
        {
            _isWallNode = true;
        }
    }

    public Vector3 GetPosition() => _nodePosition;

    public List<Node> GetNeighbours() => _neighbours;

    public void SetNeighbour(Node neighbour)
    {
        _neighbours.Add(neighbour);
    }

    public Node GetPreviousNode() => _previousNode;
    public void SetPreviousNode(Node previous)
    {
        _previousNode = previous;
    }

    public bool IsWallNode() => _isWallNode;
}

