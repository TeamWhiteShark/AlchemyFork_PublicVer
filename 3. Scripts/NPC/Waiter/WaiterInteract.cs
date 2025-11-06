public class WaiterInteract : IInteract
{
    
    public void Interact(NPC npc, NPCStateMachine stateMachine)
    {
        switch (npc.targetArchType)
        {
            case ArchType.Cook:
                if (npc.npcInven.CurrentQuantity == npc.npcInven.MaxQuantity
                     || (ArchitectureManager.Instance.cooks[npc.targetItem].productCount == 0 && npc.npcInven.CurrentQuantity != 0))
                {
                    npc.targetItem = npc.awayItem;
                    npc.targetArchType = ArchType.Stand;
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                }
                else if (npc.targetObj.TryGetComponent(out BaseArchitecture arch) && arch.productCount == 0 && npc.npcInven.CurrentQuantity == 0)
                {
                    npc.targetItem = null;
                    npc.homeItem = null;
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                }

                break;
            case ArchType.Stand:
                if (npc.npcInven.CurrentQuantity == 0)
                {
                    npc.targetItem = npc.homeItem;
                    npc.targetArchType = ArchType.Cook;
                    npc.targetObj = null;
                    stateMachine.ChangeState(stateMachine.NPCIdleState);
                }

                break;
        }
    }
}
