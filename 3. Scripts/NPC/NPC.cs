using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public enum NPCType
{
    Customer,
    Chef,
    Waiter,
    Cashier,
    Hunter,
    None
}

public enum ObjType
{
    Meat,
    Mushroom,
}

public class NPC : MonoBehaviour, IPoolable
{
    public HunterWeapon hunterWeapon;
    public float time;
    public float speed;
    public NPCStateMachine stateMachine;
    public NPCType npcType;
    public NPCInven npcInven;
    public Transform body;
    public string NPCState;
    public ObjType objType;

    [SerializeField] public bool satisfied;
    
    public ItemSO targetItem;
    public int targetCount;
    public ArchType targetArchType;

    [field: SerializeField] public NPCAnimationData NpcAnimationData { get; private set; }
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private Vector3 moving;

    [Header("InventoryInfo")]
    public CustomerInven customerInven;
    public bool calculateEnd;
    public ItemSO homeItem;
    public ItemSO awayItem;
    
    [Header("MoveInfo")]
    public GameObject targetObj;
    public NavMeshAgent agent;
    public NavMeshPath path;
    public float distance;
    public MonsterData targetMonsterData;
    
    public bool anger;
    public float waitTime;
    public Coroutine waitCoroutine;
    
    protected Action<GameObject> returnToPool;
    public event Action<GameObject> OnBeforeReturn;

    protected virtual void Awake()
    {        
        animator = GetComponent<Animator>();
        
        if (NpcAnimationData == null)
            NpcAnimationData = new NPCAnimationData();
        
        stateMachine = new NPCStateMachine(this, SetStrategy(npcType));
        
        NpcAnimationData.Initialize();
    }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        if (npcType == NPCType.Customer && agent.Warp(CustomerManager.Instance._spawnPosition))
            path =  new NavMeshPath();
        if (npcType != NPCType.Customer)
        {
            if(agent.Warp(NPCManager.Instance.waitingPoint.transform.position))
                path = new NavMeshPath();
            else
                Debug.LogError("[NPC] Agent waiting point not found");
        }
        

        targetItem = homeItem;
        stateMachine.ChangeState(stateMachine.NPCIdleState);
    }
    
    protected virtual void Update()
    {
        if (transform.rotation != Quaternion.Euler(Vector3.zero))
        {
            Debug.Log("버그발생");
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        
        stateMachine?.Execute();
        
        if (npcType != NPCType.Hunter && npcType != NPCType.Cashier)
        {
            moving = agent.desiredVelocity;
            body.localScale = moving.x <= 0 ? GameConstants.NPC.NPC_SCALE_LEFT : GameConstants.NPC.NPC_SCALE_RIGHT;
        }
        else if (npcType == NPCType.Cashier)
        {
            moving = agent.desiredVelocity;
            body.localScale = moving.x < 0 ? GameConstants.NPC.NPC_SCALE_LEFT : GameConstants.NPC.NPC_SCALE_RIGHT;
        }
    }

    private Strategy SetStrategy(NPCType npcType)
    {
        switch (npcType)
        {
            case NPCType.Customer:
                return new Strategy
                {
                    Find = new CustomerFind(),
                    Move = new NPCMove(),
                    Wait = new CustomerWait(),
                    Interact = new CustomerInteract(),
                };
            case NPCType.Chef:
                return new Strategy
                {
                    Find = new ChefFind(),
                    Move = new NPCMove(),
                    Wait = new ChefWait(),
                    Interact = new ChefInteract(),
                };
            case NPCType.Waiter:
                return new Strategy
                {
                    Find = new WaiterFind(),
                    Move = new WaiterMove(),
                    Wait = new WaiterWait(),
                    Interact = new WaiterInteract(),
                };
            case NPCType.Cashier:
                return new Strategy
                {
                    Find = new CashierFind(),
                    Move = new NPCMove(),
                    Wait = new CashierWait(),
                    Interact = new CashierInteract(),
                };
            case NPCType.Hunter:
                return new Strategy
                {
                    Find = new HunterFind(),
                    Move =  new HunterMove(),
                    Wait = new HunterWait(),
                    Interact = new HunterInteract(),
                };
            default:
                return null;
        }
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        anger = true;
    }

    public void Initialize(Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        anger = false;
        calculateEnd = false;
        if(npcType == NPCType.Customer)
        {
            customerInven.itemCount = 0;
            targetItem = homeItem;
            targetObj = null;
            satisfied = false;
            targetArchType = ArchType.Stand;
        }
        stateMachine.ChangeState(stateMachine.NPCIdleState);
    }

    public void OnDespawn()
    {
        OnBeforeReturn?.Invoke(gameObject);
        returnToPool?.Invoke(gameObject);
    }
}
