using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : UIBase
{
    public override bool isDestroy => false;
    
    [Header("UI 내부 오브젝트들")]
    public Transform questContents;
    public CanvasGroup canvasGroup;

    [Header("UI 관련 데이터들")] 
    public bool isOpen = false;
    public List<QuestSlot> slots = new List<QuestSlot>();
    public int slotCount = 0;

    private void Awake()
    {
        foreach (Transform child in questContents)
        {
            slots.Add(child.GetComponent<QuestSlot>());
            if (child.TryGetComponent(out QuestSlot questSlot))
            {
                questSlot.questUI = this;
            }
        }
        
        canvasGroup = GetComponent<CanvasGroup>();
        
        Debug.Log("12345");
    }

    private void Start()
    {
        QuestManager.Instance.questUI = this;
    }

    private void Update()
    {
        CheckSlots();
    }

    public override void OpenUI()
    {
        isOpen = true;
        UIManager.Instance.isUIOn = true;
        ToggleUI();
    }

    public override void CloseUI()
    {
        isOpen = false;
        UIManager.Instance.isUIOn = false;
        ToggleUI();
    }

    private void ToggleUI()
    {
        if (isOpen)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void CheckSlots()
    {
        //if (QuestManager.Instance.QuestDic.Count <= slotCount) return;
        
        foreach (var slot in slots)
        {
            if (slot.currentState == SlotState.Waiting)
            {
                break;
            }
            
            if (slot.currentState == SlotState.Empty)
            {
                // 퀘스트 슬롯을 대기 상태로 바꾸고
                // 퀘스트 슬롯에 퀘스트 정보를 넘겨준다
                slot.quest = QuestManager.Instance.GetRandomQuest(slot);
                if(slot.quest.questData != null)
                {
                    slot.ChangeToWaitingState();
                }
                break;
            }
        }
    }

    public void OnClickRefuseButton()
    {
        CloseUI();
    }

    public void DropEmptyBlock(QuestSlot emptySlot)
    {
        // Hierarchy 순서를 마지막으로 이동
        emptySlot.transform.SetSiblingIndex(slots.Count - 1);

        // 리스트 첫번째에서 제거하고 마지막으로 추가
        slots.Remove(emptySlot);
        slots.Add(emptySlot);
    }
}
