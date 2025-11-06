using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestManager : MonoSingleton<QuestManager>
{
    protected override bool isDestroy => false;

    private const string QuestPath = GameConstants.Paths.QUEST_DATA_PATH;
    
    // 퀘스트 SO 원본 데이터
    public Dictionary<int, QuestSO> QuestDic = new Dictionary<int, QuestSO>();

    public QuestUI questUI;
    public Player player;

    protected override void Awake()
    {
        base.Awake();

        InitQuest();
        
        Debug.Log("123");
    }

    public void AddQuest(QuestSO[] quests)
    {
        foreach (var quest in quests)
        {
            if (!(QuestDic.ContainsKey(quest.questID)))
            {
                QuestDic.Add(quest.questID, quest);
            }
        }
        
        Debug.Log(QuestDic.Count + "개의 퀘스트 SO를 로드했습니다!");
    }

    private void InitQuest()
    {
        QuestSO[] quests = Resources.LoadAll<QuestSO>(QuestPath);
        
        // QuestSO[] quests =
        // {
        //     Resources.Load<QuestSO>(QuestPath + "/2"),
        //     Resources.Load<QuestSO>(QuestPath + "/3"),
        // };
        
        foreach (var quest in quests)
        {
            if (!(QuestDic.ContainsKey(quest.questID)))
            {
                QuestDic.Add(quest.questID, quest);
            }
        }
        
        Debug.Log(QuestDic.Count + "개의 퀘스트 SO를 로드했습니다!");
    }

    public Quest GetRandomQuest(QuestSlot currentSlot)
    {
        // 현재 슬롯들에 들어간 퀘스트들의 퀘스트ID 수집 
        HashSet<int> usedQuestIds = new HashSet<int>();
        foreach (var slot in questUI.slots)
        {
            if (slot.quest != null && slot.quest.questData != null)
            {
                // Debug.Log(slot.quest.questData.questID);
                usedQuestIds.Add(slot.quest.questData.questID);
            }
        }
        
        // 현재 중복되지 않아 사용 가능한 퀘스트 후보들
        List<QuestSO> candidates = new List<QuestSO>();
        foreach (var quest in QuestDic)
        {
            if (!usedQuestIds.Contains(quest.Key))
            {
                candidates.Add(quest.Value);
            }
        }
        
        // 퀘스트 후보에서 랜덤으로 하나를 뽑아
        var selected = candidates[Random.Range(0, candidates.Count)];
        if (selected == null)
        {
            Debug.LogError("GetRandomQuest: 선택된 퀘스트 데이터가 null!");
            return null;
        }
        
        // Quest 클래스를 만들어서 슬롯에 넣어준다
        var newQuest = new Quest(selected);
        return newQuest;
    }

    public void UpdateQuestProgress(int targetObjectID)
    {
        if (questUI.slots == null) return;

        foreach (var slot in questUI.slots)
        {
            if (slot.quest == null) continue;

            if (slot.quest.currentState != QuestState.InProgress) continue;

            if (slot.quest.questData.targetObjectID == targetObjectID)
            {
                slot.quest.currentValue++;
                
                slot.UpdateInProgressUI();

                if (slot.quest.currentValue >= slot.quest.questData.targetValue)
                {
                    slot.quest.currentState = QuestState.Completed;
                    slot.currentState = SlotState.Complete;
                    slot.ChangeToCompleteState();
                    //questUI.slotCount--;
                }
            }
        }
    }
}
