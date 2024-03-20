using Unity.Netcode;
using UnityEngine;

public class PaintManagerNetworkHandler : NetworkBehaviour
{
    public static PaintManagerNetworkHandler Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Call this method from player movement or painting input
    public void RequestPaint(Vector3 position, Color color, float radius, float hardness)
    {
        PaintData data = new PaintData
        {
            position = position,
            color = color,
            radius = radius,
            hardness = hardness
        };
        SendPaintDataServerRpc(data);
    }

    [ServerRpc]
    public void SendPaintDataServerRpc(PaintData data)
    {
        ReceivePaintDataClientRpc(data);
    }

    [ClientRpc]
    public void ReceivePaintDataClientRpc(PaintData data)
    {
        PaintManager.instance.PaintFloor(data);
    }
}
