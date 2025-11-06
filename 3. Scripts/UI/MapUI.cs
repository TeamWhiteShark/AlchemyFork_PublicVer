using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapUI : UIBase
{
    public override bool isDestroy => false;

    [SerializeField] private string nextSceneName = "";
    [SerializeField] private GameObject notReadyPopup;   
    [SerializeField] private GameObject FinishPopup;
    [SerializeField] private Button btn;
    
    private InventoryButton inventoryButton;
    private int MaxNum;    

    private void Awake()
    {
        EventManager.Instance.Subscribe<NextMapEvent>(ReadyNextScene);
    }

    private void Start()
    {            
        inventoryButton = btn.GetComponent<InventoryButton>();
        btn.onClick.AddListener(NotReady);
        Debug.Log("초기화됨");        
        PlayerManager.Instance.Player.playerInventory.Money += 0; // 돈 조건 확인용    
    }

    private void OnDestroy()
    {
        // 씬 전환 등으로 MapUI 오브젝트가 파괴될 때 이벤트 구독을 해제합니다.
        // EventManager.Instance가 null이 아닐 때만 해제해야 안전합니다.
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<NextMapEvent>(ReadyNextScene);
        }
    }

    // 플레이어가 특정 조건을 충족할때마다 호출되어 다음 씬으로 이동할 수 있는지 확인
    public void ReadyNextScene(NextMapEvent e)    
    {        
        MaxNum |= e.triggerNum;
        Debug.Log("현재 누적된 조건: " + MaxNum);
        if (MaxNum == GameConstants.NextMapMaxValue)
        {            
            btn.onClick.RemoveAllListeners();
            if (SceneManager.GetActiveScene().name == "MainGameScene")
            {
                btn.onClick.AddListener(SecondMainGameSceneMove);
            }
            else
            {                
                btn.onClick.AddListener(OpenFinishPopup);                
            }
            inventoryButton.UnlockFunctionOfButton();
            MaxNum = 0;
        }
    }

    public void SecondMainGameSceneMove()
    {         
        StartCoroutine(WaitForSecondsCoroutine());
        SceneLoadManager.Instance.ChangeScene(nextSceneName);        
    }
    
    private IEnumerator WaitForSecondsCoroutine()
    {
        yield return AllDelete();
    }
    
    private IEnumerator AllDelete()
    {
        SaveLoadManager.Instance.isClickedNext = true;
        
        PlayerManager.Instance.Player.playerInventory.itemsDic.Clear();
        Debug.Log("인벤토리 초기화");
        PlayerManager.Instance.Player.playerInventory.Money = 600;
        Debug.Log("플레이어 돈 초기화");
        ArchitectureManager.Instance.counters.Clear();
        Debug.Log("카운터 돈 초기화");
        ArchitectureManager.Instance.warehouses[0].upgradeLevel = 1;
        Debug.Log("창고1 업그레이드 초기화");
        ArchitectureManager.Instance.warehouses[1].upgradeLevel = 1;
        Debug.Log("창고2 업그레이드 초기화");
        ArchitectureManager.Instance.warehouses[0].itemsDic.Clear();
        Debug.Log("창고1 초기화");
        ArchitectureManager.Instance.warehouses[1].itemsDic.Clear();
        Debug.Log("창고2 초기화");
        NPCManager.Instance.ClearAndReturnAllNPCs();
        Debug.Log("NPC 초기화");       
        SaveLoadManager.Instance.ConfirmSaveData();
        Debug.Log("세이브 데이터 초기화");
        yield return null;
    }

    public void NotReady()
    { 
        notReadyPopup.SetActive(true);
    }

    public void CloseNotReady()
    {
        notReadyPopup.SetActive(false);
    }

    public void OpenFinishPopup()
    {
        FinishPopup.SetActive(true);
    }

    public void CloseFinishPopup()
    {
        FinishPopup.SetActive(false);
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
