using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public PlayerController playerController;
    [SerializeField] private GameObject weapon;

    private GameObject character;
    public Camera mainCamera;
    public PlayerCondition playerCondition;
    public PlayerInventory playerInventory;
    public Rigidbody2D rb;
    public bool InDungeon;
    public bool IsMoving;
    public Animator animator;
    public PlayerStateMachine playerStateMachine;
    public Image HPbar;
    public Image HpBg;
    public Weapon weaponScript;
    public int rotationSpeed;
    public bool isCameraMovedInTutorial = false;

    public float moveSpeed;

    public GameObject Weapon
    {
        get { return weapon; }
    }

    public void Awake()
    {
        PlayerManager.Instance.Player = this;
        
        playerCondition = GetComponent<PlayerCondition>();
        playerInventory = GetComponent<PlayerInventory>();
        rb = GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        
        character = transform.GetChild(0).gameObject;
        weapon = transform.GetChild(1).gameObject;
        
        weaponScript = weapon.GetComponent<Weapon>();
        animator = character.GetComponent<Animator>();
        
        playerStateMachine = new PlayerStateMachine(this);
    }

    private void Start()
    {
        moveSpeed = playerCondition.WalkSpeed;
        // QuestManager.Instance.player = this;
        playerController = UIManager.Instance.GetUI<JoystickUI>().playerController;
        StartCoroutine(playerCondition.AutoHeal());
        
        if (SaveLoadManager.Instance.isClickedContinue || SceneManager.GetActiveScene().name == GameConstants.SceneNames.SECOND_MAIN_GAME_SCENE)
        {
            LoadPlayerData();
        }
    }

    private void Update()
    {
        if (playerCondition.Health == playerCondition.MaxHealth)
        {
            HpBg.enabled = false;
            HPbar.enabled = false;
        }
        else
        {
            HpBg.enabled = true;
            HPbar.enabled = true;
        }

        if (playerStateMachine.currentState == playerStateMachine.PlayerDieState)
        { 
            return; 
        }
        weapon.SetActive(InDungeon);

        float baseMoveSpeed = InDungeon ? GameConstants.Player.DUNGEON_MOVE_SPEED : playerCondition.totalWalkSpeed / 10 ;
        //조이스틱 핸들러 벡터값을 받아와서 플레이어 이동
        Vector3 move = new Vector3(playerController.Horizontal(), playerController.Vertical(), 0);
        rb.MovePosition(transform.position + move * baseMoveSpeed * Time.fixedDeltaTime);

        //플레이어 좌우 반전
        if (move.x > 0)
        {
            character.transform.localScale = GameConstants.Player.PLAYER_SCALE_RIGHT;
        }
        else if (move.x < 0)
        {
            character.transform.localScale = GameConstants.Player.PLAYER_SCALE_LEFT;
        }

        //State 변경 및 무기 회전
        if (move != Vector3.zero)
        {
            if (playerStateMachine.currentState == playerStateMachine.PlayerDieState)
                return;
            
            IsMoving = true;
            playerStateMachine.ChangeState(playerStateMachine.PlayerMoveState);
            float angle = Vector2.SignedAngle(Vector3.right, move);
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation,
                                        targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            if (playerStateMachine.currentState == playerStateMachine.PlayerDieState)
                return;
            
            IsMoving = false;
            playerStateMachine.ChangeState(playerStateMachine.PlayerIdleState);
        }

        var loadingUI = UIManager.Instance.IsExistUI<LoadingUI>() ? UIManager.Instance.GetUI<LoadingUI>() : null;
        bool isLoadingVisible = loadingUI != null && loadingUI.gameObject.activeInHierarchy;
        
        // 개발을 위해 모바일 터치를 마우스 클릭으로 변경
        if (Input.GetMouseButtonDown(0) && UIManager.Instance.isUIOn == false && isCameraMovedInTutorial == false && isLoadingVisible == false)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            playerController.joystickBG.gameObject.SetActive(true);
            playerController.OnClickDown(eventData);
        }
    }
    
    private void LoadPlayerData()
    {
        var data = SaveLoadManager.Instance.saveData;
        if (data == null)
        {
            Debug.LogError("세이브 데이터가 존재하지 않습니다.");
            return;
        }

        if (BigInteger.TryParse(data.playerMoney, out BigInteger money))
        {
            // playerInventory.Money = money;
            playerInventory.InitForLoading(money);
        }
        playerInventory.diamond = data.playerDiamond;
        
        playerInventory.itemsDic.Clear();
        foreach (var item in data.itemCounts)
        {
            string itemID = item.Key;
            int itemCount = item.Value;

            // Resources 폴더에서 아이템 불러오기
            ItemSO itemSO = Resources.Load<ItemSO>($"{GameConstants.Paths.ITEM_DATA_PATH}{itemID}");

            if (itemSO != null)
            {
                playerInventory.itemsDic[itemSO] = itemCount;
            }
            else
            {
                Debug.LogWarning($"[로드 실패] 아이템 {itemID}를 Resources/{GameConstants.Paths.ITEM_DATA_PATH} 폴더에서 찾을 수 없습니다.");
            }
        }
    }   
}
