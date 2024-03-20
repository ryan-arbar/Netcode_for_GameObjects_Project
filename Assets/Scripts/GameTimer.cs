using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GameTimer : NetworkBehaviour
{
    public static GameTimer Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private AudioClip matchEndClip;
    private AudioSource audioSource;

    private float matchDuration = 60f;
    private float timeRemaining;
    private bool timerIsRunning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTimerServerRpc()
    {
        if (IsServer)
        {
            timeRemaining = matchDuration;
            timerIsRunning = true;
            UpdateTimerClientRpc(timeRemaining);
        }
    }

    [ClientRpc]
    public void UpdateTimerClientRpc(float time)
    {
        timeRemaining = time;
        UpdateTimerDisplay();
    }

    // The timer updates for all players in the game. When the timer reaches 0, the team with the
    // most paint on the floor is announced in the top left corner, replacing the timer
    private void Update()
    {
        if (IsServer && timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerClientRpc(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                DeclareWinnerServerRpc();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = $"{Mathf.FloorToInt(timeRemaining / 60)}:{Mathf.FloorToInt(timeRemaining % 60):00}";
    }

    [ServerRpc]
    private void DeclareWinnerServerRpc()
    {
        Team winner = CalculateWinner();
        DisplayWinnerClientRpc(winner);
    }

    // Check which team has more paint on the floor
    private Team CalculateWinner()
    {
        float blueScore = PaintCoverageTracker.Instance.CalculateCoveragePercentage(Team.Blue);
        float yellowScore = PaintCoverageTracker.Instance.CalculateCoveragePercentage(Team.Yellow);

        return blueScore > yellowScore ? Team.Blue : Team.Yellow;
    }

    [ClientRpc]
    private void DisplayWinnerClientRpc(Team winner)
    {
        timerText.text = $"Winner: {winner}";
        if (audioSource != null && matchEndClip != null)
        {
            audioSource.PlayOneShot(matchEndClip);
        }
    }
}
