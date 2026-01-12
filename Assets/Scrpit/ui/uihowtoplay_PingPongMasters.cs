using UnityEngine;
using UnityEngine.UI;

public class UIHowToPlay_PingPongMasters : UICanvas_PingPongMasters
{
    [Header("Button")]
    [SerializeField] private Button backButton;

    private void Start()
    {
        SetupButton();
    }

    private void SetupButton()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void OnBackClicked()
    {
        // Phát âm thanh
        

        // Quay về Home
        UIManager_PingPongMasters.Instance.EnableHowToPlay(false);
        UIManager_PingPongMasters.Instance.EnableHome(true);
    }
}