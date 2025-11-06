using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
   Plant,
   Animal
}

public enum MonsterBehaviorType
{
    Aggressive,  // 공격형 (플레이어에게 데미지를 받으면 플레이어를 공격)
    Cowardly     // 겁쟁이 (플레이어에게 데미지를 받으면 플레이어로부터 도망)
}

[CreateAssetMenu(menuName = "Monster/monster")]
public class MonsterData : PoolData
{
    public MonsterBehaviorType behaviorType;
    public MonsterType monsterType;
    public string monsterName;
    public int monsterID;
    public int monsterHealth;
    public int monsterAttack;
    public int monsterSpeed;
    public float monsterAttackRate; 
    public ItemSO dropItem; 
    public Sprite sprite;
}
