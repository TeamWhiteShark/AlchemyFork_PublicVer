using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingUI : UIBase
{
    public override bool isDestroy => false;

    private Coroutine timerCoroutine;

    private CanvasGroup canvasGroup;
    private float duration = 1f;

    public GameObject witchImage;
    [SerializeField] private TextMeshProUGUI progressText;

    public event Action OnFadeOutCompleted;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public override void OpenUI()
    {
        base.OpenUI();
        UIManager.Instance.isUIOn = true;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        witchImage.transform.localScale = Vector3.one;
        
        if (progressText != null) 
            progressText.text = "0%";
    }

    public override void CloseUI()
    {
        base.CloseUI();
        UIManager.Instance.isUIOn = false;
    }

    public void SetProgress(float value)
    {
        if (progressText != null)
            progressText.text = $"{(value * 100f):F0}%";
    }
    
    public void StartFadeOut(float delay = 0f, Action onComplete = null)
    {
        OnFadeOutCompleted += onComplete;
        timerCoroutine = StartCoroutine(ExitCoroutine(delay));
    }
    
    private IEnumerator ExitCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        float startAlpha = canvasGroup.alpha;
        Vector3 startScale = witchImage.transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            witchImage.transform.localScale = Vector3.Lerp(startScale, GameConstants.UI.LOADING_WITCH_SCALE, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false; // 클릭 방지
        
        CloseUI();
        
        OnFadeOutCompleted?.Invoke();
    }
}
