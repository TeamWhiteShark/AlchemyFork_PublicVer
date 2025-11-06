using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObject/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int Playerhealth;
    public int PlayerattackPoint;
    public int PlayerwalkSpeed;
    public float PlayerattackRate;
    public int Money;
    public int Diamond;
    public int MaxQuantity;
    public Sprite MoneyImg;
}
