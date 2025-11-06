using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    // Entity는 플레이어와 몬스터의 공통적인 스탯 정보를 담아둔다.
    
    [SerializeField] public int health;
    public int attackPoint;
    public int walkSpeed;
    public float attackRate;
    
    public int Health 
    {
        get => health; 
        protected set => health = value; 
    }

    public int AttackPoint
    {
        get => attackPoint; 
        protected set => attackPoint = value;
    }

    public int WalkSpeed
    {
        get => walkSpeed; 
        protected set => walkSpeed = value;
    }

    public float AttackRate
    {
        get => attackRate; 
        protected set => attackRate = value;
    }
}
