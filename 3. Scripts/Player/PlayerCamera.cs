/*using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

// Renamed the class to avoid conflict with UnityEngine.Camera
public class PlayerCamera : MonoBehaviour
{
    //private PlayerManager playerManager;
    //private Camera mainCamera; // UnityEngine.Camera
    private CinemachineVirtualCamera virtualCamera;

    void Awake()
    {
        //playerManager = PlayerManager.Instance;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (PlayerManager.Instance.Player.IsMoving)
        {
            Debug.Log("Player is moving");
            virtualCamera.m_Lens.OrthographicSize = 30f;
        }
        else
        {
            Debug.Log("Player is not moving");
            virtualCamera.m_Lens.OrthographicSize = 3f;
        }
    }
}*/
