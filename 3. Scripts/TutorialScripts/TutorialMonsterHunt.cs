using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class TutorialMonsterHunt : TutorialBase
{
    [SerializeField] private int targetPrefabIndex;
    [SerializeField] private float targetCount;
   
    private TutorialController controller;
    private int currentCount = 0;
    private bool isCompleted = false;

    private List<MonsterCondition> connectedMonsters = new List<MonsterCondition>();
    
    public override void Enter(TutorialController controller)
    {
        this.controller = controller;
        
        var monsters = TutorialEnemyManager.Instance.GetSpawnedMonsters(targetPrefabIndex);
        foreach (var monster in monsters)
        {
            var condition = monster.GetComponent<MonsterCondition>();
            if (condition != null)
            {
                connectedMonsters.Add(condition);
            }
        }
        
        EventManager.Instance.Subscribe<MonsterDiedEvent>(OnMonsterDie);
        EventManager.Instance.Subscribe<MonsterSpawnedEvent>(OnMonsterSpawnedHandler);
        
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(true);
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText(currentText);
        
        // controller.tutorialProgressUI.SetActive(true);
        // controller.progressText.text = currentText;
    }

    public override void Execute(TutorialController controller)
    {
    }

    public override void Exit(TutorialController controller)
    {
        EventManager.Instance.Unsubscribe<MonsterDiedEvent>(OnMonsterDie);
        EventManager.Instance.Unsubscribe<MonsterSpawnedEvent>(OnMonsterSpawnedHandler);
        
        connectedMonsters.Clear();
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(false);
        // controller.tutorialProgressUI.SetActive(false);
    }
    
    private void OnMonsterDie(MonsterDiedEvent e)
    {
        if (isCompleted) return;

        currentCount++;

        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText("파랑 버섯 처치 ( " + currentCount + " / " + targetCount + " )");
        // controller.progressText.text = "파랑 버섯 처치 ( " + currentCount + " / " + targetCount + " )";

        if (currentCount >= targetCount)
        {
            isCompleted = true;
            CompleteTutorial(this.controller);
            this.controller = null;
        }
    }
    
    private void CompleteTutorial(TutorialController controller)
    {
        controller.SetNextTutorial();
    }
    
    private void OnMonsterSpawnedHandler(MonsterSpawnedEvent eventData)
    {
        if (eventData.PrefabIndex != targetPrefabIndex) return;

        var condition = eventData.MonsterInstance.GetComponent<MonsterCondition>();
        if (condition == null) return;

        EventManager.Instance.Subscribe<MonsterDiedEvent>(OnMonsterDie);
        connectedMonsters.Add(condition);
    }
}
