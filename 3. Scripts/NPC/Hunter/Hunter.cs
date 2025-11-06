using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : NPC
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject rayObj;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayLength = 0.8f;
    [SerializeField] private float checkInterval = 0.05f;
    
    [SerializeField] private float triggerDuration = 0.5f;
    
    [SerializeField] private CircleCollider2D circleCollider;
    
    bool isChecking = false;
    
    private WaitForSeconds wait;
    private Vector3 move;

    public bool inDungeon;
    private float rotationSpeed;
    private float angle;

    protected override void Awake()
    {
        base.Awake();
        wait = new WaitForSeconds(checkInterval);
        InitTarget();
    }

    private void InitTarget()
    {
        homeItem = NPCManager.Instance.stageData.orderItems[0].recipe[0];
        foreach (var monsterData in EnemyManager.Instance.monsters.Keys)
        {
            if(monsterData.dropItem == homeItem)
            {
                targetMonsterData = monsterData;
                break;
            }
        }
    }

    private IEnumerator Check()
    {
        isChecking = true;
            var dir = Vector2.up;
            
            if(agent && agent.desiredVelocity.sqrMagnitude > 0.0001f)
                dir = new Vector2(agent.desiredVelocity.x, agent.desiredVelocity.z).normalized;

            var origin = (Vector2)transform.position + dir * 0.5f;
            
            var hit = Physics2D.Raycast(origin, dir, rayLength, layerMask);

            if (hit.collider != null && hit.collider.gameObject != this.gameObject && !inDungeon && NPCState == "Move")
            {
                circleCollider.isTrigger = true;
                yield return new WaitForSeconds(triggerDuration);
                circleCollider.isTrigger = false;
            }

        yield return wait;
        isChecking = false;
    }
    
    protected override void Update()
    {
        base.Update();
        if (!isChecking)
            StartCoroutine(Check());
        
        
        weapon.SetActive(inDungeon);
        if (targetObj != null && Vector3.Distance(targetObj.transform.position, transform.position) < 1f)
        {
            move = targetObj.transform.position - transform.position;            
        }
        else
        {
            move = agent.desiredVelocity;            
        }
        
        sprite.flipX = move.x < 0;

        if (move.sqrMagnitude > 0.0001f)
        {
             angle =  Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg;

             //angle = Vector2.SignedAngle(Vector3.right, move);
             Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
             
             rotationSpeed = 10f;
             
             
             weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation,
                 targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
