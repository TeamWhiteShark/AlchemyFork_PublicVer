using UnityEngine;

[RequireComponent(typeof(DialogSystem))]
public class TutorialDialog : TutorialBase
{
	// 캐릭터들의 대사를 진행하는 DialogSystem
	private	DialogSystem dialogSystem;

	public override void Enter(TutorialController controller)
	{
		dialogSystem = GetComponent<DialogSystem>();
		dialogSystem.Setup();
        UIManager.Instance.isUIOn = true;
    }

	public override void Execute(TutorialController controller)
	{
		// 현재 분기에 진행되는 대사 진행
		bool isCompleted = dialogSystem.UpdateDialog();

		// 현재 분기의 대사 진행이 완료되면
		if ( isCompleted == true )
		{
			// 다음 튜토리얼로 이동
			controller.SetNextTutorial();
		}
	}

	public override void Exit(TutorialController controller)
    {
        UIManager.Instance.isUIOn = false;
    }
}

