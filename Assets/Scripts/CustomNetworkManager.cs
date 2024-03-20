using Unity.Netcode;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public GameObject blueTeamPrefab;
    public GameObject yellowTeamPrefab;

    // Call this method to spawn the player with the selected team prefab.
    public void SpawnPlayerWithTeam(ulong clientId, int teamId)
    {
        GameObject playerPrefab = teamId == 0 ? blueTeamPrefab : yellowTeamPrefab;
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned.");
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefab);
        NetworkObject playerNetworkObject = playerInstance.GetComponent<NetworkObject>();

        if (playerNetworkObject != null)
        {
            playerNetworkObject.SpawnAsPlayerObject(clientId);
        }
        else
        {
            Debug.LogError("Spawned player prefab does not have a NetworkObject component.");
        }
    }
}
