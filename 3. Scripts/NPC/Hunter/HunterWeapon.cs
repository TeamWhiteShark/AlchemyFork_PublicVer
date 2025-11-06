using System.Collections.Generic;
using UnityEngine;

public class HunterWeapon : MonoBehaviour
{
    private Hunter hunter;
    private Animator animator;
    private float attackCooldown = 0f;

    private List<IDamagable> _monsterList = new List<IDamagable>();

    public bool IsAttack => _monsterList.Count > 0;

    private void Awake()
    {
        hunter = GetComponentInParent<Hunter>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        if (IsAttack && attackCooldown <= 0f)
        {
            animator.SetBool("IsAttack", true);
            attackCooldown = 1f;
        }
        else
        {
            animator.SetBool("IsAttack", false);
        }
    }

    // 무기를 껏다킬꺼니까
    private void OnEnable()
    {
        InvokeRepeating(nameof(DealDamage), 0, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(DealDamage));
    }

    private void DealDamage()
    {
        for (int i = 0; i < _monsterList.Count; i++)
        {
            _monsterList[i].GetDamage(NPCManager.Instance.hunterTotalAttack);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            if (other.CompareTag("NPC") || other.CompareTag("Player"))
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
