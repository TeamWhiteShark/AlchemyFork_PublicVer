using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoSingleton<EventManager>
{
    protected override bool isDestroy => false;
    
    // 이벤트 딕셔너리 - 타입별로 이벤트를 관리
    private Dictionary<Type, List<Delegate>> eventHandlers = new Dictionary<Type, List<Delegate>>();

    /// <summary>
    /// 이벤트 구독
    /// </summary>
    public void Subscribe<T>(Action<T> handler) where T : struct
    {
        Type eventType = typeof(T);

        if (!eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] = new List<Delegate>();
        }

        eventHandlers[eventType].Add(handler);
    }

    /// <summary>
    /// 이벤트 구독 해제
    /// </summary>
    public void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        Type eventType = typeof(T);

        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType].Remove(handler);
        }
    }

    /// <summary>
    /// 이벤트 발행
    /// </summary>
    public void Publish<T>(T eventData) where T : struct
    {
        Type eventType = typeof(T);
        
        if (eventHandlers.ContainsKey(eventType) && eventHandlers[eventType].Count > 0)
        {
            var handlersCopy = new List<Delegate>(eventHandlers[eventType]);

            foreach (var handler in handlersCopy)
            {
                if (handler is Action<T> typedHandler)
                {
                    try
                    {
                        typedHandler.Invoke(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"이벤트 핸들러 실행 중 오류 발생: {e.Message}");
                        Debug.LogError($"문제 핸들러 대상: {typedHandler.Target} / 메서드: {typedHandler.Method.Name}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 모든 이벤트 구독 해제 (씬 전환 시 사용)
    /// </summary>
    public void ClearAllEvents()
    {
        eventHandlers.Clear();
    }

    /// <summary>
    /// 특정 타입의 이벤트만 구독 해제
    /// </summary>
    public void ClearEventsOfType<T>() where T : struct
    {
        Type eventType = typeof(T);
        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType].Clear();
        }
    }

    /// <summary>
    /// 이벤트 핸들러 수 조회 (디버깅용)
    /// </summary>
    public int GetEventHandlerCount<T>() where T : struct
    {
        Type eventType = typeof(T);
        return eventHandlers.ContainsKey(eventType) ? eventHandlers[eventType].Count : 0;
    }
}
