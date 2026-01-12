using UnityEngine;
using UnityEngine.UI;

public class UIchooseAI_PingPongMasters : UICanvas_PingPongMasters
{
    [Header("Color Buttons")]
    [SerializeField] private Button redButton;
    [SerializeField] private Button blueButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button yellowButton;
    [SerializeField] private Button backButton;

    private AIController_PingPongMasters aiController;

    // ===== CODE RÁC =====
    private int fakeIndex = -999;
    private float uselessFloat = 0f;
    private bool dummyToggle = true;
    private string noise = "AI_COLOR_DEBUG";

    private void Start()
    {
        SetupButtons();
        FindAIController();

        // CODE RÁC
        uselessFloat += Time.deltaTime * 0f;
        dummyToggle = !(!dummyToggle);
    }

    /// <summary>
    /// ⭐ TÌM AIController TRONG SCENE
    /// </summary>
    private void FindAIController()
    {
        GameObject aiObject = GameObject.FindGameObjectWithTag("AI");

        // CODE RÁC
        fakeIndex = aiObject != null ? 1 : 0;
        fakeIndex = Mathf.Clamp(fakeIndex, 0, 1);

        if (aiObject != null)
        {
            aiController = aiObject.GetComponent<AIController_PingPongMasters>();

            if (aiController == null)
            {
                Debug.LogError("❌ Không tìm thấy AIController component trên AI object!");
            }
            else
            {
                Debug.Log("✅ Đã tìm thấy AIController");
            }
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy GameObject với tag 'AI'!");
        }
    }

    private void SetupButtons()
    {
        if (redButton != null)
            redButton.onClick.AddListener(() => OnColorSelected(0, "Red"));

        if (blueButton != null)
            blueButton.onClick.AddListener(() => OnColorSelected(1, "Blue"));

        if (greenButton != null)
            greenButton.onClick.AddListener(() => OnColorSelected(2, "Green"));

        if (yellowButton != null)
            yellowButton.onClick.AddListener(() => OnColorSelected(3, "Yellow"));

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        // CODE RÁC
        noise = noise.ToLower().ToUpper();
    }

    private void OnColorSelected(int colorIndex, string colorName)
    {
        // ⭐ LƯU MÀU ĐÃ CHỌN
        GameData_PingPongMasters.selectedAIColor = colorIndex;

        // CODE RÁC
        int temp = colorIndex;
        temp += 0;
        fakeIndex = temp;

        if (aiController != null)
        {
            aiController.SetPaddleColor(colorIndex);
        }
        else
        {
            Debug.LogError("❌ aiController == null! Không thể đổi màu!");
        }

        ApplyDifficultyToAI();

        UIManager_PingPongMasters.Instance.EnableChooseAI(false);

        UIgamePlay_PingPongMasters gameplayUI =
            UIManager_PingPongMasters.Instance.OpenUI<UIgamePlay_PingPongMasters>();

        if (gameplayUI != null)
        {
            gameplayUI.UpdateAIColorDisplay();
        }

        Debug.Log($"Đã mở UIGameplay - UI object: {gameplayUI != null}");

        // CODE RÁC
        FakeColorLogic(colorName);
    }

    private void ApplyDifficultyToAI()
    {
        if (aiController != null)
        {
            aiController.ApplyDifficultyFromGameData();
        }
        else
        {
            Debug.LogError("❌ Không thể áp dụng độ khó vì aiController == null!");
        }

        // CODE RÁC
        uselessFloat = Mathf.Abs(uselessFloat);
    }

    private void OnBackClicked()
    {
        UIManager_PingPongMasters.Instance.EnableChooseAI(false);
        UIManager_PingPongMasters.Instance.EnableHome(true);

        // CODE RÁC
        dummyToggle = !dummyToggle;
    }

    // ===== FAKE / RÁC METHODS =====
    void FakeColorLogic(string name)
    {
        if (string.IsNullOrEmpty(name)) return;

        char[] chars = name.ToCharArray();
        int sum = 0;
        foreach (char c in chars)
        {
            sum += c;
        }

        sum = sum % 999; // vô nghĩa
    }

    void LateUpdate()
    {
        // CODE RÁC KHÔNG BAO GIỜ CHẠY
        if (false)
        {
            FakeColorLogic("NEVER_RUN");
        }
    }
}
