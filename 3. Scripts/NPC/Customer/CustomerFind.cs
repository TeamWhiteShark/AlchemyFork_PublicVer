using UnityEngine;

public class CustomerFind : IFind
{
    public GameObject FindTarget(NPC npc, NPCStateMachine stateMachine, ItemSO item, ArchType archType)
    {
        switch (archType)
        {
            case ArchType.Cook:
                if (!ArchitectureManager.Instance.cooks.ContainsKey(item)) return null;
                stateMachine.TargetPos = ArchitectureManager.Instance.cooks[item].gameObject.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, -2f));
                return ArchitectureManager.Instance.cooks[item].gameObject;
            case ArchType.Stand:
                if (!ArchitectureManager.Instance.stands.ContainsKey(item)) return null;
                stateMachine.TargetPos = ArchitectureManager.Instance.stands[item].gameObject.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, -2f));
                return ArchitectureManager.Instance.stands[item].gameObject; 
            case ArchType.Counter:
                foreach (var counter in ArchitectureManager.Instance.counters.Values)
                {
                    if(counter.GetComponent<Counter>().calculateItems.Contains(npc.homeItem))
                    {
                        var interactZone = counter.GetComponentInChildren<InteractZone>();
                        stateMachine.TargetPos = counter.transform.position + new Vector3(Random.Range(interactZone.minXpos, interactZone.maxXpos), Random.Range(interactZone.minYpos, interactZone.maxYpos));
                        return counter.gameObject;
                    }
                }
                
                return null;
            default:
                return null;
        }
    }
}
