using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pet", menuName = "GameData/CreatePetData")]

public class PetSO : ScriptableObject
{
    public int PetID;
    public string PetName;    
    public int PetAttack;
    public int PetSpeed;
    public int PetInven;
    public Sprite PetImage;
    public GameObject PetPrefab;
    public Vector3 PetOffset;
    public string PetInfo;
    public DateTime StartTime;
    public float DurationTime;
    public float ExtraTime;
}
