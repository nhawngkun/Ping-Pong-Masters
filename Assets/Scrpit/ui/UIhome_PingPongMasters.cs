using UnityEngine;
using UnityEngine.UI;

public class UIHome_PingPongMasters : UICanvas_PingPongMasters
{
    [Header("Difficulty Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button howToPlayButton;

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (easyButton != null)
            easyButton.onClick.AddListener(() => OnDifficultySelected(AIDifficulty.Easy));

        if (mediumButton != null)
            mediumButton.onClick.AddListener(() => OnDifficultySelected(AIDifficulty.Medium));

        if (hardButton != null)
            hardButton.onClick.AddListener(() => OnDifficultySelected(AIDifficulty.Hard));

        if (howToPlayButton != null)
            howToPlayButton.onClick.AddListener(OnHowToPlayClicked);
    }

    private void OnDifficultySelected(AIDifficulty difficulty)
    {
        // Lưu độ khó đã chọn
        GameData_PingPongMasters.selectedDifficulty = difficulty;

        Debug.Log($"✅ Đã chọn độ khó: {difficulty}");

       
        // Chuyển sang UI chọn màu AI
        UIManager_PingPongMasters.Instance.EnableHome(false);
        UIManager_PingPongMasters.Instance.EnableChooseAI(true);
    }

    private void OnHowToPlayClicked()
    {
        // Phát âm thanh
        

        UIManager_PingPongMasters.Instance.EnableHome(false);
        UIManager_PingPongMasters.Instance.EnableHowToPlay(true);
    }
}