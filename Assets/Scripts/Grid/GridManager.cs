using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")] public float cellSize = 0.5f; // Size of each 2D cell
    public float sampleRadius = 0.25f; // Radius for sampling walkable points
    public float minDistanceBetweenCells = 0.1f; // Minimum distance between cell edges

    [Header("NavMesh Area")] public int navMeshAreaMask = NavMesh.AllAreas;

    public List<GridData> gridCells = new List<GridData>();

    // Store previous values to detect changes
    [SerializeField] private float lastCellSize;
    [SerializeField] private float lastSampleRadius;
    [SerializeField] private float lastMinDistance;
    [SerializeField] private int lastNavMeshAreaMask;

    void OnEnable()
    {
        // Generate grid when component is added or enabled
        GenerateGridFromNavMesh();
    }

    void Update()
    {
        // Only run in Editor, not in Play Mode
        if (!Application.isPlaying)
        {
            // Check if any grid settings have changed
            if (cellSize != lastCellSize || sampleRadius != lastSampleRadius ||
                minDistanceBetweenCells != lastMinDistance || navMeshAreaMask != lastNavMeshAreaMask)
            {
                GenerateGridFromNavMesh();
                lastCellSize = cellSize;
                lastSampleRadius = sampleRadius;
                lastMinDistance = minDistanceBetweenCells;
                lastNavMeshAreaMask = navMeshAreaMask;
            }
        }
    }

    [ContextMenu("Generate Grid")] // Allows manual grid generation from context menu
    public void GenerateGridFromNavMesh()
    {
        gridCells.Clear();

        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        if (navMeshData.vertices.Length == 0)
        {
            Debug.LogWarning("No NavMesh data found!");
            return;
        }

        Bounds bounds = GetNavMeshBounds(navMeshData.vertices);

        // Sample 2D areas across the NavMesh
        for (float x = bounds.min.x; x < bounds.max.x; x += cellSize + minDistanceBetweenCells)
        {
            for (float z = bounds.min.z; z < bounds.max.z; z += cellSize + minDistanceBetweenCells)
            {
                Vector3 samplePos = new Vector3(x + cellSize / 2, bounds.center.y, z + cellSize / 2);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(samplePos, out hit, sampleRadius, navMeshAreaMask))
                {
                    Vector2Int gridPos = new Vector2Int(
                        Mathf.RoundToInt((x - bounds.min.x) / (cellSize + minDistanceBetweenCells)),
                        Mathf.RoundToInt((z - bounds.min.z) / (cellSize + minDistanceBetweenCells))
                    );
                    gridCells.Add(new GridData(gridPos, hit.position, cellSize));
                }
            }
        }

        Debug.Log($"Generated {gridCells.Count} 2D grid cells across NavMesh");
    }

    Bounds GetNavMeshBounds(Vector3[] vertices)
    {
        if (vertices.Length == 0) return new Bounds(Vector3.zero, Vector3.zero);
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
            // Draw wireframe cube for each cell
            Gizmos.DrawWireCube(cell.worldPosition, new Vector3(cell.cellSize, 0.1f, cell.cellSize));
        }
    }
}

[System.Serializable]
public struct GridData
{
    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    public float cellSize;

    public GridData(Vector2Int gridPosition, Vector3 worldPosition, float cellSize)
    {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;
        this.cellSize = cellSize;
    }
}