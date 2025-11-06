using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    public GameObject settingPanel;    

    public void OnSetting()
    {
        if (UIManager.Instance.isUIOn == false)
        {
            UIManager.Instance.OpenUI<SetPanelUI>();
        }
    }
    
    public void ExitSetting()
    {
        settingPanel.SetActive(false);
    }
}
