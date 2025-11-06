using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefFind : IFind
{
    public GameObject FindTarget(NPC npc, NPCStateMachine stateMachine, ItemSO item, ArchType archType)
    {
        switch (archType)
        {
            case ArchType.Cook:
                if (!ArchitectureManager.Instance.cooks.ContainsKey(item)) return null;
                stateMachine.TargetPos = ArchitectureManager.Instance.cooks[item].gameObject.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, -2f));
                return ArchitectureManager.Instance.cooks[item].gameObject;
            case ArchType.Warehouse:
                switch (npc.objType)
                {
                    case ObjType.Mushroom:
                        if (ArchitectureManager.Instance.warehouses[0].CurrentQuantity == 0 && ArchitectureManager.Instance.warehouses[1].CurrentQuantity != 0)
                        {
                            npc.objType = ObjType.Meat;
                            return null;
                        }
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
