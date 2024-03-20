using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;

    // Movement variables
    public float moveSpeed = 5.0f;
    public float maxVelocity = 10f;
    public float acceleration = 60f;
    public float deceleration = 40f;
    public float airControlStrength = 2f;
    public float jumpForce = 7f;
    public float pushForce = 5.0f;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.5f;

    public PlayerAudio playerAudio;
    private AudioSource rollingAudioSource;
    public AudioClip rollingClip;
    public float volumeScale = 0.5f;
    public float pitchScale = 0.1f;

    public Team team;

    private void Start()
    {
        rollingAudioSource = gameObject.AddComponent<AudioSource>();
        rollingAudioSource.clip = rollingClip;
        rollingAudioSource.loop = true;
        rollingAudioSource.playOnAwake = false;
    }

    // Only control local client ball
    void FixedUpdate()
    {
        if (!IsOwner || !Application.isFocused) return;

        bool isGrounded = IsGrounded();

        if (IsGrounded())
        {
            HandlePhysicsMovement();
        }
        else
        {
            HandleAirControl();
        }

        ManageRollingSound(isGrounded);
    }

    // Plays rolling sound only if player is above certain velocity on the ground
    private void ManageRollingSound(bool isGrounded)
    {
        if (isGrounded && ballRigidbody.velocity.magnitude > 0.1f)
        {
            float velocityRatio = ballRigidbody.velocity.magnitude / maxVelocity;
            playerAudio.PlayRollingSound(velocityRatio);
        }
        else
        {
            playerAudio.StopRollingSound();
        }
    }

    public void ChooseTeam(int teamId)
    {
        CmdChooseTeamServerRpc(teamId);
    }

    // Server handling team selection and spawning
    [ServerRpc(RequireOwnership = false)]
    public void CmdChooseTeamServerRpc(int teamId, ServerRpcParams rpcParams = default)
    {
        GlobalGameManager.Instance.SpawnTeamPrefab(teamId, OwnerClientId);
    }

    // Camera follows ball prefab being controlled by local client
    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            CameraFollowTarget cameraFollow = FindObjectOfType<CameraFollowTarget>();
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(gameObject);
            }
        }
    }

    // WASD movement for ball
    private void HandlePhysicsMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")).normalized;
        Vector3 force = input * acceleration - ballRigidbody.velocity * deceleration;
        force.y = 0; // Do not affect vertical movement

        ballRigidbody.AddForce(force, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(ballRigidbody.velocity.x, 0, ballRigidbody.velocity.z);
        if (horizontalVelocity.magnitude > maxVelocity)
        {
            ballRigidbody.velocity = horizontalVelocity.normalized * maxVelocity + Vector3.up * ballRigidbody.velocity.y;
        }
    }

    // Give player a little control of movement while in air
    private void HandleAirControl()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        Vector3 force = input * airControlStrength;
        force.y = 0; // Maintain current vertical velocity

        ballRigidbody.AddForce(force, ForceMode.Acceleration);
    }

    void Update()
    {
        if (IsOwner && Input.GetButtonDown("Jump") && IsGrounded())
        {
            ballRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAudio.PlayJumpSound();
        }
    }

    // Ground logic for detecting the ground for jumping
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundCheckDistance, groundLayer);
    }

    // When players collide with each other, there is a small force that knocks them back
    // (Does not work very well, but I'm leaving it)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 direction = collision.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                otherRigidbody.AddForce(direction * pushForce, ForceMode.Impulse);
            }
        }
    }
}
