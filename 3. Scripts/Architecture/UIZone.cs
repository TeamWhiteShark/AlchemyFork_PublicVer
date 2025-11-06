using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIZone : MonoBehaviour
{
    [SerializeField]private BaseArchitecture architecture;
    private float _startTime;
    [SerializeField] private float _targetTime = 2f;
    private Coroutine _coroutine;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //ui 타이머 시작
            _coroutine = StartCoroutine(ShowUI());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //ui 오프
            StopCoroutine(_coroutine);
            if (architecture.interactionUI == null) return;
            architecture.interactionUI.baseArchitecture = null;
            architecture.interactionUI.upgradeButton.onClick.RemoveAllListeners();
            architecture.upgradeWaitTime = 0.5f;
            UIManager.Instance.CloseUI<InteractionUI>();
        }
    }
    
    private IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(_targetTime);
        architecture.interactionUI = UIManager.Instance.GetUI<InteractionUI>();
        architecture.interactionUI.baseArchitecture = architecture;
        UIManager.Instance.OpenUI<InteractionUI>();
    }
}