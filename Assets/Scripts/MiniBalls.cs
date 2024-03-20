using UnityEngine;
using Unity.Netcode;

public class MiniBalls : NetworkBehaviour
{
    public GameObject miniBallPrefabBlue;
    public GameObject miniBallPrefabYellow;
    public int miniBallCount = 5;
    public float pushForce = 5.0f;

    private AudioSource audioSource;

    [SerializeField] private AudioClip pickUpSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsServer)
        {
            var playerNetwork = other.GetComponent<PlayerNetwork>();
            if (playerNetwork != null)
            {
                // Determine the correct prefab based on the team of the player who triggered the power-up
                GameObject prefabToSpawn = playerNetwork.team == Team.Blue ? miniBallPrefabBlue : miniBallPrefabYellow;

                // Spawn the mini balls
                SpawnMiniBalls(prefabToSpawn, transform.position, miniBallCount, pushForce);

                // Play the pickup sound for all clients
                PlayPowerUpSoundClientRpc();

                // Despawn this power-up object
                NetworkObject networkObject = GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Despawn();
                }
            }
        }
    }

    private void SpawnMiniBalls(GameObject prefab, Vector3 position, int count, float force)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject miniBall = Instantiate(prefab, position + Random.insideUnitSphere * 0.5f, Quaternion.identity);
            NetworkObject netObj = miniBall.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn();
                MiniBall miniBallScript = miniBall.GetComponent<MiniBall>();
                if (miniBallScript != null)
                {
                    miniBallScript.Initialize(force);
                }
            }
        }
    }

    [ClientRpc]
    private void PlayPowerUpSoundClientRpc()
    {
        // Ensure audioSource is not null and play clip at the power-up's position
        if (audioSource != null)
        {
            audioSource.PlayOneShot(pickUpSound);
        }
    }
}
