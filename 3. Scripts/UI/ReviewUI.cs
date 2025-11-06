using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewUI : UIBase
{
    public override bool isDestroy => false;

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

    public void OnClickExitButton()
    {
        CloseUI();
    }

    public void OnClickReviewButton()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdT5TGzNq5aCbtChv3JTZR7gcd_6Urt0T69rEZFOhc0l9NcVQ/formResponse");
    }
}
