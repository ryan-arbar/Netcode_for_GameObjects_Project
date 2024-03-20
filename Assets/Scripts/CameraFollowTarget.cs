using UnityEngine;
using Unity.Netcode;

public class CameraFollowTarget : MonoBehaviour
{
    private Transform playerTransform;

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }

    public void SetTarget(GameObject player)
    {
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log($"Camera target set to {player.name}");
        }
    }
}
