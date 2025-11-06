using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUIZone : MonoBehaviour
{
    private Coroutine _uiCoroutine;
    [SerializeField] private float targetTime = 2f;

    private void Awake()
    {
        _uiCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_uiCoroutine == null)
            {
                _uiCoroutine = StartCoroutine(ShowUI());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopCoroutine(_uiCoroutine);
            _uiCoroutine = null;
            UIManager.Instance.CloseUI<QuestUI>();
        }
    }

    private IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(targetTime);

        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<QuestUI>();
        }
    }
}
