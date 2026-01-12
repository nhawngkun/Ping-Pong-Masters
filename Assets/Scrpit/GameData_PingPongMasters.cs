using UnityEngine;

/// <summary>
/// Class static để lưu trữ dữ liệu game giữa các UI
/// </summary>
public static class GameData_PingPongMasters
{
    // Độ khó AI đã chọn
    public static AIDifficulty selectedDifficulty = AIDifficulty.Easy;

    // Màu AI đã chọn (0: Red, 1: Blue, 2: Green, 3: Yellow)
    public static int selectedAIColor = 0;

    // Điểm trong một hiệp
    public static int playerRoundScore = 0;
    public static int aiRoundScore = 0;

    // Số hiệp đã thắng (Bo3)
    public static int playerTotalWins = 0;
    public static int aiTotalWins = 0;

    // Hiệp hiện tại
    public static int currentRound = 1;

    /// <summary>
    /// Reset điểm của một hiệp
    /// </summary>
    public static void ResetRoundScores()
    {
        playerRoundScore = 0;
        aiRoundScore = 0;
    }

    /// <summary>
    /// Reset toàn bộ data game
    /// </summary>
    public static void ResetAllData()
    {
        playerRoundScore = 0;
        aiRoundScore = 0;
        playerTotalWins = 0;
        aiTotalWins = 0;
        currentRound = 1;
        selectedDifficulty = AIDifficulty.Easy;
        selectedAIColor = 0;
    }

    /// <summary>
    /// Kiểm tra xem có ai đã thắng chung cuộc chưa (2/3 hiệp)
    /// </summary>
    public static bool HasMatchWinner()
    {
        return playerTotalWins >= 2 || aiTotalWins >= 2;
    }

    /// <summary>
    /// Lấy tên người thắng chung cuộc
    /// </summary>
    public static string GetMatchWinner()
    {
        if (playerTotalWins >= 2)
            return "PLAYER";
        else if (aiTotalWins >= 2)
            return "AI";
        else
            return "";
    }
}