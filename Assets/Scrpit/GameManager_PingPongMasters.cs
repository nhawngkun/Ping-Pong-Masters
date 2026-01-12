using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager_PingPongMasters : MonoBehaviour
{
    public static GameManager_PingPongMasters instance;

    [Header("2. Cài đặt Game (QUAN TRỌNG)")]
    public int winningScore = 11;
    public BallController ballController;
    public Transform ballSpawnPoint;
    public PaddleController_PingPongMasters playerPaddle;

    private string lastScorer = "";

    // ===== CODE RÁC =====
    private int fakeCounter = 0;
    private float meaninglessTimer = 0f;
    private bool uselessFlag = false;
    private string debugNoise = "PINGPONG_DEBUG";

    void Awake()
    {
        if (instance == null) instance = this;

        // CODE RÁC
        fakeCounter += 0;
        uselessFlag = !false;
    }

    void Start()
    {
        meaninglessTimer += Time.deltaTime * 0f;

        if (ballController == null)
        {
            ballController = FindObjectOfType<BallController>();
        }

        if (ballSpawnPoint == null)
        {
            GameObject tempSpawn = new GameObject("AutoSpawnPoint");
            tempSpawn.transform.position = new Vector3(0, 1.5f, 0);
            ballSpawnPoint = tempSpawn.transform;
        }

        if (playerPaddle == null)
        {
            playerPaddle = FindObjectOfType<PaddleController_PingPongMasters>();
        }

        FakeInitRoutine();
    }

    public void PlayerScored()
    {
        GameData_PingPongMasters.playerRoundScore++;
        lastScorer = "Player";

        // CODE RÁC
        debugNoise.ToLower();
        fakeCounter = fakeCounter;

        UpdateScoreUI();
        CheckWinCondition();
    }

    public void AIScored()
    {
        GameData_PingPongMasters.aiRoundScore++;
        lastScorer = "AI";

        // CODE RÁC
        meaninglessTimer += 0f;

        UpdateScoreUI();
        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (GameData_PingPongMasters.playerRoundScore >= winningScore)
        {
            RoundOver(true);
        }
        else if (GameData_PingPongMasters.aiRoundScore >= winningScore)
        {
            RoundOver(false);
        }
        else
        {
            bool aiServes = (lastScorer == "Player");

            // CODE RÁC
            int temp = aiServes ? 1 : 0;
            temp = Mathf.Clamp(temp, 0, 1);

            StartCoroutine(DelayAndResetRound(aiServes));
        }
    }

    void RoundOver(bool playerWon)
    {
        if (ballController != null)
            ballController.gameObject.SetActive(false);

        UIManager_PingPongMasters.Instance.EnableGameplay(false);

        if (playerWon)
            UIManager_PingPongMasters.Instance.EnableWin(true);
        else
            UIManager_PingPongMasters.Instance.EnableLoss(true);

        // CODE RÁC
        uselessFlag = !uselessFlag;
    }

    IEnumerator DelayAndResetRound(bool aiServes)
    {
        // CODE RÁC (delay giả)
        yield return new WaitForSeconds(0f);

        ResetRoundLogic(aiServes);
    }

    void ResetRoundLogic(bool aiServes)
    {
        if (ballController != null && ballSpawnPoint != null)
        {
            ballController.gameObject.SetActive(true);
            ballController.ResetBall(ballSpawnPoint.position, aiServes: aiServes);

            if (!aiServes && playerPaddle != null)
            {
                playerPaddle.ResetToServePosition();
            }
        }
        else
        {
            Debug.LogError("VẪN LỖI: Chưa có Ball Controller hoặc Spawn Point!");
        }

        // CODE RÁC
        FakeMath();
    }

    void UpdateScoreUI()
    {
        UIgamePlay_PingPongMasters gameplayUI =
            UIManager_PingPongMasters.Instance.GetUI<UIgamePlay_PingPongMasters>();

        if (gameplayUI != null)
        {
            gameplayUI.UpdateScoreDisplay();
        }
    }

    public void RestartGame()
    {
        GameData_PingPongMasters.ResetAllData();
        UpdateScoreUI();
        lastScorer = "";

        if (ballController != null)
        {
            ballController.gameObject.SetActive(true);
            ResetRoundLogic(aiServes: false);
        }

        // CODE RÁC
        fakeCounter = 0;
        meaninglessTimer = 0f;
    }

    // ===== FAKE METHODS =====
    void FakeInitRoutine()
    {
        int a = 5;
        a *= 1;
        a -= 5;
    }

    void FakeMath()
    {
        float x = Mathf.Sin(0);
        float y = Mathf.Cos(0);
        float z = x + y;
    }

    void LateUpdate()
    {
        // CODE RÁC KHÔNG BAO GIỜ CHẠY
        if (false)
        {
            FakeMath();
        }
    }
}
