using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConditionUI : UIBase
{
    [SerializeField] private TextMeshProUGUI conditionText;
    public ArchSpawner archSpawner;
    
    public override bool isDestroy => true;

    protected override void OnOpen()
    {
        conditionText.text = archSpawner.archData.conditionText;
    }
}
