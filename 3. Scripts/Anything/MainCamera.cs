using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    CinemachineVirtualCamera  virtualCamera;

    [SerializeField] float smoothTime = 0.15f;
    private float targetSize;
    private float sizeVel;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        targetSize = virtualCamera.m_Lens.OrthographicSize;
    }


    void Update()
    {
        if(virtualCamera.Follow == null)
        {
            virtualCamera.Follow = PlayerManager.Instance.Player.transform;
        }

        if (PlayerManager.Instance.Player.InDungeon)
        {
            targetSize = 11f;
        }
        else
        {
            if(PlayerManager.Instance.Player.IsMoving)
                targetSize = 13f;
            else
                targetSize = 12f;
        }
        // ★ 추가: 현재값 → 목표값으로 부드럽게 이동
        virtualCamera.m_Lens.OrthographicSize =
            Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, targetSize, ref sizeVel, smoothTime);

        /*if (PlayerManager.Instance.Player.IsMoving)
        {
            Debug.Log("Player is moving");
            virtualCamera.m_Lens.OrthographicSize = 11f;
        }
        else
        {
            Debug.Log("Player is not moving");
            virtualCamera.m_Lens.OrthographicSize = 9f;
        }*/
    }
}
