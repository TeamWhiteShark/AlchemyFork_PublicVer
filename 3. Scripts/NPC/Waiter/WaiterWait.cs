using UnityEngine;

public class WaiterWait : IWait
{
    public void Decide(NPC npc, NPCStateMachine stateMachine)
    {
        if (npc.targetItem == npc.homeItem && npc.targetArchType == ArchType.Cook)
        {
            if (ArchitectureManager.Instance.GetBestArch(npc) != null && ArchitectureManager.Instance.GetBestArch(npc).gameObject != npc.targetObj)
            {
                npc.targetObj = null;
                stateMachine.ChangeState(stateMachine.NPCIdleState);
                return;
            }
            
            if (npc.targetObj.TryGetComponent(out BaseArchitecture targetArch) && targetArch.productCount > 0)
            {
                stateMachine.ChangeState(stateMachine.NPCInteractState);
            }
            else if(npc.targetObj.TryGetComponent(out BaseArchitecture arch) && arch.productCount == 0)
            {
                if (ArchitectureManager.Instance.GetBestArch(npc) != null)
                {
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                }
            }
        }
        else if (npc.targetItem == npc.awayItem && npc.targetArchType == ArchType.Stand)
        {
            if (ArchitectureManager.Instance.stands.ContainsKey(npc.targetItem))
            {
                stateMachine.ChangeState(stateMachine.NPCInteractState);
            }
        }
    }
}
