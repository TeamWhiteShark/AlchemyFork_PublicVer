using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "MonsterStat", menuName = "GameData/CreateMonsterStatData")]

public class MonsterStatSO : ScriptableObject
{
	public int monsterID;
	public string monsterName;
	public int monsterHealth;
	public int monsterAttack;
	public int monsterSpeed;
	public float monsterAttackRate;

}
