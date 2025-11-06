using UnityEngine;

public class HunterFind : IFind
{
    private Monster target;

    public GameObject FindTarget(NPC npc, NPCStateMachine stateMachine, ItemSO item, ArchType archType)
    {
        switch (archType)
        {
            case ArchType.DungeonWall:
                target = EnemyManager.Instance.GetNearestMonster(npc.targetMonsterData, npc.transform.position);
                if (target == null) return null;
                
                target.GetComponent<IPoolable>().OnBeforeReturn += obj =>
                {
                    npc.targetObj = null;
                    npc.agent.ResetPath();
                };
                return target.gameObject;

            case ArchType.Warehouse:
                if (npc.targetItem == null)
                {
                    if(npc.npcInven.CurrentQuantity > 0)
                    {
                        var n = Random.Range(0, 2);
                        stateMachine.TargetPos = ArchitectureManager.Instance.warehouses[n].transform.position +
                                                 new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-2.5f, -3f));
                        return ArchitectureManager.Instance.warehouses[n].gameObject;
                    }

                    stateMachine.TargetPos = NPCManager.Instance.waitingPoint.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-0.5f, -1.5f));
                    return NPCManager.Instance.waitingPoint;
                }
                
                switch (npc.objType)
                {
                    case ObjType.Mushroom:
                        stateMachine.TargetPos = ArchitectureManager.Instance.warehouses[0].transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-2.5f, -3f));
                        return ArchitectureManager.Instance.warehouses[0].gameObject;
                    case ObjType.Meat:
                        stateMachine.TargetPos = ArchitectureManager.Instance.warehouses[1].transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-2.5f, -3f));
                        return ArchitectureManager.Instance.warehouses[1].gameObject;
                    default:
                        return null;
                }
                
            default:
                return null;
        }
    }
    
}
