using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")] public float cellSize = 1f;
    public float yOffset = 0f;

    [Header("NavMesh Area")] public int navMeshAreaMask = NavMesh.AllAreas;

    public List<GridData> gridCells = new List<GridData>();

    void Start()
    {
        GenerateGridFromNavMesh();
    }

    void GenerateGridFromNavMesh()
    {
        gridCells.Clear();

        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        if (navMeshData.vertices.Length == 0)
        {
            Debug.LogWarning("No NavMesh data found!");
            return;
        }

        Bounds bounds = GetNavMeshBounds(navMeshData.vertices);

        for (float x = bounds.min.x; x < bounds.max.x; x += cellSize)
        {
            for (float z = bounds.min.z; z < bounds.max.z; z += cellSize)
            {
                Vector3 samplePos = new Vector3(x + cellSize / 2, bounds.center.y + yOffset, z + cellSize / 2);

                if (IsPointOnNavMesh(samplePos, cellSize * 0.5f))
                {
                    Vector2Int gridPos = new Vector2Int(
                        Mathf.RoundToInt((x - bounds.min.x) / cellSize),
                        Mathf.RoundToInt((z - bounds.min.z) / cellSize)
                    );

                    gridCells.Add(new GridData(gridPos, samplePos));
                }
            }
        }
    }

    Bounds GetNavMeshBounds(Vector3[] vertices)
    {
        Bounds bounds = new Bounds(vertices[0], Vector3.zero);
        foreach (Vector3 v in vertices)
            bounds.Encapsulate(v);
        return bounds;
    }

    bool IsPointOnNavMesh(Vector3 pos, float maxDist)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(pos, out hit, maxDist, navMeshAreaMask);
    }

    public GridData? GetNearestWalkableGridCell(Vector3 position)
    {
        if (gridCells == null || gridCells.Count == 0)
            return null;

        GridData? nearest = null;
        float minDist = float.MaxValue;

        foreach (var cell in gridCells)
        {
            float dist = Vector3.Distance(position, cell.worldPosition);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = cell;
            }
        }

        return nearest;
    }

    void OnDrawGizmos()
    {
        if (gridCells == null || gridCells.Count == 0)
            return;

        Gizmos.color = Color.green;
        foreach (var cell in gridCells)
        {
            Gizmos.DrawWireCube(cell.worldPosition, new Vector3(cellSize, 0.1f, cellSize));
        }
    }
}

[System.Serializable]
public struct GridData
{
    public Vector2Int gridPosition;
    public Vector3 worldPosition;

    public GridData(Vector2Int gridPosition, Vector3 worldPosition)
    {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;
    }
}