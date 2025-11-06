using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    BeforeStart,
    InProgress,
    Completed
}

// 게임 중의 퀘스트 
[System.Serializable]
public class Quest
{
    // QuestSO에서 가져올 데이터
    public QuestSO questData;
    
    // 퀘스트의 지금 상태
    public QuestState currentState;
    // 현재 진행도
    public int currentValue;

    public Quest(QuestSO data)
    {
        this.questData = data;
        currentState = QuestState.BeforeStart;
        currentValue = 0;
    }
}
