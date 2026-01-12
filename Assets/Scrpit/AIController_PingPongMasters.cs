using UnityEngine;

public enum AIDifficulty
{
    Easy,      // AI cơ bản - dễ bị loại
    Medium,    // AI trung bình - ít bị loại, đánh chuẩn
    Hard       // AI khó - gần như không bị loại, tấn công thông minh
}

public class AIController_PingPongMasters : MonoBehaviour
{
    [Header("⭐ CHỌN MỨC ĐỘ AI")]
    public AIDifficulty difficulty = AIDifficulty.Easy;

    [Header("⭐ MÀU VỢT AI (Kéo 4 Models vào đây)")]
    [Tooltip("Model vợt màu Đỏ (Index 0)")]
    public GameObject redPaddleModel;
    [Tooltip("Model vợt màu Xanh dương (Index 1)")]
    public GameObject bluePaddleModel;
    [Tooltip("Model vợt màu Xanh lá (Index 2)")]
    public GameObject greenPaddleModel;
    [Tooltip("Model vợt màu Vàng (Index 3)")]
    public GameObject yellowPaddleModel;

    [Header("Cấu hình AI")]
    public Rigidbody ball;
    public float speed = 6.0f;
    public float reactionDelay = 0.1f;
    public float errorMargin = 0.2f;

    [Header("⭐ Cải thiện di chuyển mượt")]
    [Tooltip("Tốc độ làm mượt di chuyển (0-1, càng cao càng mượt nhưng chậm hơn)")]
    [Range(0f, 1f)]
    public float smoothing = 0.15f;
    [Tooltip("Khoảng cách tối thiểu để AI bắt đầu di chuyển (tránh rung giật)")]
    public float movementThreshold = 0.05f;

    [Header("Giới hạn sân")]
    [Tooltip("Tối thiểu X (cực trái)")]
    public float minX = -2.2f;
    [Tooltip("Tối đa X (cực phải)")]
    public float maxX = 2.2f;
    [Tooltip("Vị trí gần bàn nhất (sẵn sàng đánh)")]
    public float minZ = 0.8f;
    [Tooltip("Vị trí xa bàn nhất (lùi về)")]
    public float maxZ = 3.0f;

    [Header("⭐ CẢI THIỆN LỰC ĐÁNH")]
    [Tooltip("Tốc độ di chuyển tối đa khi AI lấy đà đánh mạnh")]
    public float rushSpeed = 15f;
    [Tooltip("Khoảng cách tối thiểu để AI bắt đầu lấy đà")]
    public float rushDistanceThreshold = 1.5f;
    [Tooltip("AI sẽ tiến lên gần bàn khi đánh mạnh")]
    public bool enableAggressivePlay = true;

    [Header("⭐ XOAY VỢT AI")]
    [Tooltip("Bật/Tắt xoay vợt AI")]
    public bool enableAIPaddleRotation = true;
    [Tooltip("Tốc độ xoay vợt AI")]
    public float aiRotationSpeed = 20f;
    [Tooltip("Góc xoay ban đầu")]
    public float initialRotationY = 180f;

    [Header("⭐ CÀI ĐẶT MỨC ĐỘ")]
    [Tooltip("MEDIUM: Sai số khi dự đoán bóng (nhỏ hơn = chính xác hơn)")]
    public float mediumErrorMargin = 0.1f;
    [Tooltip("HARD: Sai số khi dự đoán bóng (rất nhỏ = rất chính xác)")]
    public float hardErrorMargin = 0.02f;
    [Tooltip("MEDIUM: Khoảng cách lấy đà thường")]
    public float mediumRushThreshold = 1.2f;
    [Tooltip("HARD: Khoảng cách lấy đà sớm hơn (tấn công tích cực)")]
    public float hardRushThreshold = 2.0f;
    [Tooltip("Phạm vi để AI nhắm vào (HARD: nhắm chính xác góc xa)")]
    public float hardAimPrecision = 0.95f;

    private Rigidbody rb;
    private float targetX;
    private float targetZ;
    private float timer;
    private bool isRushing = false;
    private Vector3 lastPosition;

    private float currentVelocityX;
    private float currentVelocityZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        targetX = transform.position.x;
        targetZ = transform.position.z;
        lastPosition = transform.position;

        // ⭐ ÁP DỤNG ĐỘ KHÓ TỪ GAMEDATA
        ApplyDifficultyFromGameData();

