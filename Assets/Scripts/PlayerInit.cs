using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerInit : NetworkBehaviour
{
    public TeamData yellowTeamData;
    public TeamData blueTeamData;

    public Material playerMaterial;

    private TeamSelection teamSelection;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            teamSelection = GetComponent<TeamSelection>();
            UpdatePlayerColor(teamSelection.playerTeam.Value);
        }
    }

    void UpdatePlayerColor(Team team)
    {
        switch (team)
        {
            case Team.Yellow:
                playerMaterial.color = yellowTeamData.teamColor;
                break;
            case Team.Blue:
                playerMaterial.color = blueTeamData.teamColor;
                break;
        }
    }
}
