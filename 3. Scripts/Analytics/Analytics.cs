using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;

public static class Analytics
{
    private static bool _isInitialized = false;

    // 이벤트를 각각 메서드로 생성하여 사용
    /*public static void AnalyticsDeathCount(int monsterid)
    {
        var DieEvent = new CustomEvent("Player_Died"); //evemt
        DieEvent["monster_id"] = monsterid; // parameter

        AnalyticsService.Instance.RecordEvent(DieEvent);
        Debug.Log("Analytics: Player Died");
    }

    public static void AnalyticsWorkCount(string Entity, string JobType)
    {
        var WorkEvent = new CustomEvent("Work_Completed"); //evemt
        WorkEvent["Entity"] = Entity; // parameter
        WorkEvent["JobType"] = JobType; // parameter

        AnalyticsService.Instance.RecordEvent(WorkEvent);
        Debug.Log("Analytics: Work Completed");
    }*/

    // 이벤트 이름과 파라미터를 받아서 처리하는 메서드
    public static void AddEvent(string eventName, Dictionary<string, object> pairs)
    {
        CustomEvent myevent = new CustomEvent(eventName);

        foreach (var p in pairs)
        {
            myevent.Add(p.Key, p.Value);
        }
        AnalyticsService.Instance.RecordEvent(myevent);

            // 기록 안 될때 강제 저장
        AnalyticsService.Instance.Flush();
    }

    //gpt가 만들어준 오류 수정용 메서드 후에 분석 필요함
    public static async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            _isInitialized = true;
            Debug.Log("✅ Analytics initialized successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Analytics initialization failed: {e.Message}");
        }
    }
}
