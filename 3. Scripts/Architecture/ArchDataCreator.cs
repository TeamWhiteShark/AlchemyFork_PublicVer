#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ArchDataCreator
{
    [MenuItem("Tools/Arch/Create ArchDataSO From ArchInfoSO")]
    public static void CreateArchDataSO()
    {
        string outputPath = "Assets/Resources/Data/Arch/ArchDataSO";

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        
        // 모든 MonsterStatSO 불러오기
        Dictionary<int, ArchDataSO> archIDs = new Dictionary<int, ArchDataSO>();
        ArchDataSO[] allDatas = Resources.LoadAll<ArchDataSO>("Data/Arch/ArchDataSO");

        foreach (ArchDataSO archData in allDatas)
        {
            archIDs.Add(archData.archID, archData);
        }
            
        ArchInfoSO[] allInfos = Resources.LoadAll<ArchInfoSO>("Data/Arch/ArchInfoSO");

        foreach (ArchInfoSO info in allInfos)
        {
            ArchDataSO data = null;

            // 새로운 ArchDataSO 생성
            data = !archIDs.TryGetValue(info.archID, out ArchDataSO d) ? ScriptableObject.CreateInstance<ArchDataSO>() : d;

            // 스프레드 시트 기반 데이터를 가져온다
            data.archID = info.archID;
            data.archName = info.archName;
            data.upgradeType = info.upgradeType;
            data.unlockMoney = info.unlockMoney;
            data.upgradePrice = info.upgradePrice;
            data.upgradeMultiplier = info.upgradeMultiplier;
            data.upgradeMultiplierChangeRate = info.upgradeMultiplierChangeRate;
            data.productPrice = info.productPrice;
            data.productUpgradeMultiplier = info.productUpgradeMultiplier;
            data.productUpgradeMultiplierChangeRate = info.productUpgradeMultiplierChangeRate;
            data.productTime = info.productTime;
            data.maxLevel = info.maxLevel;
            data.conditionText = info.conditionText;

            if (!archIDs.ContainsKey(info.archID))
            {
                // SO 파일 저장
                string assetPath = $"{outputPath}/{data.archID}.asset";
                AssetDatabase.CreateAsset(data, assetPath);
            }
            else
            {
                EditorUtility.SetDirty(data); // 이미 존재하는 경우 반드시 추가
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ ArchDataSO 생성 완료: {allInfos.Length}개 생성됨");
    }
}
#endif