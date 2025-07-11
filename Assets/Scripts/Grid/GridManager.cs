using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridData
{
    public Vector2Int GridPos;
    public Vector3 WorldPos;
    public bool IsWalkable;
}

public class GridManager : MonoBehaviour
{
    public Vector2 gridWorldSize = new Vector2(20, 20); // this will change
    public float cellSize = 1f; // Size of each cell

    public List<GridData> currentGrid;
    private Vector3 origin; // Bottom-left of NavMesh

    void Start()
    {
        origin = GetNavMeshBottomLeft();
        GenerateGrid();
    }

    void GenerateGrid()
    {
        int gridWidth = Mathf.RoundToInt(gridWorldSize.x);
        int gridHeight = Mathf.RoundToInt(gridWorldSize.y);
        currentGrid = new();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellCenter = origin + new Vector3(x * cellSize, 0, y * cellSize);
                bool walkable = NavMesh.SamplePosition(cellCenter, out var hit, cellSize * 0.5f, NavMesh.AllAreas);

                GridData gridData = new GridData
                {
                    GridPos = new Vector2Int(x, y),
                    WorldPos = cellCenter,
                    IsWalkable = walkable
                };
                currentGrid.Add(gridData);
            }
        }
    }

    public GridData GetNearestGridCell(Vector3 worldPos)
    {
        GridData nearest = null;
        float minDist = float.MaxValue;
        foreach (var cell in currentGrid)
        {
            float dist = Vector3.Distance(worldPos, cell.WorldPos);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = cell;
            }
        }

        return nearest;
    }

    public GridData GetNearestWalkableGridCell(Vector3 worldPos)
    {
        GridData nearest = null;
        float minDist = float.MaxValue;
        foreach (var cell in currentGrid)
        {
            if (!cell.IsWalkable) continue;
            float dist = Vector3.Distance(worldPos, cell.WorldPos);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = cell;
            }
        }

        return nearest;
    }

    Vector3 GetNavMeshBottomLeft()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        Vector3 min = navMeshData.vertices[0];
        foreach (var v in navMeshData.vertices)
        {
            if (v.x < min.x) min.x = v.x;
            if (v.z < min.z) min.z = v.z;
        }

        min.y = 0; // Usually ground level
        return min;
    }

    void OnDrawGizmos()
    {
        if (currentGrid == null) return;

        Gizmos.color = Color.green;
        foreach (var gridData in currentGrid)
        {
            Gizmos.DrawWireCube(gridData.WorldPos, new Vector3(cellSize, 0.1f, cellSize));
            if (!gridData.IsWalkable)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(gridData.WorldPos, new Vector3(cellSize * 0.5f, 0.1f, cellSize * 0.5f));
                Gizmos.color = Color.green; // Reset color
            }
        }
    }
}