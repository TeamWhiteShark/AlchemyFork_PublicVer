using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetized : MonoBehaviour
{
    public Transform target;
    public float minSpeed = 2f;
    public float maxSpeed = 10f;
    public float accelDuration = 0.3f;
    public float stopDistance = 0.05f;
    public float elapsed = 0f;
    public int lastSeenFrame = -999999;
    
    private void Update()
    {
        if (!target) { enabled = false; return; }

        var player = target.GetComponent<Player>();
        
        if (player != null && 
            player.playerStateMachine.currentState == player.playerStateMachine.PlayerDieState)
        {
            enabled = false;
            return;
        }

        // 가속 보간
        elapsed = Mathf.Min(elapsed + Time.deltaTime, accelDuration);
        float p = accelDuration <= 0f ? 1f : elapsed / accelDuration;
        float speed = Mathf.Lerp(minSpeed, maxSpeed, p);

        // 이동
        Vector3 to = target.position - transform.position;
        float dist = to.magnitude;
        if (dist <= stopDistance)
        {
            transform.position = target.position;
            enabled = false;        // 도착
            return;
        }

        Vector3 dir = to / dist;
        float step = speed * Time.deltaTime;

        // 오버슈트 방지
        if (step >= dist) transform.position = target.position;
        else transform.position += dir * step;
    }
}

public class Magnetic : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float accelDuration;
    [SerializeField] LayerMask Item; // Inspector에서 "Item" 체크
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] private Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Item != (Item | (1 << other.gameObject.layer))) return;
        if (player.playerInventory.CurrentQuantity >= player.playerInventory.MaxQuantity) return;
        if (player.playerStateMachine.currentState == player.playerStateMachine.PlayerDieState) return;
        
        if (other.TryGetComponent(out Item item) && !item.get)
        {
            item.get = true;
            var t = item.transform;
            //if (!t) continue;
    
            var token = t.GetComponent<Magnetized>();
            if (!token) token = t.gameObject.AddComponent<Magnetized>();

            token.target       = transform;   // 플레이어 Transform
            token.minSpeed     = minSpeed;
            token.maxSpeed     = maxSpeed;
            token.accelDuration= accelDuration;
            token.enabled      = true;
        }
    }
}
