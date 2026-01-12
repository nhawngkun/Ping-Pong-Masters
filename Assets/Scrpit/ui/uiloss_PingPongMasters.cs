using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILoss_PingPongMasters : UICanvas_PingPongMasters
{
    [Header("UI Elements")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Image aiColorImage;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button backToHomeButton;

    [Header("AI Color Sprites")]
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite yellowSprite;

    public override void Open()
    {
        base.Open();

        // ⭐⭐⭐ PHÁT ÂM THANH THUA (SOUND INDEX 2) ⭐⭐⭐
        if (SoundManager_PingPongMasters.Instance != null)
        {
            SoundManager_PingPongMasters.Instance.PlayVFXSound(2);
            Debug.Log("🔊 Phát âm thanh Loss (sound index 2)");
        }
        
        UpdateDisplay();
        SetupButtons();
    }

    private void UpdateDisplay()
    {
        GameData_PingPongMasters.aiTotalWins++;
        Debug.Log($"✅ AI thắng hiệp {GameData_PingPongMasters.currentRound}! Tổng: Player {GameData_PingPongMasters.playerTotalWins} - AI {GameData_PingPongMasters.aiTotalWins}");

        bool isFinalLoss = (GameData_PingPongMasters.aiTotalWins >= 2);

        if (scoreText != null)
        {
            if (isFinalLoss)
            {
                scoreText.text = $"{GameData_PingPongMasters.playerTotalWins} - {GameData_PingPongMasters.aiTotalWins}";
                Debug.Log($"🏆 Tỉ số hiệp cuối cùng: {GameData_PingPongMasters.playerTotalWins} - {GameData_PingPongMasters.aiTotalWins}");
            }
            else
            {
                scoreText.text = $"{GameData_PingPongMasters.playerRoundScore} - {GameData_PingPongMasters.aiRoundScore}";
            }
        }

        UpdateAIColorImage();

        if (isFinalLoss)
        {
            Debug.Log("🏆 AI THẮNG CHUNG CUỘC Bo3 (2 hiệp)!");
        }
        else
        {
            Debug.Log($"📋 Tỉ số hiệp: Player {GameData_PingPongMasters.playerTotalWins} - AI {GameData_PingPongMasters.aiTotalWins} → Chơi tiếp hiệp {GameData_PingPongMasters.currentRound + 1}");
        }

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(!isFinalLoss);
        }

        if (homeButton != null)
        {
            homeButton.gameObject.SetActive(!isFinalLoss);
        }

        if (backToHomeButton != null)
        {
            backToHomeButton.gameObject.SetActive(isFinalLoss);
        }
    }

    private void UpdateAIColorImage()
    {
        if (aiColorImage == null) return;

        switch (GameData_PingPongMasters.selectedAIColor)
        {
            case 0:
                aiColorImage.sprite = redSprite;
                break;
            case 1:
                aiColorImage.sprite = blueSprite;
                break;
            case 2:
                aiColorImage.sprite = greenSprite;
                break;
            case 3:
                aiColorImage.sprite = yellowSprite;
                break;
        }
    }

    private void SetupButtons()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnHomeClicked);
        }

        if (backToHomeButton != null)
        {
            backToHomeButton.onClick.RemoveAllListeners();
            backToHomeButton.onClick.AddListener(OnHomeClicked);
        }
    }

    private void OnContinueClicked()
    {
        if (SoundManager_PingPongMasters.Instance != null)
            SoundManager_PingPongMasters.Instance.PlayVFXSound(0);

        Debug.Log($"🎮 Ấn Continue từ Loss UI - Chuẩn bị hiệp {GameData_PingPongMasters.currentRound + 1}");

        UIManager_PingPongMasters.Instance.EnableLoss(false);

        GameData_PingPongMasters.ResetRoundScores();
        GameData_PingPongMasters.currentRound++;

        Debug.Log($"🏁 Bắt đầu hiệp {GameData_PingPongMasters.currentRound} - Tỉ số tổng: Player {GameData_PingPongMasters.playerTotalWins} - AI {GameData_PingPongMasters.aiTotalWins}");

        UIgamePlay_PingPongMasters gameplayUI = UIManager_PingPongMasters.Instance.GetUI<UIgamePlay_PingPongMasters >();
        if (gameplayUI != null)
        {
            gameplayUI.ShowRoundPanel();
        }

        UIManager_PingPongMasters.Instance.EnableGameplay(true);
    }

    private void OnHomeClicked()
    {
        if (SoundManager_PingPongMasters.Instance != null)
            SoundManager_PingPongMasters.Instance.PlayVFXSound(1);

        Time.timeScale = 1f;

        GameData_PingPongMasters.ResetAllData();

        UIManager_PingPongMasters.Instance.EnableLoss(false);

        UIManager_PingPongMasters.Instance.EnableHome(true);
    }
}