        // ⭐ BẬT MÀU VỢT ĐÃ CHỌN
        SetPaddleColor(GameData_PingPongMasters.selectedAIColor);
    }

    public void SetPaddleColor(int colorIndex)
    {
        // Tắt tất cả models trước
        if (redPaddleModel != null) redPaddleModel.SetActive(false);
        if (bluePaddleModel != null) bluePaddleModel.SetActive(false);
        if (greenPaddleModel != null) greenPaddleModel.SetActive(false);
        if (yellowPaddleModel != null) yellowPaddleModel.SetActive(false);

        // Bật model màu đã chọn
        switch (colorIndex)
        {
            case 0: // Red
                if (redPaddleModel != null)
                {
                    redPaddleModel.SetActive(true);
                   
                }
                break;
            case 1: // Blue
                if (bluePaddleModel != null)
                {
                    bluePaddleModel.SetActive(true);
                  
                }
                break;
            case 2: // Green
                if (greenPaddleModel != null)
                {
                    greenPaddleModel.SetActive(true);
                    
                }
                break;
            case 3: // Yellow
                if (yellowPaddleModel != null)
                {
                    yellowPaddleModel.SetActive(true);
                   
                }
                break;
            default:
               
                // Bật màu đỏ mặc định
                if (redPaddleModel != null) redPaddleModel.SetActive(true);
                break;
        }
    }

    public void ApplyDifficultyFromGameData()
    {
        difficulty = GameData_PingPongMasters.selectedDifficulty;
      

        // Điều chỉnh tham số theo độ khó
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                speed = 6f;
                reactionDelay = 0.15f;
                errorMargin = 0.25f;
                break;

            case AIDifficulty.Medium:
                speed = 8f;
                reactionDelay = 0.08f;
                errorMargin = mediumErrorMargin;
                break;

            case AIDifficulty.Hard:
                speed = 10f;
                reactionDelay = 0.05f;
                errorMargin = hardErrorMargin;
                enableAggressivePlay = true;
                break;
        }
    }

    void FixedUpdate()
    {
        if (ball == null) return;

        lastPosition = transform.position;

        timer += Time.fixedDeltaTime;
        if (timer >= reactionDelay)
        {
            timer = 0;
            PredictBallPosition();
        }

        Vector3 currentPos = transform.position;
        float currentSpeed = isRushing ? rushSpeed : speed;

        float distanceX = Mathf.Abs(currentPos.x - targetX);
        float distanceZ = Mathf.Abs(currentPos.z - targetZ);

        float newX = currentPos.x;
        float newZ = currentPos.z;

        if (distanceX > movementThreshold)
        {
            newX = Mathf.SmoothDamp(currentPos.x, targetX, ref currentVelocityX, smoothing, currentSpeed);
            newX = Mathf.Clamp(newX, minX, maxX);
        }
        else
        {
            newX = targetX;
        }

        if (distanceZ > movementThreshold)
        {
            newZ = Mathf.SmoothDamp(currentPos.z, targetZ, ref currentVelocityZ, smoothing, currentSpeed);
            newZ = Mathf.Clamp(newZ, minZ, maxZ);
        }
        else
        {
            newZ = targetZ;
        }

        rb.MovePosition(new Vector3(newX, currentPos.y, newZ));

        if (enableAIPaddleRotation)
        {
            UpdateAIPaddleRotation();
        }
    }

    void PredictBallPosition()
    {
        if (ball.linearVelocity.z > 0.5f)
        {
            float distanceZ = transform.position.z - ball.position.z;
            float timeToImpact = Mathf.Abs(distanceZ / ball.linearVelocity.z);

            float predictedX = ball.position.x + (ball.linearVelocity.x * timeToImpact);
            float predictedY = ball.position.y + (ball.linearVelocity.y * timeToImpact) - (Physics.gravity.y * timeToImpact * timeToImpact * 0.5f);

            if (predictedY < 0.5f && ball.linearVelocity.z > 0)
            {
                float timeToTable = Mathf.Sqrt(2 * Mathf.Abs(ball.position.y - 0.5f) / Mathf.Abs(Physics.gravity.y));
                predictedX = ball.position.x + (ball.linearVelocity.x * timeToTable);
            }

            float currentErrorMargin = GetErrorMarginByDifficulty();
            float randomError = Random.Range(-currentErrorMargin, currentErrorMargin);
            targetX = predictedX + randomError;
            targetX = Mathf.Clamp(targetX, minX, maxX);

            float safeZ = Mathf.Clamp(transform.position.z - (ball.linearVelocity.z * timeToImpact * 0.1f), minZ + 0.2f, maxZ - 0.2f);

            float distanceToTarget = Vector3.Distance(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(predictedX, 0, ball.position.z)
            );

            float currentRushThreshold = GetRushThresholdByDifficulty();

            if (distanceToTarget > currentRushThreshold)
            {
                isRushing = true;
                if (enableAggressivePlay)
                {
                    targetZ = minZ + 0.3f;
                }
            }
            else
            {
                isRushing = false;

                if (difficulty == AIDifficulty.Hard)
                {
                    targetZ = minZ + 0.1f;
                }
                else if (difficulty == AIDifficulty.Medium)
                {
                    targetZ = safeZ;
                }
                else
                {
                    targetZ = (minZ + maxZ) / 2;
                }
            }
        }
        else if (ball.linearVelocity.z < -0.5f)
        {
            float distanceZ = ball.position.z - transform.position.z;

            if (ball.linearVelocity.z != 0)
            {
                float timeToImpact = Mathf.Abs(distanceZ / ball.linearVelocity.z);
                float predictedX = ball.position.x + (ball.linearVelocity.x * timeToImpact);
                float predictedY = ball.position.y + (ball.linearVelocity.y * timeToImpact) - (Physics.gravity.y * timeToImpact * timeToImpact * 0.5f);

                if (predictedY < 0.5f && Mathf.Abs(ball.linearVelocity.y) > 0.1f)
                {
                    float timeToTable = Mathf.Sqrt(2 * Mathf.Abs(ball.position.y - 0.5f) / Mathf.Abs(Physics.gravity.y));
                    predictedX = ball.position.x + (ball.linearVelocity.x * timeToTable);
                    float predictedZ = ball.position.z + (ball.linearVelocity.z * timeToTable);
                    targetZ = Mathf.Clamp(predictedZ, minZ, maxZ);
                }
                else
                {
                    float predictedZ = ball.position.z + (ball.linearVelocity.z * timeToImpact);
                    targetZ = Mathf.Clamp(predictedZ, minZ, maxZ);
                }

                float currentErrorMargin = GetErrorMarginByDifficulty() * 0.6f;
                float randomError = Random.Range(-currentErrorMargin, currentErrorMargin);
                predictedX = Mathf.Clamp(predictedX, minX, maxX);
                targetX = predictedX + randomError;
                targetX = Mathf.Clamp(targetX, minX, maxX);

                if (difficulty == AIDifficulty.Hard)
                {
                    targetZ = Mathf.Clamp(targetZ - 0.2f, minZ, maxZ);
                }
                else if (difficulty == AIDifficulty.Medium)
                {
                    targetZ = Mathf.Clamp(targetZ, minZ, maxZ);
                }
                else
                {
                    targetZ = Mathf.Clamp(targetZ + 0.1f, minZ, maxZ);
                }
            }

            isRushing = false;
        }
        else
        {
            isRushing = false;
            targetX = Mathf.Lerp(targetX, 0, 0.1f);
            targetZ = maxZ - 0.5f;
        }
    }

    float GetErrorMarginByDifficulty()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                return errorMargin;
            case AIDifficulty.Medium:
                return mediumErrorMargin;
            case AIDifficulty.Hard:
                return hardErrorMargin;
            default:
                return errorMargin;
        }
    }

    float GetRushThresholdByDifficulty()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                return rushDistanceThreshold;
            case AIDifficulty.Medium:
                return mediumRushThreshold;
            case AIDifficulty.Hard:
                return hardRushThreshold;
            default:
                return rushDistanceThreshold;
        }
    }

    void UpdateAIPaddleRotation()
    {
        float currentX = transform.localPosition.x;
        float t = Mathf.InverseLerp(minX, maxX, currentX);
        float targetRotationZ = Mathf.Lerp(-90f, 90f, t);

        Quaternion targetRotation = Quaternion.Euler(0f, initialRotationY, targetRotationZ);
        rb.MoveRotation(Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * aiRotationSpeed));
    }

    public Vector3 GetCurrentVelocity()
    {
        Vector3 velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        return velocity;
    }

    public float GetCurrentSpeed()
    {
        Vector3 velocity = GetCurrentVelocity();
        return new Vector2(velocity.x, velocity.z).magnitude;
    }

    public bool IsRushing()
    {
        return isRushing;
    }

    public AIDifficulty GetDifficulty()
    {
        return difficulty;
    }
}