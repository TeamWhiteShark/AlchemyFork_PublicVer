using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class LogInUI : UIBase
{
    public override bool isDestroy => false;

    [Header("로그인 / 회원가입 화면 전환")]
    [SerializeField] private GameObject logInBackground;
    [SerializeField] private GameObject signUpBackground;

    [Header("로그인 관련")] 
    [SerializeField] private TMP_InputField userIDInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("회원가입 관련")] 
    [SerializeField] private TMP_InputField signUpIDInput;
    [SerializeField] private TMP_InputField signUpPasswordInput;

    [Header("경고창 관련")] 
    [SerializeField] private GameObject warningUI;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject exitButton;

    private bool isInit = false;

    public override async void OpenUI()
    {
        base.OpenUI();
        UIManager.Instance.isUIOn = true;
        
        OpenLogIn();
        
        if (!isInit)
        {
            await InitializeUnityServices();
            isInit = true;
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
        UIManager.Instance.isUIOn = false;
    }

    public void OpenLogIn()
    {
        logInBackground.SetActive(true);
        signUpBackground.SetActive(false);
    }

    public void OpenSignUp()
    {
        logInBackground.SetActive(false);
        signUpBackground.SetActive(true);
    }

    public void OnClickExitWarningButton()
    {
        warningUI.SetActive(false);
    }

    public async void OnClickTryLogInButton()
    {
        string userID = userIDInput.text.Trim();
        string password = passwordInput.text.Trim();
        
        if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(password))
        {
            SetStatus("아이디와 비밀번호를\n입력하세요.");
            return;
        }
        
        // 이미 로그인 상태면 먼저 로그아웃
        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
        }

        SetStatus("로그인 중...", false);

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(userID, password);
            Debug.Log($"로그인 성공! PlayerID: {AuthenticationService.Instance.PlayerId}");
            SetStatus("로그인 성공!", false);

            var data = await SaveLoadManager.Instance.LoadPlayerDataFromCloudAsync();

            if (data != null)
            {
                SaveLoadManager.Instance.isClickedContinue = true;
                await SceneLoadManager.Instance.ChangeSceneAsync(data.SceneName);
            }
            else
            {
                await SceneLoadManager.Instance.ChangeSceneAsync(GameConstants.SceneNames.TUTORIAL_SCENE);
            }
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError("로그인 실패: " + ex.Message);
            SetStatus("로그인 실패");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError("요청 실패: " + ex.Message);
            SetStatus("요청 실패");
        }
    }

    public async void OnClickTrySignUpButton()
    {
        string userID = signUpIDInput.text.Trim();
        string password = signUpPasswordInput.text.Trim();
        
        if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(password))
        {
            SetStatus("아이디와 비밀번호를\n입력하세요.");
            return;
        }
        
        if (!IsValidPassword(password))
        {
            SetStatus("비밀번호는 8자 이상,\n대문자/소문자/숫자/특수\n문자를 포함해야 합니다.");
            return;
        }
        
        SetStatus("회원가입 중...");
        
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(userID, password);
            Debug.Log($"회원가입 성공! PlayerID: {AuthenticationService.Instance.PlayerId}");
            SetStatus("회원가입 성공!\n로그인 화면으로\n돌아갑니다.");

            await Task.Delay(1500);
            OpenLogIn();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError("회원가입 실패: " + ex.Message);
            SetStatus("회원가입 실패: ");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError("요청 실패: " + ex.Message);
            SetStatus("요청 실패: ");
        }
    }
    
    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("UGS 초기화 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError("UGS 초기화 실패: " + e.Message);
            SetStatus("서비스 초기화 실패. 네트워크를 확인하세요.");
        }
    }
    
    private bool IsValidPassword(string password)
    {
        if (password.Length < 8) return false;
        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else hasSpecial = true;
        }

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    private void SetStatus(string message, bool active = true)
    {
        warningUI.SetActive(true);
        exitButton.SetActive(active);

        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
