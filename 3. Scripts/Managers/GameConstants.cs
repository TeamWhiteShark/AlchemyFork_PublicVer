using UnityEngine;

/// <summary>
/// 게임에서 사용되는 모든 상수값들을 관리하는 클래스
/// 하드코딩된 값들을 중앙화하여 관리하고 유지보수성을 향상시킵니다.
/// </summary>

public static class GameConstants
{
    public static int NextMapMaxValue = 7;
    #region 경로 상수 (Paths)

    /// <summary>
    /// UI 프리팹 경로
    /// </summary>
    public static class Paths
    {
        public const string UI_PREFAB_PATH = "Prefabs/UI/";
        public const string ARCHITECTURE_PREFAB_PATH = "Prefabs/Architecture/";
        public const string ITEM_PREFAB_PATH = "Prefabs/Item/";
        
        // 데이터 경로
        public const string QUEST_DATA_PATH = "Data/Quest/QuestSO";
        public const string ITEM_DATA_PATH = "Data/Item/";
        public const string MONSTER_DATA_PATH = "Data/Monster/MonsterData/";
        public const string PLAYER_DATA_PATH = "Data/Player/PlayerData";
        public const string ARCH_DATA_PATH = "Data/Arch/ArchDataSO";
        
        // 세이브 데이터 경로
        public const string SAVE_DATA_PATH = "SaveData/SaveData";
        public const string SAVE_DATA_FILE_NAME = "SaveData.json";
        
        // 에디터 도구 경로
        public const string JSON_FILES_PATH = "Assets/Resources/JsonFiles/";
        public const string DATA_CLASS_PATH = "Assets/Resources/DataClass/";
        public const string MONSTER_DATA_OUTPUT_PATH = "Assets/Resources/Data/Monster/MonsterData";
        public const string ARCH_DATA_OUTPUT_PATH = "Assets/Resources/Data/Arch/ArchDataSO";
    }
    
    #endregion
    
    #region 게임플레이 상수 (Gameplay)
    
    /// <summary>
    /// 플레이어 관련 상수
    /// </summary>
    public static class Player
    {
        public const float DEFAULT_MOVE_SPEED = 6f;
        public const float DUNGEON_MOVE_SPEED = 6f;
        public const int ROTATION_SPEED = 10;
        
        // 플레이어 스케일
        public static readonly Vector3 PLAYER_SCALE_RIGHT = new Vector3(1, 1, 1);
        public static readonly Vector3 PLAYER_SCALE_LEFT = new Vector3(-1, 1, 1);
        
        // 플레이어 위치
        public static readonly Vector3 PLAYER_RESPAWN_POSITION = new Vector3(0, -15, 0);
        public static readonly Vector3 PLAYER_RESPAWN_POSITION2 = new Vector3(44, 3, 0);
    }
    
    /// <summary>
    /// NPC 관련 상수
    /// </summary>
    public static class NPC
    {
        // NPC 스폰 위치
        public static readonly Vector3 HUNTER_SPAWN_POSITION = new Vector3(26.5f, 2.5f, 0);
        public static readonly Vector3 CHEF_SPAWN_POSITION = new Vector3(26.5f, 3.5f, 0);
        public static readonly Vector3 WAITER_SPAWN_POSITION = new Vector3(27.5f, 2.5f, 0);
        public static readonly Vector3 CASHIER_SPAWN_POSITION = new Vector3(28.5f, 2.5f, 0);
        public static readonly Vector3 DEFAULT_NPC_SPAWN_POSITION = new Vector3(27.5f, 3f, 0);
        public static readonly Vector3 LOAD_NPC_SPAWN_POSITION = new Vector3(-27.5f, 2.5f, 0);
        
        // NPC 스케일
        public static readonly Vector3 NPC_SCALE_RIGHT = new Vector3(1, 1, 1);
        public static readonly Vector3 NPC_SCALE_LEFT = new Vector3(-1, 1, 1);
        
        // NPC 타겟 위치 오프셋
        public static readonly Vector3 CASHIER_TARGET_OFFSET = new Vector3(-1.5f, 3f, 0);
        public static readonly Vector3 ARCHITECTURE_OFFSET_MIN = new Vector3(-1.5f, -2.5f, 0);
        public static readonly Vector3 ARCHITECTURE_OFFSET_MAX = new Vector3(1.5f, -3f, 0);
        public static readonly Vector3 STAND_OFFSET_MIN = new Vector3(-1.5f, 2f, 0);
        public static readonly Vector3 STAND_OFFSET_MAX = new Vector3(1.5f, 2.3f, 0);
        
        // NPC 배열 인덱스 (주석에서 확인)
        public const int HUNTER_INDEX = 0;
        public const int CHEF_INDEX = 1;
        public const int WAITER_INDEX = 2;
        public const int CASHIER_INDEX = 3;
    }
    
