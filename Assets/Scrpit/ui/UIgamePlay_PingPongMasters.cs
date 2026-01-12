using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIgamePlay_PingPongMasters : UICanvas_PingPongMasters
{
    [Header("Round Panel - GameObject thường")]
    [SerializeField] private GameObject roundPanel;
    [SerializeField] private Text roundText;
    [SerializeField] private Button startRoundButton;
    [SerializeField] private Button backToHomeButton;
    [Header("⭐ MÀU AI TRONG ROUND PANEL")]
    [Tooltip("Image hiển thị màu AI trong Round Panel")]
    [SerializeField] private Image roundPanelAIColorImage;

    [Header("Gameplay Panel - GameObject thường")]
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [Header("⭐ MÀU AI TRONG GAMEPLAY PANEL")]
    [Tooltip("Image hiển thị màu AI trong Gameplay Panel")]
    [SerializeField] private Image gameplayAIColorImage;
    [SerializeField] private Button settingsButton;

    [Header("⭐ HIỂN THỊ SỐ HIỆP THẮNG (4 icons)")]
    [Tooltip("Icon bên trái - Hiệp 1 của Player")]
    [SerializeField] private Image playerWin1Icon;
    [Tooltip("Icon bên trái - Hiệp 2 của Player")]
    [SerializeField] private Image playerWin2Icon;
    [Tooltip("Icon bên phải - Hiệp 1 của AI")]
    [SerializeField] private Image aiWin1Icon;
    [Tooltip("Icon bên phải - Hiệp 2 của AI")]
    [SerializeField] private Image aiWin2Icon;

    [Header("AI Color Sprites")]
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite yellowSprite;

    public override void Open()
    {
        base.Open();

        // ⭐ Set màu AI cho CẢ 2 PANEL
        UpdateAllAIColorImages();

        SetupButtons();

        // ⭐ CẬP NHẬT ICONS HIỆP THẮNG
        UpdateWinIcons();

        // ⭐ HIỆN ROUND PANEL DỰA VÀO HIỆP HIỆN TẠI
        ShowRoundPanel();

        Debug.Log($"✅ UIGameplay.Open() - Round {GameData_PingPongMasters.currentRound}");
    }

    void Start()
    {
        // ⭐ ĐẢM BẢO TẤT CẢ ICONS TẮT KHI BẮT ĐẦU
        if (playerWin1Icon != null) playerWin1Icon.enabled = false;
        if (playerWin2Icon != null) playerWin2Icon.enabled = false;
        if (aiWin1Icon != null) aiWin1Icon.enabled = false;
        if (aiWin2Icon != null) aiWin2Icon.enabled = false;
    }

    /// <summary>
    /// ⭐ HÀM MỚI: Hiện Round Panel với số hiệp chính xác
    /// </summary>
    public void ShowRoundPanel()
    {
        // Tắt Gameplay Panel, Bật Round Panel
        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (roundPanel != null)
        {
            roundPanel.SetActive(true);
            if (roundText != null)
            {
                roundText.text = $"#{GameData_PingPongMasters.currentRound}";
                Debug.Log($"📋 Hiện Round Panel: HIỆP #{GameData_PingPongMasters.currentRound}");
            }
        }

        // ⭐⭐⭐ CẬP NHẬT MÀU AI TRONG ROUND PANEL ⭐⭐⭐
        UpdateAllAIColorImages();

        // ⭐ Cập nhật icons khi hiện Round Panel
        UpdateWinIcons();
    }

    private void SetupButtons()
    {
        if (startRoundButton != null)
        {
            startRoundButton.onClick.RemoveAllListeners();
            startRoundButton.onClick.AddListener(OnStartRoundClicked);
        }

        if (backToHomeButton != null)
        {
            backToHomeButton.onClick.RemoveAllListeners();
            backToHomeButton.onClick.AddListener(OnBackToHomeClicked);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }
    }

    private void OnStartRoundClicked()
    {
        // Phát âm thanh
       

        // ⭐ Tắt Round Panel, Bật Gameplay Panel
        if (roundPanel != null)
            roundPanel.SetActive(false);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        // Cập nhật điểm hiển thị
        UpdateScoreDisplay();

        // ⭐ Cập nhật icons
        UpdateWinIcons();

        // Reset bóng để bắt đầu chơi
        if (GameManager_PingPongMasters.instance != null && GameManager_PingPongMasters.instance.ballController != null)
        {
            // ⭐ AI phát bóng nếu Player thắng hiệp trước
            bool aiServes = (GameData_PingPongMasters.playerTotalWins > GameData_PingPongMasters.aiTotalWins);

            GameManager_PingPongMasters.instance.ballController.gameObject.SetActive(true);
            GameManager_PingPongMasters.instance.ballController.ResetBall(Vector3.zero, aiServes);

           
        }
    }

    private void OnBackToHomeClicked()
    {
        // Phát âm thanh
        

        // Reset game data
        GameData_PingPongMasters.ResetAllData();

        // Về home
        UIManager_PingPongMasters.Instance.EnableGameplay(false);
        UIManager_PingPongMasters.Instance.EnableHome(true);
    }

    private void OnSettingsClicked()
    {
        // Phát âm thanh
       

        // Mở settings
        UIManager_PingPongMasters.Instance.EnableSettingPanel(true);
    }

    public void UpdateScoreDisplay()
    {
        if (playerScoreText != null)
            playerScoreText.text = GameData_PingPongMasters.playerRoundScore.ToString();

        if (aiScoreText != null)
            aiScoreText.text = GameData_PingPongMasters.aiRoundScore.ToString();

      
        UpdateWinIcons();
    }

    
    private void UpdateWinIcons()
    {
       
        if (playerWin1Icon != null)
            playerWin1Icon.enabled = (GameData_PingPongMasters.playerTotalWins >= 1);

        if (playerWin2Icon != null)
            playerWin2Icon.enabled = (GameData_PingPongMasters.playerTotalWins >= 2);

      
        if (aiWin1Icon != null)
            aiWin1Icon.enabled = (GameData_PingPongMasters.aiTotalWins >= 1);

        if (aiWin2Icon != null)
            aiWin2Icon.enabled = (GameData_PingPongMasters.aiTotalWins >= 2);

     
    }

  
    public void UpdateAIColorDisplay()
    {
        UpdateAllAIColorImages();
      
    }

    /// <summary>
    /// ⭐ CẬP NHẬT MÀU AI TRONG CẢ ROUND PANEL VÀ GAMEPLAY PANEL
    /// </summary>
    private void UpdateAllAIColorImages()
    {
        Sprite selectedSprite = GetAIColorSprite();

        if (selectedSprite == null)
        {
          
            return;
        }

        // ⭐ CẬP NHẬT ROUND PANEL
        if (roundPanelAIColorImage != null)
        {
            roundPanelAIColorImage.sprite = selectedSprite;
          
        }
        else
        {
            Debug.LogWarning("⚠️ roundPanelAIColorImage chưa được gán trong Inspector!");
        }

        // ⭐ CẬP NHẬT GAMEPLAY PANEL
        if (gameplayAIColorImage != null)
        {
            gameplayAIColorImage.sprite = selectedSprite;
           
        }
        else
        {
            Debug.LogWarning("⚠️ gameplayAIColorImage chưa được gán trong Inspector!");
        }
    }

    /// <summary>
    /// ⭐ LẤY SPRITE DỰA VÀO INDEX MÀU
    /// </summary>
    private Sprite GetAIColorSprite()
    {
        switch (GameData_PingPongMasters.selectedAIColor)
        {
            case 0: return redSprite;
            case 1: return blueSprite;
            case 2: return greenSprite;
            case 3: return yellowSprite;
            default:
                Debug.LogWarning($"⚠️ Invalid AI color index: {GameData_PingPongMasters.selectedAIColor}");
                return redSprite; // Default về màu đỏ
        }
    }

   
    private string GetColorName(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0: return "RED ";
            case 1: return "BLUE ";
            case 2: return "GREEN ";
            case 3: return "YELLOW ";
            default: return "UNKNOWN";
        }
    }
}