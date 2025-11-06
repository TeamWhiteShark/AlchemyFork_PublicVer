using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI buttonText;
    public GameObject lockImage;

    private void Awake()
    {
        backgroundImage = GetComponentInChildren<Image>();
        buttonText = backgroundImage.transform.GetComponentInChildren<TextMeshProUGUI>();
        lockImage = backgroundImage.transform.GetChild(1).gameObject;

        if (buttonText != null)
        {
            buttonText.gameObject.SetActive(false);
        }
    }

    public void UnlockFunctionOfButton()
    {
        backgroundImage.color = new Color(255f, 255f, 255f, 230f);
        buttonText.gameObject.SetActive(true);
        lockImage.SetActive(false);
    }
}
