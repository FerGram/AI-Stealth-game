//#define DEBUG_INFO

using System.Diagnostics;
using System.Collections.Generic;

static class Algorithm
{
    static public List<Node> BFSAlgorithm(Grid grid, Node start, Node end)
    {
        var watch = new Stopwatch();

        List<Node> visited = new List<Node>();
        Queue<Node> nodeQueue = new Queue<Node>();

#if DEBUG_INFO
        UnityEngine.Debug.Log("Start node position at: " + start.GetPosition());
        UnityEngine.Debug.Log("End node position at: " + end.GetPosition());
#endif

        Node currentNode = null;
        nodeQueue.Enqueue(start);
        grid.CleanAllPreviousNodes();

        watch.Start();
        while (currentNode != end && nodeQueue.Count > 0)
        {
            currentNode = nodeQueue.Dequeue();

            if (currentNode != null)
            {
                List<Node> neighbours = currentNode.GetNeighbours();
                foreach (var neighbour in neighbours)
                {
                    if (neighbour != null && !visited.Contains(neighbour) && !nodeQueue.Contains(neighbour))
                    {
                        neighbour.SetPreviousNode(currentNode);
                        nodeQueue.Enqueue(neighbour);
                    }
                }

                visited.Add(currentNode);
            }
        }
        watch.Stop();
#if DEBUG_INFO
        UnityEngine.Debug.Log("Main loop took: " + watch.ElapsedMilliseconds + " ms");
#endif
        if (currentNode == end) return GetPath(currentNode);
        else
        {
            UnityEngine.Debug.LogWarning("Queue length reached 0");
            return null;
        }
    }

    static public void AStarAlgorithm(Grid grid, Node start, Node end)
    {

    }

    static private List<Node> GetPath(Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode.GetPreviousNode() != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.GetPreviousNode();
        }
        path.Reverse();
        return path;
    }
}