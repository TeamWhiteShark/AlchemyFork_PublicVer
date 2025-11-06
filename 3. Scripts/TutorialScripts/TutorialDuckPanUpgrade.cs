using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDuckPanUpgrade : TutorialBase
{
    private bool isCompleted = false;
    private bool isPanFound = false;
    private bool isStandFound = false;
    private bool isDungeonWallUnlocked = false;
    
    private BaseArchitecture currentPan;
    private ArchSpawner dungeonWall;
    private TutorialController tutorialController;

    private GameObject arrowDungeonWall;
    private GameObject arrowPan;
    private GameObject arrowStand;

    [SerializeField] private int targetCount;
    
    public override void Enter(TutorialController controller)
    {
        isCompleted = false;
        isPanFound = false;
        isStandFound = false;
        currentPan = null;
        tutorialController = controller;

        if (ArchitectureManager.Instance.dungeonWall != null)
        {
            isDungeonWallUnlocked = false;
            dungeonWall = ArchitectureManager.Instance.dungeonWall[0];
        }

        arrowDungeonWall = controller.currentArrow[0];
        arrowPan = controller.currentArrow[1];
        arrowStand = controller.currentArrow[2];
        
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(true);
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText(currentText);
        
        // controller.tutorialProgressUI.SetActive(true);
        // controller.progressText.text = currentText;
    }

    public override void Execute(TutorialController controller)
    {
        if (!isPanFound)
        {
            foreach (var cook in ArchitectureManager.Instance.cooks)
            {
                if (cook.Key != null && cook.Key.itemID == "2004")
                {
                    currentPan = cook.Value;
                    isPanFound = true;

                    Destroy(arrowPan);
                    controller.currentArrow.Remove(arrowPan);
                    
                    if (currentPan != null)
                    {
                        EventManager.Instance.Subscribe<ArchitectureUpgradedEvent>(OnArchitectureUpgraded);
                        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText("화로 업그레이드 ( " + currentPan.upgradeLevel + " / " + targetCount + " )");
                        //tutorialController.progressText.text = "오리구이 화로를 업그레이드 \n( " + currentPan.upgradeLevel + " / " + targetCount + " )";
                    }

                    break;
                }
            }
        }

        if (!isStandFound)
        {
            foreach (var stand in ArchitectureManager.Instance.stands)
            {
                if (stand.Key != null && stand.Key.itemID == "2004")
                {
                    isStandFound = true;
                    
                    Destroy(arrowStand);
                    controller.currentArrow.Remove(arrowStand);
                }
            }
        }

        if (!isDungeonWallUnlocked)
        {
            if (ArchitectureManager.Instance.dungeonWall[0].gameObject.activeSelf == false)
            {
                isDungeonWallUnlocked = true;
                Destroy(arrowDungeonWall);
                controller.currentArrow.Remove(arrowDungeonWall);
            }
        }
        
        if (isCompleted)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit(TutorialController controller)
    {
        if (currentPan != null)
        {
            EventManager.Instance.Unsubscribe<ArchitectureUpgradedEvent>(OnArchitectureUpgraded);
        }

        foreach (var arrow in controller.currentArrow)
        {
            Destroy(arrow);
        }

        controller.currentArrow?.Clear();
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(false);
        // controller.tutorialProgressUI.SetActive(false);
    }
    
    private void OnArchitectureUpgraded(ArchitectureUpgradedEvent e)
    {
        if (currentPan != null)
        {
            UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText("화로 업그레이드 ( " + currentPan.upgradeLevel + " / " + targetCount + " )");
            // tutorialController.progressText.text = "오리구이 화로를 업그레이드 ( " + currentPan.upgradeLevel + " / " + targetCount + " )";
        }
        
        if (currentPan != null && e.Architecture.archType == ArchType.Cook && e.Architecture.upgradeLevel >= targetCount)
        {
            isCompleted = true;
        }
    }
}
