using UnityEngine;
using Unity.Netcode;

public class TeamSelection : NetworkBehaviour
{
    public NetworkVariable<Team> playerTeam = new NetworkVariable<Team>();

    public void ChooseTeam(Team team)
    {
        if (IsClient && IsOwner)
        {
            ChooseTeamServerRpc(team);
        }
    }

    [ServerRpc]
    void ChooseTeamServerRpc(Team team, ServerRpcParams rpcParams = default)
    {
        playerTeam.Value = team;
    }
}
