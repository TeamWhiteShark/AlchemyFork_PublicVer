using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HoldToRepeatUpgrade : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Target")]
    public LaboratorySlot slot;       // Inspector에서 할당

    [Header("Repeat Settings")]
    public float initialDelay = 0.35f;   // 길게 누른 뒤 첫 발사까지 지연
    public float repeatInterval = 0.12f; // 기본 반복 간격
    public float minInterval = 0.04f;    // 최소 간격(가속 하한)
    public float acceleration = 0.9f;    // 반복할 때마다 interval *= acceleration

    private bool holding;
    private Coroutine co;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (co != null) StopCoroutine(co);
        holding = true;
        co = StartCoroutine(HoldLoop());
    }

    public void OnPointerUp(PointerEventData eventData)  => StopHold();
    public void OnPointerExit(PointerEventData eventData)=> StopHold();
    private void OnDisable()                             => StopHold();

    private void StopHold()
    {
        holding = false;
        if (co != null) { StopCoroutine(co); co = null; }
    }

    private IEnumerator HoldLoop()
    {
        // 탭(짧은 클릭)과 구분하기 위한 초지연
        yield return new WaitForSecondsRealtime(initialDelay);

        float interval = repeatInterval;
        while (holding)
        {
            if (slot == null || !slot.CanUpgrade())
            {
                StopHold();
                yield break;
            }

            slot.OnClickUpgradeButton(); // 기존 단일 업그레이드 로직 재사용

            yield return new WaitForSecondsRealtime(interval);
            interval = Mathf.Max(minInterval, interval * acceleration);
        }
    }
}