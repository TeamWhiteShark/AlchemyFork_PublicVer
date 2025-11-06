using UnityEngine;

/// <summary>
/// 건축물 업그레이드 시 발생하는 이벤트 데이터입니다.
/// </summary>
public struct ArchitectureUpgradedEvent
{
    public BaseArchitecture Architecture; // 업그레이드된 건축물 참조
    // 필요하다면 업그레이드 레벨 등 추가 정보 포함 가능
}

/// <summary>
/// 카운터에서 고객 계산 완료 시 발생하는 이벤트 데이터입니다. (튜토리얼용)
/// </summary>
public struct CustomerCalculatedEvent
{
    // 필요하다면 계산 관련 정보 포함 가능 (예: 고객, 금액 등)
}

/// <summary>
/// Cook 건축물에서 아이템 생산 완료 시 발생하는 이벤트 데이터입니다.
/// </summary>
public struct ItemProducedEvent
{
    public Cook CookInstance; // 아이템을 생산한 Cook 인스턴스
    public ItemSO ProducedItem; // 생산된 아이템 데이터
    // 필요하다면 생산 수량 등 추가 정보 포함 가능
}

/// <summary>
/// 몬스터가 죽었을 때 발생하는 이벤트 데이터입니다.
/// </summary>
public struct MonsterDiedEvent
{
    public MonsterCondition MonsterCondition; // 죽은 몬스터의 MonsterCondition 컴포넌트
    // 필요하다면 몬스터 ID, 위치 등 추가 정보 포함 가능
}

/// <summary>
/// 몬스터가 피해를 입었을 때 발생하는 이벤트 데이터입니다.
/// </summary>
public struct MonsterDamagedEvent
{
    public MonsterCondition MonsterCondition; // 피해 입은 몬스터의 MonsterCondition 컴포넌트
    public int DamageAmount; // 입은 피해량 (선택적)
}

/// <summary>
/// 몬스터가 스폰되었을 때 발생하는 이벤트 데이터입니다.
/// </summary>
public struct MonsterSpawnedEvent
{
    public int PrefabIndex; // 스폰된 몬스터의 프리팹 인덱스
    public Monster MonsterInstance; // 스폰된 몬스터 인스턴스
}

/// <summary>
/// 로딩 UI 페이드아웃 완료 시 발생하는 이벤트 데이터입니다.
/// </summary>
public struct LoadingFadeOutCompletedEvent
{
    // 필요하다면 추가 정보 포함 가능
}

public struct NextMapEvent
{
    public int triggerNum;
}