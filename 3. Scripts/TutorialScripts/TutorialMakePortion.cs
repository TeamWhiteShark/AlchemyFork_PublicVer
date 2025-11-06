using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialMakePortion : TutorialBase
{
    [SerializeField] private int targetCount;
    [SerializeField] private int targetCookIndex;

    private TutorialController tutorialController;
    private Cook targetCook;
    private int currentCount = 0;
    private bool isCompleted = false;
    
    public override void Enter(TutorialController controller)
    {
        tutorialController = controller;
        
        var cooksDict = ArchitectureManager.Instance.cooks;
        if (cooksDict == null || cooksDict.Count == 0)
        {
            Debug.LogWarning("[튜토리얼] 조리대 없음");
            controller.SetNextTutorial();
            return;
        }

        if (targetCookIndex < 0 || targetCookIndex >= cooksDict.Count)
        {
            Debug.LogWarning($"[튜토리얼] 잘못된 조리대 인덱스: {targetCookIndex}");
            controller.SetNextTutorial();
            return;
        }

        var targetArch = cooksDict.ElementAt(targetCookIndex).Value;
        targetCook = targetArch as Cook;
        if (targetCook == null)
        {
            Debug.LogWarning("[튜토리얼] 선택된 건물이 Cook이 아님");
            controller.SetNextTutorial();
            return;
        }
        
        EventManager.Instance.Subscribe<ItemProducedEvent>(OnItemProduced);
        currentCount = 0;

        Debug.Log($"[튜토리얼] '{targetCook.name}' 조리대 추적 시작");
        
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(true);
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText(currentText);
        
        // controller.tutorialProgressUI.SetActive(true);
        // controller.progressText.text = currentText;
    }

    public override void Execute(TutorialController controller)
    {
        if (isCompleted || targetCook == null)
            return;

        if (targetCook.productCount > currentCount)
        {
            currentCount = targetCook.productCount;

            if (currentCount >= targetCount)
            {
                CompleteTutorial(controller);
            }
        }
    }

    public override void Exit(TutorialController controller)
    {
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(false);
        // controller.tutorialProgressUI.SetActive(false);
    }
    
    private void CompleteTutorial(TutorialController controller)
    {
        isCompleted = true;
        controller.SetNextTutorial();
    }
    
    private void OnItemProduced(ItemProducedEvent eventData)
    {
        if (isCompleted || targetCook != eventData.CookInstance)
            return;

        currentCount++;
        
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText("파랑 포션 제작 ( " + currentCount + " / " + targetCount + " )");
        // tutorialController.progressText.text = "파랑 포션 제작 ( " + currentCount + " / " + targetCount + " )";

        if (currentCount >= targetCount)
        {
            isCompleted = true;
            CompleteTutorial(tutorialController);
        }
    }
}
