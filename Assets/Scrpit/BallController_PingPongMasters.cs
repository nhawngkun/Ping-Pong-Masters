using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("1. Lực và Tốc độ")]
    public float minForce = 7f;
    public float maxForce = 9f;
    public float velocityMultiplier = 0.2f;
    public float upwardFactor = 0.5f;

    [Header("1.1 ⭐ GIỚI HẠN TỐC ĐỘ TUYỆT ĐỐI")]
    [Tooltip("Tốc độ tối đa của paddle (giới hạn chặt để tránh bug)")]
    public float maxPaddleSpeed = 5f;
    [Tooltip("Tốc độ tối đa của bóng (m/s) - BẮT BUỘC")]
    public float maxBallSpeed = 15f;

    [Header("1.2 ⭐ Cải thiện AI")]
    [Tooltip("Lực bổ sung khi AI đang lấy đà (rushing)")]
    public float aiRushBonus = 1.5f;
    [Tooltip("Lực tối thiểu của AI (để đảm bảo đánh qua lưới)")]
    public float aiMinForce = 8f;

    [Header("1.3 ⭐ BONI LỰC THEO MỨC ĐỘ")]
    [Tooltip("MEDIUM: Lực bổ sung khi đánh")]
    public float mediumForceBonus = 1.0f;
    [Tooltip("HARD: Lực bổ sung khi đánh")]
    public float hardForceBonus = 2.0f;
    [Tooltip("HARD: Lực bổ sung khi lấy đà")]
    public float hardRushBonus = 3.0f;

    [Header("2. Điều hướng")]
    public float steeringSensitivity = 1.2f;
    [Range(0, 1)] public float steeringInfluence = 0.7f;
    [Range(0, 1)] public float aiAimPrecision = 0.8f;
    [Tooltip("HARD: AI nhắm chính xác góc xa để player khó chơi")]
    [Range(0, 1)] public float hardAimPrecision = 0.95f;

    [Header("3. Trạng thái Game")]
    public bool isServed = false;

    [Header("⭐ RESET BALL SETTINGS")]
    [Tooltip("Độ trễ trước khi reset bóng (giây)")]
    public float resetDelay = 1.5f;
    [Tooltip("Điểm phát bóng của Player")]
    public Vector3 playerServePoint = new Vector3(0, 1.5f, -1.2f);
    [Tooltip("Điểm phát bóng của AI")]
    public Vector3 aiServePoint = new Vector3(0, 1.5f, 1.2f);

    private string lastHitter = "";
    private bool hasBouncedOnTable = false;
    private bool touchedNet = false;

    private Rigidbody rb;
    private Transform playerTransform;
    private float stopTimer = 0f;
    private AIController_PingPongMasters aiController;

    private bool canPlayerHit = true;
    private bool canAIHit = false;

    private bool isInResetDelay = false;
    private float delayTimer = 0f;

    // ⭐⭐⭐ THÊM BIẾN ĐỂ TRÁNH GỌI CheckScore() 2 LẦN ⭐⭐⭐
    private bool hasScored = false; // Flag để đánh dấu đã tính điểm rồi

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = false;

        rb.mass = 0.01f;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.5f;
        rb.maxLinearVelocity = maxBallSpeed;
        rb.maxAngularVelocity = 7f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        GameObject aiObj = GameObject.FindGameObjectWithTag("AI");
        if (aiObj != null) aiController = aiObj.GetComponent<AIController_PingPongMasters>();

        Debug.Log($"⚙️ Ball setup: MaxBallSpeed={maxBallSpeed}, MaxPaddleSpeed={maxPaddleSpeed}");
    }

    void Update()
    {
        if (isInResetDelay)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                isInResetDelay = false;
               
            }
        }

        // ⭐⭐⭐ CHỈ KIỂM TRA BÓNG DỪNG KHI CHƯA TÍNH ĐIỂM ⭐⭐⭐
        if (isServed && !hasScored)
        {
            if (rb.linearVelocity.magnitude < 0.2f)
            {
                stopTimer += Time.deltaTime;
                if (stopTimer > 0.5f)
                {
                   
                    CheckScore();
                    stopTimer = 0f;
                }
            }
            else
            {
                stopTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > maxBallSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxBallSpeed;
            Debug.LogWarning($"🚨 BÓNG QUÁ NHANH! Đã giảm từ {rb.linearVelocity.magnitude:F2} → {maxBallSpeed}");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 oldVelocity = rb.linearVelocity;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AI"))
        {
            // ⭐⭐⭐ PHÁT ÂM THANH KHI ĐÁNH TRÚNG BÓNG ⭐⭐⭐
            if (SoundManager_PingPongMasters.Instance != null)
            {
                SoundManager_PingPongMasters.Instance.PlayVFXSound(3);
               
            }

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (isInResetDelay)
            {
                
                return;
            }

            // ⭐ RESET FLAG KHI CÓ AI ĐÁNH BÓNG (Bắt đầu lượt mới)
            hasScored = false;

            if (!isServed && collision.gameObject.CompareTag("Player"))
            {
                isServed = true;
                rb.useGravity = true;
                canPlayerHit = false;
                canAIHit = true;
                lastHitter = "Player";
                hasBouncedOnTable = false;
                touchedNet = false;
                HitBall(collision);
                return;
            }

            if (!isServed && collision.gameObject.CompareTag("AI"))
            {
                isServed = true;
                rb.useGravity = true;
                canPlayerHit = true;
                canAIHit = false;
                lastHitter = "AI";
                hasBouncedOnTable = false;
                touchedNet = false;

                // ⭐ GỌI HÀM RIÊNG CHO AI PHÁT BÓNG
                AIServeBall();
                return;
            }

            if (isServed)
            {
                lastHitter = collision.gameObject.tag;
                hasBouncedOnTable = false;
                touchedNet = false;

                if (collision.gameObject.CompareTag("Player"))
                {
                    canPlayerHit = false;
                    canAIHit = true;
                   
                }
                else
                {
                    canPlayerHit = true;
                    canAIHit = false;
                  
                }

                HitBall(collision);
            }
        }
        else if (collision.gameObject.CompareTag("Net"))
        {
            touchedNet = true;
            rb.linearVelocity = rb.linearVelocity * 0.5f;
            
        }
        else if (collision.gameObject.CompareTag("Table"))
        {
            hasBouncedOnTable = true;
            

            if (touchedNet)
            {
                float ballZ = transform.position.z;
                bool landedOnCorrectSide = false;

                if (lastHitter == "Player" && ballZ > 0)
                {
                    landedOnCorrectSide = true;
                }
                else if (lastHitter == "AI" && ballZ < 0)
                {
                    landedOnCorrectSide = true;
                }

                if (landedOnCorrectSide)
                {
                    
                    touchedNet = false;
                }
                else
                {
                 
                    CheckScore();
                    return;
                }
            }
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
          

            if (!hasBouncedOnTable && !touchedNet)
            {
                
                if (lastHitter == "Player")
                {
                    GameManager_PingPongMasters.instance.AIScored();
                }
                else
                {
                    GameManager_PingPongMasters.instance.PlayerScored();
                }
                return;
            }

            CheckScore();
        }
    }

    void CheckScore()
    {
        // ⭐⭐⭐ TRÁNH GỌI 2 LẦN ⭐⭐⭐
        if (hasScored)
        {
           
            return;
        }

        hasScored = true; // Đánh dấu đã tính điểm
        isServed = false; // Ngưng kiểm tra bóng dừng trong Update()

        if (string.IsNullOrEmpty(lastHitter)) return;

        Collider ballCollider = GetComponent<Collider>();
        if (ballCollider != null)
        {
            ballCollider.enabled = false;
        }

        if (touchedNet)
        {
            if (lastHitter == "Player")
            {
              
                GameManager_PingPongMasters.instance.AIScored();
            }
            else if (lastHitter == "AI")
            {
                
                GameManager_PingPongMasters.instance.PlayerScored();
            }
            return;
        }

        float ballZ = transform.position.z;
        bool landedOnPlayerSide = (ballZ < 0);
        bool isBallOnTable = (transform.position.y > 0.5f);

        if (lastHitter == "Player")
        {
            if (landedOnPlayerSide)
            {
               
                GameManager_PingPongMasters.instance.AIScored();
            }
            else
            {
                if (hasBouncedOnTable || isBallOnTable)
                {
                    GameManager_PingPongMasters.instance.PlayerScored();
                }
                else
                {
                    GameManager_PingPongMasters.instance.AIScored();
                }
            }
        }
        else if (lastHitter == "AI")
        {
            if (ballZ > 0)
            {
               
                GameManager_PingPongMasters.instance.PlayerScored();
            }
            else
            {
                if (hasBouncedOnTable || isBallOnTable)
                {
                    GameManager_PingPongMasters.instance.AIScored();
                }
                else
                {
                    GameManager_PingPongMasters.instance.PlayerScored();
                }
            }
        }
    }

    void HitBall(Collision collision)
    {
        Vector3 relativeVelocity = collision.relativeVelocity;

        Vector2 horizontalVelocity = new Vector2(
            Mathf.Abs(relativeVelocity.x),
            Mathf.Abs(relativeVelocity.z)
        );
        float paddleSpeed = horizontalVelocity.magnitude;

        paddleSpeed = Mathf.Clamp(paddleSpeed, 0f, maxPaddleSpeed);

       

        float forceFromSpeed = paddleSpeed * velocityMultiplier;
        float finalForce = minForce + forceFromSpeed;

        finalForce = Mathf.Clamp(finalForce, minForce, maxForce);

       

        if (collision.gameObject.CompareTag("AI") && aiController != null)
        {
            AIDifficulty difficulty = aiController.GetDifficulty();
            float totalBonus = 0f;

            switch (difficulty)
            {
                case AIDifficulty.Easy:
                    if (aiController.IsRushing())
                    {
                        totalBonus = aiRushBonus;
                    }
                    break;

                case AIDifficulty.Medium:
                    totalBonus = mediumForceBonus;
                    if (aiController.IsRushing())
                    {
                        totalBonus += aiRushBonus;
                    }
                    break;

                case AIDifficulty.Hard:
                    totalBonus = hardForceBonus;
                    if (aiController.IsRushing())
                    {
                        totalBonus += hardRushBonus;
                    }
                    break;
            }

            finalForce += totalBonus;

            finalForce = Mathf.Max(finalForce, aiMinForce);

            float aiMaxForce = maxForce * 1.3f;
            finalForce = Mathf.Clamp(finalForce, aiMinForce, aiMaxForce);

          
        }
        else
        {
            finalForce = Mathf.Clamp(finalForce, minForce, maxForce);
          
        }

        Vector3 finalDir = Vector3.forward;

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            Vector3 steerDir = reflectDir;

            Rigidbody paddleRb = collision.gameObject.GetComponent<Rigidbody>();
            if (paddleRb != null)
            {
                steerDir.x = paddleRb.linearVelocity.x * steeringSensitivity;
                steerDir.z = 1f;
            }

            finalDir = Vector3.Lerp(reflectDir.normalized, steerDir.normalized, steeringInfluence);

            // ⭐⭐⭐ PLAYER LUÔN ĐÁNH VỀ PHÍA AI (Z DƯƠNG) ⭐⭐⭐
            finalDir.z = Mathf.Abs(finalDir.z);
            if (finalDir.z < 0.2f) finalDir.z = 0.4f;
        }
        else if (collision.gameObject.CompareTag("AI") && aiController != null)
        {
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            Vector3 aimDir = reflectDir;

            if (aiController.GetDifficulty() == AIDifficulty.Hard && playerTransform != null)
            {
                Vector3 playerPos = playerTransform.position;
                float cornerX = (playerPos.x > 0) ? -1.7f : 1.7f;
                Vector3 cornerTarget = new Vector3(cornerX, playerPos.y, playerPos.z);
                aimDir = (cornerTarget - transform.position).normalized;
                finalDir = Vector3.Lerp(reflectDir, aimDir, hardAimPrecision);
            }
            else if (playerTransform != null)
            {
                aimDir = (playerTransform.position - transform.position).normalized;
                finalDir = Vector3.Lerp(reflectDir, aimDir, aiAimPrecision);
            }
            else
            {
                finalDir = reflectDir;
            }

            
            finalDir.z = -Mathf.Abs(finalDir.z);
            if (finalDir.z > -0.2f) finalDir.z = -0.4f;
        }

        finalDir.y = Mathf.Abs(finalDir.y) * upwardFactor + 0.25f;
        finalDir = finalDir.normalized;

        Vector3 newVelocity = finalDir * finalForce;

        if (newVelocity.magnitude > maxBallSpeed)
        {
            newVelocity = newVelocity.normalized * maxBallSpeed;
           
        }

        rb.linearVelocity = newVelocity;
        transform.position += finalDir * 0.15f;

       
    }

    public void ResetBall(Vector3 startPos, bool aiServes = false)
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (aiServes)
        {
            transform.position = aiServePoint;
            
        }
        else
        {
            transform.position = playerServePoint;
           
        }

        Collider ballCollider = GetComponent<Collider>();
        if (ballCollider != null)
        {
            ballCollider.enabled = true;
        }

        isServed = false;
        lastHitter = "";
        hasBouncedOnTable = false;
        touchedNet = false;
        stopTimer = 0f;
        rb.useGravity = false;

        // ⭐ RESET FLAG TÍNH ĐIỂM
        hasScored = false;

        isInResetDelay = true;
        delayTimer = resetDelay;

        if (aiServes)
        {
            canAIHit = false;
            canPlayerHit = true;
        }
        else
        {
            canPlayerHit = false;
            canAIHit = false;
        }

        Invoke("EnablePhysics", resetDelay + 0.1f);
    }
    private void AIServeBall()
    {
        if (!isServed || lastHitter != "AI")
        {
           
            return;
        }

        // --- SOUND ---
        if (SoundManager_PingPongMasters.Instance != null)
        {
            SoundManager_PingPongMasters.Instance.PlayVFXSound(3);
        }

        // Reset vận tốc về 0 trước khi phát
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // ⭐ 1. TĂNG LỰC PHÁT CƠ BẢN (Fix lỗi bóng yếu)
        // Thay vì dùng minForce (7f), ta dùng lực khởi điểm cao hơn cho cú phát bóng từ trạng thái đứng yên.
        float baseServeForce = 11f; // Tăng lên 11 (hoặc 12 tùy cảm giác)

        float serveForce = baseServeForce;

        if (aiController != null)
        {
            AIDifficulty difficulty = aiController.GetDifficulty();
            switch (difficulty)
            {
                case AIDifficulty.Easy:
                    serveForce = baseServeForce;        // Easy: 11f
                    break;
                case AIDifficulty.Medium:
                    serveForce = baseServeForce + 1.5f; // Medium: 12.5f
                    break;
                case AIDifficulty.Hard:
                    serveForce = baseServeForce + 3.0f; // Hard: 14f (Mạnh, nhanh)
                    break;
            }
        }

        // ⭐ 2. TÍNH TOÁN HƯỚNG ĐÁNH
        Vector3 serveDirection;

        if (aiController != null && aiController.GetDifficulty() == AIDifficulty.Hard && playerTransform != null)
        {
            // HARD: Nhắm góc hiểm
            float targetX = (playerTransform.position.x > 0) ? -1.8f : 1.8f; // Mở rộng góc ra biên hơn chút
            Vector3 targetPos = new Vector3(targetX, playerTransform.position.y, playerTransform.position.z);
            serveDirection = (targetPos - transform.position).normalized;
        }
        else if (playerTransform != null)
        {
            // EASY/MEDIUM: Nhắm vào người chơi
            serveDirection = (playerTransform.position - transform.position).normalized;
        }
        else
        {
            serveDirection = new Vector3(0, 0, -1f).normalized;
        }

        // ⭐ 3. ĐIỀU CHỈNH GÓC ĐỘ ĐỂ QUA LƯỚI

        // Ép hướng Z luôn âm (về phía Player)
        serveDirection.z = -Mathf.Abs(serveDirection.z);

        // Đảm bảo bóng lao tới trước đủ nhiều, không bị "bắn lên trời" rồi rơi tại chỗ
        // Giá trị càng gần -1 thì lao tới càng mạnh.
        if (serveDirection.z > -0.6f)
        {
            serveDirection.z = -0.7f; // Ép lao tới mạnh hơn
        }

        // Tạo độ bổng (Y) để qua lưới
        // Giảm upwardFactor đi một chút vì lực đã mạnh, tránh bóng bay ra ngoài bàn
        serveDirection.y = 0.35f; // Fix cứng độ cao vừa phải để bóng đi sắc hơn

        // Chuẩn hóa lại vector hướng
        serveDirection = serveDirection.normalized;

        // ⭐ 4. ÁP DỤNG LỰC
        Vector3 finalVelocity = serveDirection * serveForce;

        // Giới hạn max speed (nếu cần thiết, nhưng serveForce đã được kiểm soát ở trên)
        if (finalVelocity.magnitude > maxBallSpeed)
        {
            finalVelocity = finalVelocity.normalized * maxBallSpeed;
        }

        rb.linearVelocity = finalVelocity;

        // Đẩy bóng ra khỏi vị trí phát một chút để tránh va chạm vật lý tức thời
        transform.position += serveDirection * 0.25f;

    }


    private void EnablePhysics()
    {
        rb.isKinematic = false;
    }
}