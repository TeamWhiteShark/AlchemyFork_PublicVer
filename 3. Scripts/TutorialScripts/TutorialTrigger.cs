using UnityEngine;

public class TutorialTrigger : TutorialBase
{
    [SerializeField] private ArchSpawner targetSpawner;
    [SerializeField] private int archIndex;

	public override void Enter(TutorialController controller)
    {
        targetSpawner = ArchitectureManager.Instance.archSpawners[archIndex];
        
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(true);
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicatorText(currentText);
        
        // controller.tutorialProgressUI.SetActive(true);
        // controller.progressText.text = currentText;
    }

	public override void Execute(TutorialController controller)
	{
        if (targetSpawner.isUnlockedOnTutorial == true)
        {
            foreach (var arrow in controller.currentArrow)
            {
                Destroy(arrow);
            }
            controller.currentArrow?.Clear();
            controller.SetNextTutorial();
        }
	}

	public override void Exit(TutorialController controller)
	{
		// controller.tutorialProgressUI.SetActive(false);
        UIManager.Instance.GetUI<InventoryUI>().HandleIndicator(false);
	}
}

