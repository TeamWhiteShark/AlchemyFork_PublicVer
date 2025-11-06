using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : UIBase
{
    public override bool isDestroy => true;
    
    private static readonly char[] moneyUnits = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    public BaseArchitecture baseArchitecture;
    
    public TextMeshProUGUI architectureType;
    public TextMeshProUGUI productName;
    public TextMeshProUGUI archLevel;
    public TextMeshProUGUI upgradeType;
    public TextMeshProUGUI currentValue;
    public TextMeshProUGUI nextValue;
    public TextMeshProUGUI upgradePrice;
    public Button upgradeButton;

    public bool isPressed;
    public bool isHold;

    private float holdTime = 0.25f;
    
    private Coroutine holdCoroutine;
    private Coroutine repeatCoroutine;
    
    protected override void OnOpen()
    {
        ResetUI();
        PlayerManager.Instance.Player.playerController.canMove = false;
    }
    protected override void OnClose()
    {
        PlayerManager.Instance.Player.playerController.canMove = true;
    }

    public void PointerDown()
    {
        isPressed = true;
        isHold = false;
        if (holdCoroutine == null)
            holdCoroutine = StartCoroutine(holdCheck());
    }

    private IEnumerator holdCheck()
    {
        float start = Time.time;

        while (isPressed)
        {
            float now = Time.time;
            if (now - start >= holdTime)
            {
                isHold = true;
                if (repeatCoroutine == null)
                    repeatCoroutine = StartCoroutine(RepeatUpgrate());
                break;
            }
            yield return null;
        }
        
        holdCoroutine = null;
    }

    private IEnumerator RepeatUpgrate()
    {
        while (isPressed)
        {
            if(baseArchitecture.Upgrade())
            {
                yield return new WaitForSeconds(baseArchitecture.upgradeWaitTime);
                baseArchitecture.upgradeWaitTime *= 0.8f;
            }
            else
            {
                break;
            }
            yield return null;
        }
        
        repeatCoroutine = null;
    }

    public void PointerUp()
    {
        if(isPressed && !isHold)
            baseArchitecture.Upgrade(); isPressed  = false;
        isHold = false;
    }

    public void ResetUI()
    {
        architectureType.text = baseArchitecture.archName;
        productName.text = baseArchitecture.productName;
        archLevel.text = baseArchitecture.upgradeLevel.ToString();
        upgradeType.text = baseArchitecture.upgradeType;
        if (baseArchitecture.archType == ArchType.Counter)
        {
            currentValue.text = baseArchitecture.currentValue.ToString("F3");
            nextValue.text = baseArchitecture.upgradeLevel == baseArchitecture.maxLevel ? "Max" : baseArchitecture.nextValue.ToString("F3");
        }
        else
        {
            currentValue.text = Utils.MoneyFormat((int)baseArchitecture.currentValue);
            nextValue.text = baseArchitecture.upgradeLevel == baseArchitecture.maxLevel ? "Max" : Utils.MoneyFormat((int)baseArchitecture.nextValue);
        }
        upgradePrice.text = baseArchitecture.upgradeLevel == baseArchitecture.maxLevel ? "Max" : Utils.MoneyFormat(baseArchitecture.upgradePrice);
    }
    
}
