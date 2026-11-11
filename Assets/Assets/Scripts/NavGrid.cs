using UnityEngine;

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

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
        }
    }
}
