using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSellTrigger : TutorialBase
{
    private int calculatedCustomerCount = 0;
    private bool isCompleted = false;
    private TutorialController tutorialController;
    
    [SerializeField] private int targetCount;
    
    public override void Enter(TutorialController controller)
    {
        tutorialController = controller;
        
        CustomerManager.Instance.isReadyInTutorial = true;
        isCompleted = false;

        if (ArchitectureManager.Instance.counters != null)
        {
            EventManager.Instance.Subscribe<CustomerCalculatedEvent>(OnCustomerCalculated);
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
        EventManager.Instance.Unsubscribe<CustomerCalculatedEvent>(OnCustomerCalculated);

        foreach (var arrow in controller.currentArrow)
        {
            Destroy(arrow);
        }
        
        controller.currentArrow?.Clear();
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(false);
        // controller.tutorialProgressUI.SetActive(false);
    }
    
    private void OnCustomerCalculated(CustomerCalculatedEvent eventData)
    {
        calculatedCustomerCount++;

        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText("파랑 포션 판매 ( " + calculatedCustomerCount + " / " + targetCount + " )");
        // tutorialController.progressText.text = "포션 판매 ( " + calculatedCustomerCount + " / " + targetCount + " )";
        
        if (calculatedCustomerCount >= targetCount)
        {
            isCompleted = true;
        }
    }
}
