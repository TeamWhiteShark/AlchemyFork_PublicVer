using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Godmod : MonoBehaviour
{
    private bool isClick;
    private void Update()
    {
        if (isClick)
            ShowMeTheMoneyBtn();
    }

    public void GoToNextScene()
    {
        SceneLoadManager.Instance.ChangeScene("SecondMainGameScene");
    }

    public void ShowMeTheMoneyBtn()
    {
        PlayerManager.Instance.Player.playerInventory.Money += 1000000;
        PlayerManager.Instance.Player.playerInventory.diamond += 1000;
    }

    public void PointerDown()
    {
        isClick = true;
    }

    public void PointerUp()
    {
        isClick = false;
    }
}
