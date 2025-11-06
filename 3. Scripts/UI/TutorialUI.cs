using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : UIBase
{
    public override bool isDestroy => false;

    [SerializeField] private List<GameObject> tutorialSlots = new List<GameObject>();  
    private int currentIndex = 0;

    [SerializeField] TMP_Text currentPage;
    [SerializeField] TMP_Text totalPage;

    private void Start()
    {
        ShowPage(currentIndex);
        currentPage.text = (currentIndex + 1).ToString();
        totalPage.text = tutorialSlots.Count.ToString();
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
    
    public void OnClickCloseButton()
    {
        Debug.Log("TutorialUI Close Button Clicked");
        CloseUI();
    }

    public void NextBtn()
    {
        if (currentIndex < tutorialSlots.Count - 1)
        {
            currentIndex++;
            ShowPage(currentIndex);
            currentPage.text = (currentIndex + 1).ToString();
        }
    }
    public void PrevBtn()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowPage(currentIndex);
            currentPage.text = (currentIndex + 1).ToString();
        }
    }
    private void ShowPage(int index)
    {        
        foreach (var slot in tutorialSlots)
            slot.SetActive(false);
        
        tutorialSlots[index].SetActive(true);
    }
}