    /// <summary>
    /// 건축물 관련 상수
    /// </summary>
    public static class Architecture
    {
        // 업그레이드 관련
        public const int MIN_UPGRADE_LEVEL = 1;
        public const int MAX_UPGRADE_LEVEL = 5;
        public const int SPECIAL_UPGRADE_LEVEL = 5;
        
        // 건축물 특별 ID
        public const int TUTORIAL_ARCH_ID = 8001;
        public const int TUTORIAL_TARGET_MONEY = 100;
        
        // 거리 및 스톡 관련
        public const float WEIGHT_DISTANCE = 0.6f;
        public const float WEIGHT_STOCK = 0.4f;
        public const float DISTANCE_GAMMA = 1.5f;
        public const float STOCK_GAMMA = 1.0f;
        public const float STICKINESS = 0.1f;
        public const float ETA_EPSILON = 0.1f;
        public const float RANDOM_SCORE_MULTIPLIER = 1e-4f;
        
        // 업그레이드 대기 시간
        public const float UPGRADE_WAIT_TIME = 0.5f;
    }
    
    /// <summary>
    /// 몬스터 관련 상수
    /// </summary>
    public static class Monster
    {
        // 몬스터 감지 및 공격 범위
        public const float DEFAULT_DETECTION_RANGE = 3f;
        public const float DEFAULT_ATTACK_RANGE = 2f;
        
        // 몬스터 위치 제한
        public static readonly Vector2 MONSTER_MIN_POS = new Vector2(-5f, 8f);
        public static readonly Vector2 MONSTER_MAX_POS = new Vector2(5f, 18.5f);
        
        // HP 바 스케일
        public static readonly Vector3 HP_BAR_FULL_SCALE = new Vector3(1, 1, 1);
        
        // 특별 효과 스케일
        public static readonly Vector3 RUSH_ALERT_SCALE = new Vector3(0.5f, 0f, 0.5f);
    }
    
    #endregion
    
    #region UI 관련 상수 (UI)
    
    /// <summary>
    /// UI 관련 상수
    /// </summary>
    public static class UI
    {
        // 로딩 UI 애니메이션
        public static readonly Vector3 LOADING_WITCH_SCALE = new Vector3(0.5f, 0.5f, 0.5f);
        
        // 세이브 패널 관련
        public const float SAVE_PANEL_DURATION = 1f;
        public const float SAVE_PANEL_TURN_OFF_TIME = 2f;
        
        // 돈 포맷팅
        public const int MONEY_UNIT_THRESHOLD = 1000;
        public const int MONEY_DECIMAL_PLACES = 2;
        public const string MONEY_UNITS = "abcdefghijklmnopqrstuvwxyz";
        
        // 아이템 ID
        public const string MONEY_ITEM_ID = "9999";
    }
    
    #endregion
    
    #region 튜토리얼 관련 상수 (Tutorial)
    
    /// <summary>
    /// 튜토리얼 관련 상수
    /// </summary>
    public static class Tutorial
    {
        // 화살표 위치
        public static readonly Vector3 ARROW_POSITION_1 = new Vector3(-8.38f, -5.9f, 0f);
        public static readonly Vector3 ARROW_POSITION_2 = new Vector3(0f, 1f, 0f);
        public static readonly Vector3 ARROW_POSITION_3 = new Vector3(1f, -5.4f, 0f);
        public static readonly Vector3 ARROW_POSITION_4 = new Vector3(15f, 2.05f, 0f);
        public static readonly Vector3 ARROW_POSITION_5 = new Vector3(15f, -5.9f, 0f);
        
        // 튜토리얼 NPC 생성 위치
        public static readonly Vector3 TUTORIAL_NPC_SPAWN_POSITION = new Vector3(-8, 0, 0);
    }
    
    #endregion
    
    #region 수학 상수 (Math)
    
    /// <summary>
    /// 수학 관련 상수
    /// </summary>
    public static class Math
    {
        public const int PERCENTAGE_BASE = 100;
        public const float ROUNDING_PRECISION = 100f;
        public const float EPSILON = 0.0001f;
    }
    
    #endregion
    
    #region 씬 이름 (Scene Names)
    
    /// <summary>
    /// 씬 이름 상수
    /// </summary>
    public static class SceneNames
    {
        public const string TUTORIAL_SCENE = "TutorialScene";
        public const string MAIN_GAME_SCENE = "MainGameScene";
        public const string SECOND_MAIN_GAME_SCENE = "SecondMainGameScene";
    }
    public enum NextMap
    {
        A = 1,
        B = 2,
        C = 4
    }
    #endregion
}
