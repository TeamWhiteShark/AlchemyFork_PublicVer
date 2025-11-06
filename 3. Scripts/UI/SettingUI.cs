using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    public override bool isDestroy => false;

    public Slider BgmSlider;
    public Slider sfxSlider;
    public void Start()
    {
        AudioManager.Instance.SetSliders(BgmSlider, sfxSlider);
    }
    public override void OpenUI()
    {
        base.OpenUI();
        UIManager.Instance.isUIOn = true;
    }

    public override void CloseUI()
    {
        base.CloseUI();
        UIManager.Instance.isUIOn = false;
    }
}
