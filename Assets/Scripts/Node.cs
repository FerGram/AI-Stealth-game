using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Node
{
    private Vector3 _nodePosition;

    private Node neighbor;

    public Node(Vector3 position)
    {
        _nodePosition = position;
    }

    public Vector3 GetPosition() => _nodePosition;

    public void AddNeighbors()
    {

    }
}

