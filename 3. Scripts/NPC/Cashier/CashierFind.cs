using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CashierFind : IFind
{
    public GameObject FindTarget(NPC npc, NPCStateMachine stateMachine, ItemSO item, ArchType archType)
    {
        switch (archType)
        {
            case ArchType.Counter:
                if(SceneManager.GetActiveScene().name == GameConstants.SceneNames.SECOND_MAIN_GAME_SCENE)
                {
                    foreach (var cashier in NPCManager.Instance.npc[NPCType.Cashier])
                    {
                        if (cashier.targetObj == ArchitectureManager.Instance.counters[5002].gameObject &&
                            npc != cashier)
                        {
                            stateMachine.TargetPos = ArchitectureManager.Instance.counters[5003].GetComponent<Counter>()
                                .cashierZone.transform.position;
                            return ArchitectureManager.Instance.counters[5003].gameObject;
                        }
                    }

                    stateMachine.TargetPos = ArchitectureManager.Instance.counters[5002].GetComponent<Counter>()
                        .cashierZone.transform.position;
                    return ArchitectureManager.Instance.counters[5002].gameObject;
                }
                else
                {
                    stateMachine.TargetPos = ArchitectureManager.Instance.counters[5001].GetComponent<Counter>()
                        .cashierZone.transform.position;
                    return ArchitectureManager.Instance.counters[5001].gameObject;
                }
            default:
                return null;
        }
    }
}
