using UnityEngine;
using UnityEngine.AI;

public class WaiterMove : IMove
{
    public void Move(NPC npc, NPCStateMachine stateMachine)
    {
        if(npc.targetArchType == ArchType.Cook)
        {
            if (npc.targetObj == NPCManager.Instance.waitingPoint)
            {
                if (ArchitectureManager.Instance.GetBestArch(npc) != null)
                {
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                    return;
                }
            }
            else
            {
                if (npc.targetObj != null)
                {
                    var effective = npc.targetObj.GetComponent<BaseArchitecture>().productCount;
                    foreach (var waiter in NPCManager.Instance.npc[NPCType.Waiter])
                    {
                        if (waiter != null && waiter.targetObj == npc.targetObj)
                        {
                            var waiterETA = GetPathLength(waiter.agent, npc.targetObj.transform.position) / waiter.agent.speed;
                            var myETA = GetPathLength(npc.agent, npc.targetObj.transform.position) / npc.agent.speed;
                        
                            if(waiterETA + 0.1f < myETA)
                                effective = Mathf.Max(effective - (waiter.npcInven.MaxQuantity - waiter.npcInven.CurrentQuantity), 0);
                        }
                    }

                    if (effective == 0)
                    {
                        npc.targetObj = null;
                        stateMachine.ChangeState(stateMachine.NPCIdleState);
                        return;
                    }
                }
                
                if (ArchitectureManager.Instance.GetBestArch(npc) == null) return;
                if (npc.targetObj != ArchitectureManager.Instance.GetBestArch(npc).gameObject)
                {
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                    return;
                }
            }
        }
        
        if(npc.targetArchType == ArchType.Cook && npc.targetObj.TryGetComponent<BaseArchitecture>(out var cook) && cook.productCount == 0)
        {
            npc.targetObj = null;
            stateMachine.ChangeState(stateMachine.NPCIdleState);
            return;
        }
        
        stateMachine.NPC.distance = Vector3.Distance(stateMachine.TargetPos, stateMachine.NPC.transform.position);

        if (stateMachine.NPC.agent.CalculatePath(stateMachine.TargetPos, stateMachine.NPC.path))
        {
            if(stateMachine.NPC.distance > 1f)
                stateMachine.NPC.agent.SetDestination(stateMachine.TargetPos);
            else
                stateMachine.ChangeState(stateMachine.NPCWaitState);
        }
    }
    
    float GetPathLength(NavMeshAgent agent, Vector3 target)
    {
        if (!agent.isOnNavMesh) return Mathf.Infinity;
        var path = new NavMeshPath();
        if (!agent.CalculatePath(target, path)) return Mathf.Infinity;
        float len = 0f;
        for (int i = 1; i < path.corners.Length; i++)
            len += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        return len;
    }
}
