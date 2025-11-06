using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class PetControl : MonoBehaviour
{
    private NavMeshAgent _agent;   
    
    public GameObject PetPrefab;
    public PetSO petSO;

    public Animator Animator { get; private set; }
    [field: SerializeField] public MonsterAnimationData AnimationData { get; private set; }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        if (AnimationData == null)
            AnimationData = new MonsterAnimationData();
        AnimationData.Initialize();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }
    private void Update()
    {      
        FollowPlayer();
        UpdateAnimation();
        CheckTime();
    }
    private void FollowPlayer()
    {       
        Vector3 targetPos = PlayerManager.Instance.Player.transform.position + petSO.PetOffset;
        _agent.SetDestination(targetPos);         
    }
    private void UpdateAnimation()
    {
        bool isMoving = _agent.velocity.magnitude > 0.1f;

        Animator.SetBool(AnimationData.RunParameterHash, isMoving);
        Animator.SetBool(AnimationData.IdleParameterHash, !isMoving);
    }

    private void CheckTime()
    {
        if (petSO.StartTime == DateTime.MinValue) return;
        double elapsed = (DateTime.Now - petSO.StartTime).TotalSeconds; // 경과 시간
        double remaining = petSO.DurationTime + petSO.ExtraTime - elapsed;    // 남은 시간
        if (remaining <= 0)
        {         
            Destroy(this.gameObject);
            petSO.StartTime = DateTime.MinValue;               
            PetDeActive();            
            return;
        }
    }
    public void PetDeActive()
    {
        var Player = PlayerManager.Instance.Player;
        Player.moveSpeed -= petSO.PetSpeed;
        Player.playerInventory.MaxQuantity -= petSO.PetInven;
    }
}
