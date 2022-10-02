using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Node
{
    private Vector3 _nodePosition;
    private List<Node> _neighbours;
    private Node _previousNode;

    public Node(Vector3 position)
    {
        _nodePosition = position;
        _neighbours = new List<Node>();
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
}

