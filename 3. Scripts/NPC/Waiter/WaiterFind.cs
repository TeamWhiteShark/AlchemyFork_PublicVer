using UnityEngine;

public class WaiterFind : IFind
{
    private BaseArchitecture targetArch;

    public GameObject FindTarget(NPC npc, NPCStateMachine stateMachine, ItemSO item, ArchType archType)
    {
        switch (archType)
        {
            case ArchType.Cook:
                targetArch = ArchitectureManager.Instance.GetBestArch(npc);
                if (targetArch == null)
                {
                    
                    stateMachine.TargetPos = npc.transform.position.x < NPCManager.Instance.waitingPoint.transform.position.x + 0.5f ? 
                        NPCManager.Instance.leftRelaxPos.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) : NPCManager.Instance.rightRelaxPos.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    return NPCManager.Instance.waitingPoint;
                }
                
                
                if (targetArch != null && targetArch.TryGetComponent(out BaseArchitecture arch))
                {
                    if (arch.productCount != 0)
                    {
                        npc.homeItem = targetArch.productData;
                        npc.awayItem = targetArch.productData;
                        npc.targetItem = npc.homeItem;
                        stateMachine.TargetPos = targetArch.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.7f, -2f));
                        
                        return targetArch.gameObject;
                    }
                }
                
                return null;
            
            case ArchType.Stand:
                if (!ArchitectureManager.Instance.stands.ContainsKey(item)) return null;
                stateMachine.TargetPos = ArchitectureManager.Instance.stands[item].gameObject.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(2f, 2.3f));
                return ArchitectureManager.Instance.stands[item].gameObject; 
            default:
                return null;
        }
    }
}
