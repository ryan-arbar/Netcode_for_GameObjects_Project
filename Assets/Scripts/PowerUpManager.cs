using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public Vector3 spawnAreaMin;
    public Vector3 spawnAreaMax;
    public int maxPowerUps = 3;

    private int currentPowerUps = 0;

    public static PowerUpManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnPowerUpsRoutine());
    }

    private IEnumerator SpawnPowerUpsRoutine()
    {
        while (true)
        {
            if (currentPowerUps < maxPowerUps)
            {
                SpawnPowerUp();
            }
            yield return new WaitForSeconds(5f); // Wait for 5 seconds before checking again
        }
    }

    private void SpawnPowerUp()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
        currentPowerUps++;
    }

    public void OnPowerUpCollected()
    {
        currentPowerUps--;
        Debug.Log($"Powerup collected. Current powerups: {currentPowerUps}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube((spawnAreaMin + spawnAreaMax) / 2, spawnAreaMax - spawnAreaMin);
    }
}
