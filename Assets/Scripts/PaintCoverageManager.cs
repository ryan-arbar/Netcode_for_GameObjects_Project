using Unity.Netcode;

public class PaintCoverageManager : NetworkBehaviour
{
    public NetworkVariable<int> blueCoverage = new NetworkVariable<int>(0);
    public NetworkVariable<int> yellowCoverage = new NetworkVariable<int>(0);

    // Assume total paintable area units for simplicity
    private int totalPaintableArea = 1000;

    public void IncrementPaintCoverage(Team team)
    {
        if (team == Team.Blue)
        {
            blueCoverage.Value += 1;
        }
        else if (team == Team.Yellow)
        {
            yellowCoverage.Value += 1;
        }
    }

    public float GetTeamCoveragePercentage(Team team)
    {
        if (team == Team.Blue)
        {
            return (float)blueCoverage.Value / totalPaintableArea * 100f;
        }
        else if (team == Team.Yellow)
        {
            return (float)yellowCoverage.Value / totalPaintableArea * 100f;
        }
        return 0f;
    }
}
