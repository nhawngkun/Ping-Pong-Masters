using UnityEngine;
using UnityEngine.UI;

public class UISetting_PingPongMasters : UICanvas_PingPongMasters
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Fill Images")]
    [SerializeField] private Image musicFillImage;
    [SerializeField] private Image sfxFillImage;

    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button homeButton; // ⭐ NÚT HOME
    [SerializeField] private Button resetButton;

    private void Start()
    {
        InitializeVolume();
        SetupSliders();
        SetupButtons();
    }

    private void InitializeVolume()
    {
        if (SoundManager_PingPongMasters.Instance == null) return;

        float musicVol = SoundManager_PingPongMasters.Instance.GetMusicVolume();
        float sfxVol = SoundManager_PingPongMasters.Instance.GetSFXVolume();

        if (musicSlider != null)
            musicSlider.value = musicVol;

        if (sfxSlider != null)
            sfxSlider.value = sfxVol;

        if (musicFillImage != null)
            musicFillImage.fillAmount = musicVol;

        if (sfxFillImage != null)
            sfxFillImage.fillAmount = sfxVol;
    }

    private void SetupSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }
    }

    private void SetupButtons()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackClicked);
        }

        // ⭐ SỬA NÚT HOME
        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnHomeClicked);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(OnResetClicked);
        }
    }

    public void OnMusicSliderChanged(float value)
    {
        if (SoundManager_PingPongMasters.Instance != null)
            SoundManager_PingPongMasters.Instance.SetMusicVolume(value);

        if (musicFillImage != null)
            musicFillImage.fillAmount = value;
    }

    public void OnSFXSliderChanged(float value)
    {
        if (SoundManager_PingPongMasters.Instance != null)
            SoundManager_PingPongMasters.Instance.SetSFXVolume(value);

        if (sfxFillImage != null)
            sfxFillImage.fillAmount = value;
    }

    private void OnBackClicked()
    {
        // Phát âm thanh
        if (SoundManager_PingPongMasters.Instance != null)
            SoundManager_PingPongMasters.Instance.PlayVFXSound(1);

        // Resume game
        Time.timeScale = 1f;

        // Đóng settings
        UIManager_PingPongMasters.Instance.EnableSettingPanel(false);
    }

    /// <summary>
    /// ⭐ NÚT HOME - VỀ TRANG CHỦ
    /// </summary>
    private void OnHomeClicked()
    {
        // Phát âm thanh
        if (SoundManager_PingPongMasters.Instance != null)
            SoundManager_PingPongMasters.Instance.PlayVFXSound(1);

        // Resume game
 

        // Reset tất cả data
        GameData_PingPongMasters.ResetAllData();
         if (GameManager_PingPongMasters.instance != null)
        {
            GameManager_PingPongMasters.instance.RestartGame();
        }

        // Đóng settings
        UIManager_PingPongMasters.Instance.EnableSettingPanel(false);

        // Đóng gameplay nếu đang mở
        UIManager_PingPongMasters.Instance.EnableGameplay(false);

        // Về Home
        UIManager_PingPongMasters.Instance.EnableHome(true);

        Debug.Log("🏠 Đã về Home từ Settings");
    }

    private void OnResetClicked()
    {
        // Phát âm thanh
        if (SoundManager_PingPongMasters.Instance != null)
            SoundManager_PingPongMasters.Instance.PlayVFXSound(0);

        // Resume game
        Time.timeScale = 1f;

        // Reset game data
        GameData_PingPongMasters.ResetAllData();

        // Reset điểm trong GameManager
        if (GameManager_PingPongMasters.instance != null)
        {
            GameManager_PingPongMasters.instance.RestartGame();
        }

        // Đóng settings
        UIManager_PingPongMasters.Instance.EnableSettingPanel(false);
        // Quay lại màn hình round
        UIgamePlay_PingPongMasters gameplayUI = UIManager_PingPongMasters.Instance.GetUI<UIgamePlay_PingPongMasters>();
        if (gameplayUI != null)
        {
            gameplayUI.Open(); // Reset về hiệp 1
        }

        Debug.Log("🔄 Đã Reset game từ Settings");
    }
}