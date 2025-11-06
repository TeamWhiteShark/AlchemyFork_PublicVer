using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    //[SerializeField] private Player player;
    public Player player;
    private Animator animator;

    [SerializeField] private AudioClip PlayerAttack;

    private List<IDamagable> _monsterList = new List<IDamagable>();

    public bool IsAttack => _monsterList.Count > 0;

    private void Awake()
    {
        //player = GetComponentInParent<Player>();
        player = PlayerManager.Instance.Player;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool("IsAttack", IsAttack);
        if (IsAttack == true)
        {
            PlayerManager.Instance.Player.rotationSpeed = 1;
        }
        else
        {
            PlayerManager.Instance.Player.rotationSpeed = 10;
        }
    }

    //일정 간격으로 데미지 입히기
    private void OnEnable()
    {
        InvokeRepeating(nameof(DealDamage), 0, player.playerCondition.totalAtkRate);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(DealDamage));
    }

    //리스트 내부의 몬스터들에게 데미지 입히기
    private void DealDamage()
    {
        for (int i = 0; i < _monsterList.Count; i++)
        {
            _monsterList[i].GetDamage(player.playerCondition.totalAtk);
        }
        if (IsAttack == true)
        {
            AudioManager.Instance.PlaySFX(PlayerAttack);
        }
    }

    //몬스터 리스트에 추가/제거
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            if (other.CompareTag("Player"))
                return;

            _monsterList.Add(damagable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            if (_monsterList.Contains(damagable))
            {
                _monsterList.Remove(damagable);
            }
        }
    }
}