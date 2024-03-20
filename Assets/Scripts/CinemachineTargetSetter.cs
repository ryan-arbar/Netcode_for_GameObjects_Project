using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CinemachineTargetSetter : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    public Transform cameraTarget;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.LocalClientId == clientId)
        {
            SetCameraTarget();
        }
    }

    private void SetCameraTarget()
    {
        GameObject playerObject = GetLocalPlayerObject();
        if (playerObject != null && cameraTarget != null)
        {
            // Instead of directly following the player, the camera will follow the "PlayerFollowCamera" object
            cameraTarget.position = playerObject.transform.position;
            virtualCamera.Follow = cameraTarget;
        }
    }

    private GameObject GetLocalPlayerObject()
    {
        foreach (NetworkObject networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
        {
            if (networkObject.IsLocalPlayer)
            {
                return networkObject.gameObject;
            }
        }
        return null;
    }
}
