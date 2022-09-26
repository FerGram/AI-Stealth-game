using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class Grid : MonoBehaviour
{
    public int Width = 5;
    public int Height = 5;
    public float NodeSize = 1;

    private int _width;
    private int _height;
    private float _nodeSize;
    private Node[,] _nodes;

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateGrid();
        }

        DrawGrid();
    }

    private void GenerateGrid()
    {
        _width = Width;
        _height = Height;
        _nodeSize = NodeSize;
        _nodes = new Node[_width, _height];

        PopulateGrid();
    }

    private void PopulateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                //The extra _nodeSize/2 sets the position to the center instead of corner
                _nodes[i, j] = new Node(new Vector3(i * _nodeSize + _nodeSize/2, 0, j * _nodeSize + _nodeSize/2));
            }
        }
    }

    private void DrawGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                DrawNode(_nodes[i, j]);
            }
        }
    }

    private void DrawNode(Node node)
    {
        Color drawColor = Color.white;

        Vector3 nodePosition = node.GetPosition();

        Vector3 BLcorner = nodePosition + new Vector3(-_nodeSize / 2, 0, -_nodeSize / 2);
        Vector3 BRcorner = nodePosition + new Vector3(_nodeSize / 2, 0, -_nodeSize / 2);
        Vector3 TLcorner = nodePosition + new Vector3(-_nodeSize / 2, 0, _nodeSize / 2);
        Vector3 TRcorner = nodePosition + new Vector3(_nodeSize / 2, 0, _nodeSize / 2);

        Debug.DrawLine(BLcorner, BRcorner, drawColor, Time.deltaTime);
        Debug.DrawLine(BRcorner, TRcorner, drawColor, Time.deltaTime);
        Debug.DrawLine(TRcorner, TLcorner, drawColor, Time.deltaTime);
        Debug.DrawLine(TLcorner, BLcorner, drawColor, Time.deltaTime);
    }

}
