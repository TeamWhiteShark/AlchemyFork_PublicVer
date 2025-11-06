using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningUI : UIBase
{
    public override bool isDestroy => true;

    private string warningText;
    
    protected override void OnOpen()
    {
        StartCoroutine(CloseUICoroutine());
    }

    private IEnumerator CloseUICoroutine()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.CloseUI<WarningUI>();
    }
}
