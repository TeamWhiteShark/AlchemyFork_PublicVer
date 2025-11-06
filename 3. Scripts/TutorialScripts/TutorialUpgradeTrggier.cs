using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialUpgradeTrggier : TutorialBase
{
    private bool isCompleted = false;
    private BaseArchitecture currentPot;
    private TutorialController tutorialController;
    [SerializeField] private int targetCount;
    
    public override void Enter(TutorialController controller)
    {
        tutorialController = controller;
        
        isCompleted = false;
        currentPot = ArchitectureManager.Instance.cooks.Values.FirstOrDefault();
        
        if (currentPot != null)
        {
            // 튜토리얼 중 업그레이드 이벤트 연결
            EventManager.Instance.Subscribe<ArchitectureUpgradedEvent>(OnArchitectureUpgraded);
        }
        else
        {
            Debug.LogWarning("솥 건축물이 존재하지 않습니다. TutorialUpgradePotTrigger 초기화 실패");
        }
        
        // 이미 목표 레벨 이상이라면 즉시 완료 처리
        if (currentPot != null && currentPot.upgradeLevel >= targetCount)
        {
            Debug.Log($"튜토리얼 스킵: 현재 솥 레벨 {currentPot.upgradeLevel} (목표 {targetCount})");
            isCompleted = true;
            controller.SetNextTutorial();
            return;
        }
        
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(true);
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText(currentText);
        
        // controller.tutorialProgressUI.SetActive(true);
        // controller.progressText.text = currentText;
    }

    public override void Execute(TutorialController controller)
    {
        if (isCompleted)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit(TutorialController controller)
    {
        if (currentPot != null)
        {
            EventManager.Instance.Unsubscribe<ArchitectureUpgradedEvent>(OnArchitectureUpgraded);
        }

        foreach (var arrow in controller.currentArrow)
        {
            Destroy(arrow);
        }
        controller.currentArrow?.Clear();
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(false);
        //controller.tutorialProgressUI.SetActive(false);
    }
    
    private void OnArchitectureUpgraded(ArchitectureUpgradedEvent e)
    {
        if (currentPot != null)
        {
            UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText("약솥 업그레이드 ( " + currentPot.upgradeLevel + " / " + targetCount + " )");
            // tutorialController.progressText.text = "약솥 업그레이드 ( " + currentPot.upgradeLevel + " / " + targetCount + " )";
        }
        
        if (e.Architecture.archType == ArchType.Cook && e.Architecture.upgradeLevel >= targetCount)
        {
            Debug.Log("솥 업그레이드 튜토리얼 완료");
            isCompleted = true;
        }
    }
}
