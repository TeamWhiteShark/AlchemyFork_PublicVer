using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetPanelUI : UIBase
{
    public override bool isDestroy => false;

    public GameObject exitGameButton;
    public GameObject closeButtonForIntro;
    public GameObject closeButtonForGame;

    public Slider bgmSlider;
    public Slider sfxSlider;

    public override void OpenUI()
    {
        base.OpenUI();
        UIManager.Instance.isUIOn = true;
        gameObject.SetActive(true);                

        AudioManager.Instance.SetSliders(bgmSlider, sfxSlider);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            exitGameButton.SetActive(false);
            closeButtonForIntro.SetActive(true);
            closeButtonForGame.SetActive(false);
        }
        else
        {
            exitGameButton.SetActive(true);
            closeButtonForIntro.SetActive(false);
            closeButtonForGame.SetActive(true);
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
        UIManager.Instance.isUIOn = false;
        gameObject.SetActive(false);
    }

    public void OnClickExitGameButton()
    {
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            SaveLoadManager.Instance.ConfirmSaveData();
        }

        UIManager.Instance.isUIOn = false;
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        //SceneManager.LoadScene(0);
#elif UNITY_WEBGL
        SceneManager.LoadScene(0);
#else
        Application.Quit();
#endif
    }

    public void OnClickExitSetPanelButton()
    {
        CloseUI();
    }
}
