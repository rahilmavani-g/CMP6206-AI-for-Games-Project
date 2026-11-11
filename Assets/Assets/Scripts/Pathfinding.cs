using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public NavGrid grid;

    public Transform seeker;   // where the path starts (the Alien)
    public Transform target;   // where it's going (the Player)

    List<Node> path;           // the latest path A* found

    float GetDistance(Node a, Node b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.gridX - b.gridX, 2) +  Mathf.Pow(a.gridY - b.gridY, 2));
    }

    void Update()
    {
        path = FindPath(seeker.position, target.position);
    }

    void OnDrawGizmos()
    {
        if (path == null) return;   

        Gizmos.color = Color.black;

        foreach (Node n in path)
        {
            Gizmos.DrawCube(n.worldPosition + Vector3.up * 0.2f, Vector3.one * 0.9f);
        }
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //found, but not checked
        List<Node> openSet = new List<Node>();
        //already checked
        HashSet<Node> closedSet = new HashSet<Node>();

        //putting start in openSet
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            //finding the node with lowest F
            Node current = openSet[0];
            foreach (Node n in openSet)
            {
                if (n.fCost < current.fCost)
                {
                    current = n;
                }
            }

            //moving from open to closed
            openSet.Remove(current);
            closedSet.Add(current);

            //if reached the goal then build the path and stop
            if(current == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(current))
            {
                //blocked by wall or its in closed set
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newG = current.gCost + GetDistance(current, neighbour);

                if(newG < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newG;
                    neighbour.hCost = GetDistance(neighbour,targetNode);
                    neighbour.parent = current;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;
        //walk backwards from the goal
        while (current != startNode)     
        {
            //following the parents
            path.Add(current);
            current = current.parent;    
        }
        //flip it
        path.Reverse();                   
        return path;
    }
}


