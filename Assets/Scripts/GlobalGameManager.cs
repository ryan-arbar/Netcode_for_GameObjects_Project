using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance;

    public GameObject blueTeamPrefab;
    public GameObject yellowTeamPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Get the Blue or Yellow player prefab
    public GameObject GetTeamPrefab(int teamId)
    {
        switch (teamId)
        {
            case 0: return blueTeamPrefab;
            case 1: return yellowTeamPrefab;
            default:
                Debug.LogError("Invalid team ID");
                return null;
        }
    }

    // Called from the UI when a team button is clicked
    public void ChooseTeam(int teamId)
    {
        ChooseTeamServerRpc(teamId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChooseTeamServerRpc(int teamId, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        // Spawn the new player object and update the dictionary
        GameObject newPlayerPrefab = GetTeamPrefab(teamId); // Get the prefab based on teamId (0 or 1)
        GameObject newPlayerObject = Instantiate(newPlayerPrefab);
        NetworkObject newPlayerNetObject = newPlayerObject.GetComponent<NetworkObject>();
        newPlayerNetObject.SpawnAsPlayerObject(clientId);
    }


    // Spawn the correct team prefab
    public void SpawnTeamPrefab(int teamId, ulong clientId)
    {
        Debug.Log($"Spawning team prefab for team ID: {teamId}, Client ID: {clientId}");
        GameObject prefabToSpawn = GetTeamPrefab(teamId);
        if (prefabToSpawn == null)
        {
            Debug.LogError("Failed to get team prefab. Is it assigned?");
            return;
        }

        GameObject newPlayerObj = Instantiate(prefabToSpawn);
        NetworkObject newPlayerNetObj = newPlayerObj.GetComponent<NetworkObject>();
        if (newPlayerNetObj == null)
        {
            Debug.LogError("Spawned object does not have a NetworkObject component.");
            return;
        }

        newPlayerNetObj.SpawnAsPlayerObject(clientId);
    }


}
