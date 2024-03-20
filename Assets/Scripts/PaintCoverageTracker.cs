using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PaintCoverageTracker : NetworkBehaviour
{
    public static PaintCoverageTracker Instance { get; private set; }

    public int gridSizeX = 100; // Adjust based on your plane size and desired granularity
    public int gridSizeZ = 100;
    private Team?[,] coverageGrid;

    public Slider controlSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        coverageGrid = new Team?[gridSizeX, gridSizeZ];
    }

    // Convert world position to grid coordinates
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPosition); // Convert to local position
                                                                           // Convert local position to a percentage of the grid
        float percentX = Mathf.Clamp01((localPos.x + transform.localScale.x * 0.5f) / transform.localScale.x);
        float percentZ = Mathf.Clamp01((localPos.z + transform.localScale.z * 0.5f) / transform.localScale.z);

        // Calculate grid position
        int x = Mathf.Clamp(Mathf.FloorToInt(gridSizeX * percentX), 0, gridSizeX - 1);
        int z = Mathf.Clamp(Mathf.FloorToInt(gridSizeZ * percentZ), 0, gridSizeZ - 1);

        return new Vector2Int(x, z);
    }


    public void PaintArea(Vector3 position, Team team)
    {
        // Debugging
        //Debug.Log($"Attempting to paint area. Team: {team}");

        Vector2Int gridPos = WorldToGridPosition(position);

        if (IsServer)
        {
            if (coverageGrid[gridPos.x, gridPos.y] != team)
            {
                // Update the grid with the new team color if the segment was not already painted by this team
                coverageGrid[gridPos.x, gridPos.y] = team;

                // Debugging
                //Debug.Log($"Grid position ({gridPos.x}, {gridPos.y}) painted by team {team}.");

                // Call a client RPC to update all clients about the paint change
                UpdateClientGridClientRpc(gridPos.x, gridPos.y, team);
            }
        }
        else
        {
            Debug.Log("PaintArea called, but not on server.");
        }
        UpdateControlSlider();
    }

    private void UpdateControlSlider()
    {
        float blueCoverage = CalculateCoveragePercentage(Team.Blue);
        float yellowCoverage = CalculateCoveragePercentage(Team.Yellow);
        float totalPaintedCoverage = blueCoverage + yellowCoverage;

        //Debug.Log($"Blue Coverage: {blueCoverage}% | Yellow Coverage: {yellowCoverage}% | Total Painted: {totalPaintedCoverage}%");

        // If there's no coverage, we set it to the neutral position
        if (totalPaintedCoverage == 0)
        {
            controlSlider.value = 0.5f;
        }
        else
        {
            // Calculate the ratio of blue coverage to the total painted coverage.
            // This will give us a value between 0 and 1 representing the slider position.
            float sliderValue = blueCoverage / totalPaintedCoverage;
            controlSlider.value = sliderValue;

            //Debug.Log($"Slider Value: {controlSlider.value}");
        }
    }


    [ClientRpc]
    private void UpdateClientGridClientRpc(int x, int z, Team team)
    {
        if (!IsServer) // On clients, update the local grid based on the server's instructions
        {
            coverageGrid[x, z] = team;
        }
    }

    public float CalculateCoveragePercentage(Team team)
    {
        int count = 0;
        foreach (var segment in coverageGrid)
        {
            if (segment == team)
            {
                count++;
            }
        }
        // Calculate percentage based on the total grid size
        return (float)count / (gridSizeX * gridSizeZ);
    }


    void OnDrawGizmos()
    {
        // Only draw Gizmos if the coverage grid has been initialized
        if (coverageGrid == null) return;

        // Calculate the real-world size of each grid cell based on the GameObject's scale
        Vector3 gridWorldSize = new Vector3(transform.localScale.x, 0.01f, transform.localScale.z);
        Vector3 cellSize = new Vector3(gridWorldSize.x / gridSizeX, 0.01f, gridWorldSize.z / gridSizeZ);

        // Set the color for the Gizmos (semi-transparent white)
        Gizmos.color = new Color(1f, 1f, 1f, 0.2f);

        // The starting point to draw the Gizmos is the bottom-left corner of the grid
        Vector3 gridBottomLeft = transform.position - (transform.right * gridWorldSize.x / 2) - (transform.forward * gridWorldSize.z / 2);

        // Draw the grid cells
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                // Calculate the world position of the bottom-left corner of each grid cell
                Vector3 cellBottomLeft = gridBottomLeft + (transform.right * (cellSize.x * x)) + (transform.forward * (cellSize.z * z));
                // Center of the cell
                Vector3 cellCenter = cellBottomLeft + (transform.right * cellSize.x / 2) + (transform.forward * cellSize.z / 2);

                // Draw the cell as a cube
                Gizmos.DrawWireCube(cellCenter, cellSize);
            }
        }
    }


}
