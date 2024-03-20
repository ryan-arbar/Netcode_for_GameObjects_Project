using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameStarter : NetworkBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private GameObject objectToToggle;
    [SerializeField] private AudioClip startSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        startGameButton.gameObject.SetActive(true);
        startGameButton.onClick.AddListener(OnStartGameButtonPressed);
    }

    private void OnStartGameButtonPressed()
    {
        // Play the start game sound effect
        PlayStartSound();

        // Only the host can start the game
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            ToggleObjectActiveStateServerRpc(!objectToToggle.activeSelf);
            GameTimer.Instance?.StartTimerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleObjectActiveStateServerRpc(bool newState)
    {
        ToggleObjectActiveStateClientRpc(newState);
    }

    [ClientRpc]
    private void ToggleObjectActiveStateClientRpc(bool newState)
    {
        objectToToggle.SetActive(newState);
    }

    private void PlayStartSound()
    {
        if (startSound != null)
        {
            audioSource.PlayOneShot(startSound);
        }
        else
        {
            Debug.LogWarning("Start sound clip not assigned.");
        }
    }
}
