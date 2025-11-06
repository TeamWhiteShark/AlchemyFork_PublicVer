#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MonsterDataCreator
{
    [MenuItem("Tools/Monster/Create MonsterData From MonsterStatSO")]
    public static void CreateMonsterDataSO()
    {
        string outputPath = "Assets/Resources/Data/Monster/MonsterData";

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        
        Dictionary<int, MonsterData> monsterIDs = new Dictionary<int, MonsterData>();
        MonsterData[] allDatas = Resources.LoadAll<MonsterData>("Data/Monster/MonsterData");

        foreach (MonsterData monsterData in allDatas)
        {
            monsterIDs.Add(monsterData.monsterID,  monsterData);
        }

        // 모든 MonsterStatSO 불러오기
        MonsterStatSO[] allStats = Resources.LoadAll<MonsterStatSO>("Data/Monster/MonsterStatSO");

        foreach (MonsterStatSO stat in allStats)
        {
            MonsterData data = null;
            
            // 새로운 MonsterData 생성
            data = !monsterIDs.TryGetValue(stat.monsterID, out MonsterData monster) ? ScriptableObject.CreateInstance<MonsterData>() : monster;

            // 스프레드 시트 기반 데이터를 가져온다
            data.monsterID = stat.monsterID;
            data.monsterName = stat.monsterName;
            data.monsterHealth = stat.monsterHealth;
            data.monsterAttack = stat.monsterAttack;
            data.monsterSpeed = stat.monsterSpeed;
            data.monsterAttackRate = stat.monsterAttackRate;

            // // 추가 필드는 기본값(null)
            // data.behaviorType = MonsterBehaviorType.Aggressive;
            // data.monsterType = MonsterType.Plant;
            // data.dropItem = null;
            // data.sprite = null;
            // data.prefab = null;
            
            if (!monsterIDs.ContainsKey(stat.monsterID))
            {
                // SO 파일 저장
                string assetPath = $"{outputPath}/{data.monsterID}.asset";
                AssetDatabase.CreateAsset(data, assetPath);
            }
            else
            {
                EditorUtility.SetDirty(data); // 이미 존재하는 경우 반드시 추가
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ MonsterData 생성 완료: {allStats.Length}개 생성됨");
    }
}
#endif