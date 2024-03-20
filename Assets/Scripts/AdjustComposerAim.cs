using Cinemachine;
using UnityEngine;

public class AdjustComposerAim : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Priority = 11;
    }
}
