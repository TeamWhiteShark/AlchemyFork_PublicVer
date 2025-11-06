using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum SlotState
{
    BeforeAccept,
    InProgress,
    Empty,
    Waiting,
    Complete
}

public class QuestSlot : MonoBehaviour
{
    private const string ItemPath = GameConstants.Paths.ITEM_DATA_PATH;
    private const string MonsterPath = GameConstants.Paths.MONSTER_DATA_PATH;

    [Header("슬롯 내부 데이터")] 
    public QuestUI questUI;
    public List<GameObject> blocks = new List<GameObject>();
    public Quest quest;
    public SlotState currentState;
    private float waitingTimer = 30f;
    private float curTimer = 0f;
    
    [Header("슬롯 오브젝트들")]
    [SerializeField] private GameObject beforeAcceptBlock;
    [SerializeField] private GameObject inProgressBlock;
    [SerializeField] private GameObject emptyBlock;
    [SerializeField] private GameObject waitingBlock;
    [SerializeField] private GameObject completeBlock;

    [Header("대기 상태 블럭 관련 오브젝트들")] 
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI difficultyText;

    [Header("수락 전 상태 블럭 관련 오브젝트들")] 
    [SerializeField] private Image targetImage;
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private List<GameObject> difficultyStars = new List<GameObject>();

    [Header("진행 중 상태 블럭 관련 오브젝트들")] 
    [SerializeField] private Image targetImageInProgress;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private List<GameObject> difficultyStarsInProgress = new List<GameObject>();
    [SerializeField] private Animator loadingAnimator;

    [Header("완료 상태 블럭 관련 오브젝트들")] 
    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private List<GameObject> difficultyStarsInComplete = new List<GameObject>();
    
    private void Start()
    {
        foreach (Transform child in transform)
        {
            blocks.Add(child.gameObject);
        }

        quest = null;
    }

    private void Update()
    {
        if (currentState == SlotState.Waiting)
        {
            WaitingTimeCheck();
        }
    }

    private void WaitingTimeCheck()
    {
        if (quest == null || quest.questData == null)
        {
            currentState = SlotState.Empty;
            SwapBlocks();
            return;
        }
        
        curTimer += Time.deltaTime;
        float remainTime = Mathf.Floor(waitingTimer - curTimer);

        if (remainTime < 0)
        {
            remainTime = 0;
            ChangeToBeforeAcceptState();
        }

        int minutes = (int)(remainTime / 60);
        int seconds = (int)(remainTime % 60);
        
        timerText.text = minutes.ToString() + ":" + seconds.ToString();
        difficultyText.text = "X " + quest.questData.difficulty.ToString();
    }
    
    private void SwapBlocks()
    {
        foreach (var block in blocks)
        {
            block.SetActive(false);
        }

        switch (currentState)
        {
            case SlotState.BeforeAccept:
                beforeAcceptBlock.SetActive(true);
                break;
            case SlotState.InProgress:
                inProgressBlock.SetActive(true);
                break;
            case SlotState.Empty:
                emptyBlock.SetActive(true);
                break;
            case SlotState.Waiting:
                waitingBlock.SetActive(true);
                break;
            case SlotState.Complete:
                completeBlock.SetActive(true);
                break;
        }
    }
    
    public void ChangeToWaitingState()
    {
        currentState = SlotState.Waiting;
        curTimer = 0f;
        SwapBlocks();
    }

    private void ChangeToBeforeAcceptState()
    {
        currentState = SlotState.BeforeAccept;
        SwapBlocks();

        if (quest.questData.targetObjectID >= 1000 && quest.questData.targetObjectID < 3000)
        {
            var itemData = Resources.Load<ItemSO>(ItemPath + quest.questData.targetObjectID);

            targetImage.sprite = itemData.itemSprite;
            targetImageInProgress.sprite = itemData.itemSprite;
        }
        else if (quest.questData.targetObjectID >= 3000 && quest.questData.targetObjectID < 5000)
        {
            var monsterData = Resources.Load<MonsterData>(MonsterPath + quest.questData.targetObjectID);
            
            targetImage.sprite = monsterData.sprite;
            targetImageInProgress.sprite = monsterData.sprite;
        }
        
        questTitle.text = quest.questData.questTitle;

        for (int i = 0; i < difficultyStars.Count; i++)
        {
            difficultyStars[i].SetActive(false);
            difficultyStarsInProgress[i].SetActive(false);
            difficultyStarsInComplete[i].SetActive(false);
        }

        for (int i = 0; i < quest.questData.difficulty; i++)
        {
            difficultyStars[i].SetActive(true);
            difficultyStarsInProgress[i].SetActive(true);
            difficultyStarsInComplete[i].SetActive(true);
        }
    }

    public void OnClickAcceptButton()
    {
        quest.currentState = QuestState.InProgress;
        currentState = SlotState.InProgress;
        SwapBlocks();

        UpdateInProgressUI();
    }

    public void OnClickQuestRefuseButton()
    {
        ResetSlot();
    }

    public void UpdateInProgressUI()
    {
        progressText.text = quest.currentValue + " / " + quest.questData.targetValue;
    }

    public void ChangeToCompleteState()
    {
        Analytics.AddEvent("quest_completed_count", new Dictionary<string, object>
        {
            { "quest_id", quest.questData.questID },
        });
        SwapBlocks();
        
        var rewardData = Resources.Load<ItemSO>(ItemPath + quest.questData.rewardObjectID);

        rewardImage.sprite = rewardData.itemSprite;
        rewardText.text = "X " + quest.questData.rewardValue;
    }

    public void OnClickGetRewardButton()
    {
        QuestManager.Instance.player.playerInventory.GetRewardToPlayer(quest.questData.rewardObjectID, quest.questData.rewardValue);
        ResetSlot();
    }

    private void ResetSlot()
    {
        quest = null;
        currentState = SlotState.Empty;
        SwapBlocks();

        questUI.DropEmptyBlock(this);
    }
}
