using UnityEngine;


public class Grid
{
    private int _width;
    private int _height;
    private float _nodeSize;

    private Node[,] _nodes;

    public Grid(int gridWidthSize, int gridHeightSize, float gridHeight, float gridNodeSize, bool drawGrid)
    {
        _width = gridWidthSize;
        _height = gridHeightSize;
        _nodeSize = gridNodeSize;
        _nodes = new Node[_width, _height];

        PopulateGrid(gridHeight);
        SetNodesNeighbours();

        if (drawGrid) DrawGrid(true);
    }

    public Node GetNodeAt(int i, int j)
    {
        return _nodes[i, j];
    }

    // <sumamary>
    // Populate Grid array with nodes
    // </summary>
    private void PopulateGrid(float gridHeight)
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                //The extra _nodeSize/2 sets the position to the center instead of corner
                Vector3 nodePos = new Vector3(i * _nodeSize + _nodeSize / 2, gridHeight, j * _nodeSize + _nodeSize / 2);
                _nodes[i, j] = new Node(nodePos, _nodeSize);
            }
        }
    }

    // <sumamary>
    // For each node in the grid, set its neighbours for Algorithm purposes
    // </summary>
    private void SetNodesNeighbours()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_nodes[i, j] != null && !_nodes[i, j].IsWallNode()) //Currently not checking if neighbour nodes are null
                {
                    if (j + 1 < _height && !_nodes[i, j + 1].IsWallNode()) _nodes[i, j].SetNeighbour(_nodes[i, j + 1]);    //UP neighbour
                    if (i + 1 < _width  && !_nodes[i + 1, j].IsWallNode()) _nodes[i, j].SetNeighbour(_nodes[i + 1, j]);    //RIGHT neighbour
                    if (j - 1 >= 0      && !_nodes[i, j - 1].IsWallNode()) _nodes[i, j].SetNeighbour(_nodes[i, j - 1]);    //DOWN neighbour
                    if (i - 1 >= 0      && !_nodes[i - 1, j].IsWallNode()) _nodes[i, j].SetNeighbour(_nodes[i - 1, j]);    //LEFT neighbour
                }
            }
        }
    }

    // <sumamary>
    // For each node, clean the node it came from
    // </summary>
    public void CleanAllPreviousNodes()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                _nodes[i, j].SetPreviousNode(null);
            }
        }
    }

    // <sumamary>
    // Drawing visual elements on screen
    // </summary>
    private void DrawGrid(bool drawForever = false)
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                DrawNode(_nodes[i, j], drawForever);
            }
        }
    }
    private void DrawNode(Node node, bool drawForever = false)
    {
        Color drawColor = node.IsWallNode() ? Color.red : Color.white;
        float drawTime = drawForever ? Mathf.Infinity : Time.deltaTime;

        Vector3 nodePosition = node.GetPosition();

        Vector3 BLcorner = nodePosition + new Vector3(-_nodeSize / 2, 0, -_nodeSize / 2);
        Vector3 BRcorner = nodePosition + new Vector3(_nodeSize / 2, 0, -_nodeSize / 2);
        Vector3 TLcorner = nodePosition + new Vector3(-_nodeSize / 2, 0, _nodeSize / 2);
        Vector3 TRcorner = nodePosition + new Vector3(_nodeSize / 2, 0, _nodeSize / 2);

        Debug.DrawLine(BLcorner, BRcorner, drawColor, drawTime);
        Debug.DrawLine(BRcorner, TRcorner, drawColor, drawTime);
        Debug.DrawLine(TRcorner, TLcorner, drawColor, drawTime);
        Debug.DrawLine(TLcorner, BLcorner, drawColor, drawTime);
    }

}
