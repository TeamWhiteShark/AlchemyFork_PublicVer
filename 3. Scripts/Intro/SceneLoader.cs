using UnityEngine;
using File = System.IO.File;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        Analytics.InitializeAsync();
    }
    
    public void GameStart()
    {
        // SaveLoadManager.Instance.isClickedContinue = false;
        // SceneLoadManager.Instance.ChangeScene("TutorialScene");
        
        UIManager.Instance.OpenUI<LogInUI>();
    }

    public void LoadGame()
    {
        var fullPath = Application.persistentDataPath + "/SaveData/SaveData.json";
        if (!File.Exists(fullPath))
        {
            Debug.LogError("세이브 파일이 존재하지 않습니다.");
            return;
        }
        else
        {
            var data = SaveLoadManager.Instance.LoadPlayerDataFromLocal();

            SaveLoadManager.Instance.isClickedContinue = true;                        
            SceneLoadManager.Instance.ChangeScene(data.SceneName);           
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        UIManager.Instance.OpenUI<ReviewUI>();
#else
        Application.Quit();
#endif
    }
}
