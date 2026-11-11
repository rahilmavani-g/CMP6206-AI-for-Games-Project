using UnityEngine;
using System.Collections.Generic;

public class NavGrid : MonoBehaviour
{
    //X & Z
    public Vector2 gridWorldSize = new Vector2(50, 50);
    //half the size of one square
    public float nodeRadius = 0.5f;

    public LayerMask unwalkableMask;

    //grid[x,y]
    Node[,] grid;
    //size of 1 square
    float nodeDiameter;
    //how many
    int gridSizeX, gridSizeY;

    //player
    public Transform player;

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        nodeDiameter = nodeRadius * 2;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);

        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        grid = new Node[gridSizeX, gridSizeY];

        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);

                grid[x,y] = new Node (walkable, worldPoint,x, y);
            }
        }
    }

    void OnDrawGizmos()
    {
        // outline of the whole grid area
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid == null) return;

        //Node playerNode = NodeFromWorldPoint(player.position);

        //List<Node> playerNeighbours = GetNeighbours(playerNode);

        foreach (Node n in grid)
        {
            if (n.walkable)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            //if (n == playerNode)
            //{
            //    Gizmos.color = Color.cyan;
            //}
            //if (playerNeighbours.Contains(n))
            //{
            //    Gizmos.color = Color.blue;
            //}

            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //how far across and shift so the left edge is 0, then divide by the width
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;

        //keep it between 0 and 1, in case the position is just off the edge
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //turn the percentage into a square number
        int x = Mathf.FloorToInt(percentX * gridSizeX);
        int y = Mathf.FloorToInt(percentY * gridSizeY);

        //capping it to 49
        x = Mathf.Min(x, gridSizeX - 1);
        y = Mathf.Min(y, gridSizeY - 1);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                //skip the middle square (the node itself)
                if (dx == 0 && dy == 0) continue;

                int checkX = node.gridX + dx;
                int checkY = node.gridY + dy;

                //only keep it if it's INSIDE the grid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    //put that square into the list
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
}


