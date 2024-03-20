using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TeamSelectionUI : MonoBehaviour
{
    public Button blueTeamButton;
    public Button yellowTeamButton;

    [SerializeField] private AudioClip blueTeamSound;
    [SerializeField] private AudioClip yellowTeamSound;

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
        blueTeamButton.onClick.AddListener(() => OnTeamButtonClicked(0)); // 0 for blue
        yellowTeamButton.onClick.AddListener(() => OnTeamButtonClicked(1)); // 1 for yellow
    }

    public void OnTeamButtonClicked(int teamId)
    {
        // Play the team selection sound effect
        PlayTeamSelectionSound(teamId);

        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SendTeamChoiceToServer(teamId);
        }
        else
        {
            SendTeamChoiceToServer(teamId);
        }

        // Disable both buttons after a choice is made
        blueTeamButton.interactable = false;
        yellowTeamButton.interactable = false;
    }

    private void SendTeamChoiceToServer(int teamId)
    {
        var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>();
        if (localPlayer != null)
        {
            localPlayer.CmdChooseTeamServerRpc(teamId);
        }
    }

    private void PlayTeamSelectionSound(int teamId)
    {
        AudioClip clipToPlay = teamId == 0 ? blueTeamSound : yellowTeamSound;
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning("Team selection sound clip not assigned.");
        }
    }
}
