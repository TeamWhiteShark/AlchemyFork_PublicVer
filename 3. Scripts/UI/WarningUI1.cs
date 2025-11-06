using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningUI1 : UIBase
{
    public override bool isDestroy => true;

    private string warningText;
    
    protected override void OnOpen()
    {
        StartCoroutine(CloseUI());
    }

    private IEnumerator CloseUI()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.CloseUI<WarningUI1>();
    }
}
