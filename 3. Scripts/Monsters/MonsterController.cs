using System;
using System.Collections;
using System.Threading;
using Assets.PixelFantasy.PixelMonsters.Common.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    private Monster _monster;
    private NavMeshAgent _agent;
    private Transform _player;
    public Animator Animator { get; private set; }
    public AudioClip attackSound;

    [SerializeField] private State _currentState;
    [field:SerializeField] public MonsterAnimationData AnimationData { get; private set; }

    [Header("현재 몬스터 데이터")] 
    public MonsterData currentData;
    public SpriteRenderer spriteRenderer;

    [Header("몬스터 감지 범위")] 
    public float detectionRange = GameConstants.Monster.DEFAULT_DETECTION_RANGE;
    public float attackRange = GameConstants.Monster.DEFAULT_ATTACK_RANGE;
    public Vector2 minPos = GameConstants.Monster.MONSTER_MIN_POS;
    public Vector2 maxPos = GameConstants.Monster.MONSTER_MAX_POS;

    public IdleState IdleState { get; private set; }
    public AttackState AttackState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public FleeState FleeState { get; private set; }
    public PatrolState PatrolState { get; private set; }
    public DieState DieState { get; private set; }

    private void Awake()
    {
        _monster = GetComponent<Monster>();
        _agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();        

        if (AnimationData == null)
            AnimationData = new MonsterAnimationData();
        AnimationData.Initialize();
        
        // NavmeshAgent2D 설정
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        IdleState = new IdleState(this);
        AttackState = new AttackState(this);
        ChaseState = new ChaseState(this);
        FleeState = new FleeState(this);
        PatrolState = new PatrolState(this);
        DieState = new DieState(this);        
    }

    private void Start()
    {
        _player = PlayerManager.Instance.Player.transform;
        currentData = _monster.Condition.monsterData;
        
        // 초기 상태를 Idle 상태로 설정
        ChangeState(IdleState);        
    }      

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.Execute();
        }
    }

    public void ChangeState(State newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }
        
        _currentState = newState;
        _currentState.Enter();
    }

    public void OnDamaged()
    {
        _currentState.StartAnimation(AnimationData.HitParameterHash);
        // 공격할 수 없는 상태이거나, 이미 공격/도망 상태라면 아무것도 하지 않음
        if (_currentState is AttackState || _currentState is FleeState) return;

        // MonsterCondition의 canAttack 값을 함께 고려
        var condition = GetComponent<Monster>().Condition;        
        if (!condition.canAttack)
        {
            ChangeState(new FleeState(this));
            return;
        }

        // MonsterData의 행동 타입에 따라 상태 분기
        switch (currentData.behaviorType)
        {
            case MonsterBehaviorType.Aggressive:
                // 공격형 몬스터는 플레이어를 추격/공격
                ChangeState(new ChaseState(this));
                break;
            case MonsterBehaviorType.Cowardly:
                // 겁쟁이 몬스터는 도망
                ChangeState(new FleeState(this));
                break;
        }
        _currentState.StopAnimation(AnimationData.HitParameterHash);
    }       
    #region 상태들이 사용할 기능들

    public MonsterData GetMonsterData()
    {
        return currentData;
    }

    public void StopMoving()
    {
        if (_agent != null && _agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
    }
    
    public void MoveTo(Vector3 destination)
    {
        if (_agent != null && _agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }
    }

    public bool IsPlayerInDetectionRange()
    {
        if (_player == null) return false;
        return Vector3.Distance(transform.position, _player.position) <= detectionRange;
    }

    public bool IsPlayerInAttackRange()
    {
        if (_player == null) return false;
        return Vector3.Distance(transform.position, _player.position) <= attackRange;
    }

    public Vector3 GetPlayerPosition()
    {
        if (_player != null)
        {
            return _player.position;
        }
        return transform.position; // 플레이어가 없으면 제자리
    }

    // 이외에 Die, Hit 등의 상태 변경 함수 추가 가능...

    public void InitState()
    {
        // rushGauge.enabled = true;
        // isCharging = false;
        // _rushCoroutine = null;
        // rushGauge.fillAmount = 0;
        // rushAlert.transform.localScale = new Vector3(0.5f, 0f, 0.5f);
        // rushAlert.SetActive(false);
        ChangeState(IdleState);        

        // InitState 메서드는 몬스터가 스폰되고 디스폰될 때 오브젝트 풀링을 거치면서
        // 돌진 경고 등을 초기화 해야함
        // 필요 시 수정 또는 메서드 삭제 해주시면 됩니다.
    }

    public void ChangeToDieState()
    {
        if(_currentState == DieState) return; // 이미 Die 상태라면 무시        
        ChangeState(DieState);

        // ChangeToDieState 메서드는 몬스터가 죽을 때
        // 돌진, 네브매쉬 등을 설정하는 메서드
        // 필요 시 수정 또는 메서드 삭제 해주시면 됩니다.
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player") && _currentState == AttackState)
    //    {
    //        // 플레이어에게 데미지 전달
    //        PlayerManager.Instance.Player.GetComponent<PlayerCondition>().GetDamage(GetMonsterData().monsterAttack);
    //       // Debug.Log($"플레이어에게 {GetMonsterData().monsterAttack} 데미지 전달!");
    //    }
    //}

    public void PlayerAttack()
    {
        //만약에 플레이어가 공격 범위 밖에있으면 리턴
        if(!IsPlayerInAttackRange()) return;
        PlayerManager.Instance.Player.GetComponent<PlayerCondition>().GetDamage(GetMonsterData().monsterAttack);
        PlayerManager.Instance.Player.GetComponent<PlayerCondition>().GetLastAttackedMonsterID(GetMonsterData().monsterID);
        AudioManager.Instance.PlaySFX(attackSound);
    }
    #endregion
}
