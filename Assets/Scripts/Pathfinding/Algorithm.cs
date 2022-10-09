using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

static class Algorithm
{
    static public List<Node> BFSAlgorithm(Grid grid, Node start, Node end)
    {
        var watch = new Stopwatch();

        List<Node> visited = new List<Node>();
        Queue<Node> nodeQueue = new Queue<Node>();

        Node currentNode = null;

        nodeQueue.Enqueue(start);
        grid.CleanAllPreviousNodes();

        watch.Start();
        while (currentNode != end || nodeQueue.Count == 0)
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
        UnityEngine.Debug.Log("Main loop took: " + watch.ElapsedMilliseconds + " ms");

        if (currentNode == end) return GetPath(currentNode);
        else
        {
            UnityEngine.Debug.LogWarning("Couldn't reach the end");
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

