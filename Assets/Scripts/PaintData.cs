using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct PaintData : INetworkSerializable
{
    public Vector3 position;
    public Color color;
    public float radius;
    public float hardness;
    public float strength;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref color);
        serializer.SerializeValue(ref radius);
        serializer.SerializeValue(ref hardness);
        serializer.SerializeValue(ref strength);
    }
}
