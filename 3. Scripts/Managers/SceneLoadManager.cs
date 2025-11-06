using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    public bool isManager = false;

    protected override bool isDestroy => false;

    public string NowSceneName = "";

    [SerializeField] private LoadingUI loadingUI;
    [SerializeField] private List<string> excludeLoadingScenes = new List<string> { "IntroScene" };

    protected override void Awake()
    {
        base.Awake();
        Analytics.InitializeAsync();
        NowSceneName = SceneManager.GetActiveScene().name;
    }
    
    public async void ChangeScene(string sceneName, Action callback = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        await ChangeSceneAsync(sceneName, callback, loadSceneMode);
    }
    
    public async Task ChangeSceneAsync(string sceneName, Action callback = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        if (sceneName == "Manager") return;
        
        bool showLoading = !excludeLoadingScenes.Contains(sceneName);

        if (showLoading && loadingUI == null)
        {
            loadingUI = UIManager.Instance.GetUI<LoadingUI>();
        }
        
        if (showLoading && loadingUI != null)
        {
            loadingUI.OpenUI();
        }
        
        var op = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            Debug.Log("로딩중... " + (op.progress * 100f));
            loadingUI?.SetProgress(op.progress);
            await Task.Yield();
        }
        
        loadingUI?.SetProgress(1f);
        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            await Task.Yield();
        }
        Debug.Log("로딩 완료");

        if (loadSceneMode == LoadSceneMode.Single)
            NowSceneName = sceneName;

        callback?.Invoke();
        
        if (loadingUI != null)
        {
            // 로딩 완료 후 페이드 아웃 시작
            loadingUI.StartFadeOut(3f, null);
        }
    }

    public async void UnLoadScene(string sceneName, Action callback = null)
    {
        var op = SceneManager.UnloadSceneAsync(sceneName);

        while(!op.isDone)
        {
            Debug.Log("씬 언로드 중...");
            await Task.Yield();
        }

        callback?.Invoke();
    }
}
