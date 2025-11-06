using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TutorialCameraMove : TutorialBase
{
    [SerializeField] private CinemachineVirtualCamera architectureCam;
    [SerializeField] private float duration;
    [SerializeField] private int archIndexToArrowSpawn;
    
    private WaitForSeconds focusDuration;
    private bool isFinished = false;
    
    public override void Enter(TutorialController controller)
    {
        focusDuration = new WaitForSeconds(duration);
        isFinished = false;
        
        StartCoroutine(FocusCoroutine(controller));
    }

    public override void Execute(TutorialController controller)
    {
        if (isFinished == true)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit(TutorialController controller)
    {
        PlayerManager.Instance.Player.isCameraMovedInTutorial = false;
    }

    private IEnumerator FocusCoroutine(TutorialController controller)
    {
        architectureCam.Priority = 20;
        PlayerManager.Instance.Player.isCameraMovedInTutorial = true;
        
        GameObject arrow;
        switch (archIndexToArrowSpawn)
        {
            case 1:
                arrow = Instantiate(controller.arrowPrefab,
                    ArchitectureManager.Instance.archSpawners[1].transform.position + Vector3.up * 1.2f, Quaternion.identity);
                controller.currentArrow.Add(arrow);
                break;
            case 2:
                arrow = Instantiate(controller.arrowPrefab,
                    ArchitectureManager.Instance.archSpawners[2].transform.position + Vector3.up * 1.2f, Quaternion.identity);
                controller.currentArrow.Add(arrow);
                break;
            case 3:
                arrow = Instantiate(controller.arrowPrefab, GameConstants.Tutorial.ARROW_POSITION_1, Quaternion.identity);
                controller.currentArrow.Add(arrow);
                break;
            case 4:
                arrow = Instantiate(controller.arrowPrefab, GameConstants.Tutorial.ARROW_POSITION_2, Quaternion.identity);
                controller.currentArrow.Add(arrow);
                break;
            case 5:
                arrow = Instantiate(controller.arrowPrefab, GameConstants.Tutorial.ARROW_POSITION_3, Quaternion.identity);
                controller.currentArrow.Add(arrow);
                break;
            case 6:
                arrow = Instantiate(controller.arrowPrefab,
                    ArchitectureManager.Instance.archSpawners[0].transform.position + Vector3.up * 1.2f,
                    Quaternion.identity);
                controller.currentArrow.Add(arrow);
                break;
            case 7:
                arrow = Instantiate(controller.arrowPrefab, GameConstants.Tutorial.ARROW_POSITION_4, Quaternion.identity);
                controller.currentArrow.Add(arrow);
                break;
            case 8:
                arrow = Instantiate(controller.arrowPrefab, GameConstants.Tutorial.ARROW_POSITION_5, Quaternion.identity);
                controller.currentArrow.Add(arrow);
                break;
        }
        
        yield return focusDuration;
        
        architectureCam.Priority = 0;
        isFinished = true;
    }
}
