using UnityEngine;
using Unity.Netcode;

public class MiniBall : NetworkBehaviour
{
    public float lifeTime = 5.0f; // Lifetime of the mini ball

    public void Initialize(float pushForce)
    {
        // Apply a random force for the miniball to move around
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Random.insideUnitSphere * pushForce, ForceMode.Impulse);
        }

        Destroy(gameObject, lifeTime);
    }
}
