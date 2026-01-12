using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Bắt buộc phải có Rigidbody
public class PaddleController_PingPongMasters : MonoBehaviour
{
    // --- Cài đặt Điều chỉnh trong Inspector ---

    [Header("1. Movement Settings")]
    [Tooltip("Độ nhạy cho chuyển động khi kéo chuột.")]
    public float dragSensitivity = 0.01f;

    [Tooltip("Giới hạn vị trí X tối đa (cực phải).")]
    public float maxX = 2.0f;

    [Tooltip("Giới hạn vị trí X tối thiểu (cực trái).")]
    public float minX = -2.0f;

    [Tooltip("Giới hạn vị trí Z tối đa (tiến gần bàn).")]
    public float maxZ = 1.0f;

    [Tooltip("Giới hạn vị trí Z tối thiểu (lùi xa bàn, cuối bàn).")]
    public float minZ = -1.5f;

    [Header("2. Rotation Settings")]
    [Tooltip("Tốc độ vợt xoay để bắt kịp góc xoay mục tiêu.")]
    public float rotationSpeed = 20f; // Tăng tốc độ xoay lên để phản hồi nhanh hơn

    [Tooltip("Góc xoay ban đầu của vợt.")]
    public float initialRotationY = 0f;

    [Header("3. Drag Settings")]
    public LayerMask paddleLayer;

    // --- Biến nội bộ ---
    private bool isDragging = false;
    private Vector3 dragOffset;
    private Camera mainCamera;
    private Plane dragPlane;
    private Rigidbody rb; // Thêm biến Rigidbody

    // --- Logic Khởi tạo & Cập nhật ---

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        mainCamera = Camera.main;
        dragPlane = new Plane(Vector3.up, transform.position);

        // Lấy Rigidbody và cài đặt
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // QUAN TRỌNG: Phải bật cái này để code điều khiển được vợt
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Giúp di chuyển mượt hơn
    }

    void Update()
    {
        // Xử lý Input (Click chuột)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    float enter;
                    dragPlane.SetNormalAndPosition(Vector3.up, transform.position);
                    if (dragPlane.Raycast(ray, out enter))
                    {
                        Vector3 hitPoint = ray.GetPoint(enter);
                        dragOffset = transform.position - hitPoint;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    // Dùng FixedUpdate để xử lý vật lý tốt hơn
    void FixedUpdate()
    {
        if (isDragging && Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float enter;

            if (dragPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 targetPosition = hitPoint + dragOffset;

                // Giới hạn vị trí
                float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
                float newZ = Mathf.Clamp(targetPosition.z, minZ, maxZ);

                Vector3 newPos = new Vector3(newX, transform.position.y, newZ);

                // --- SỬA ĐỔI QUAN TRỌNG NHẤT Ở ĐÂY ---
                // Thay vì: transform.position = newPos;
                // Dùng: MovePosition để vật lý không bị xuyên thấu
                rb.MovePosition(newPos);
            }
        }

        // Cập nhật góc xoay (cũng nên đưa vào FixedUpdate hoặc Update đều được)
        UpdateRotation();
    }

    void UpdateRotation()
    {
        float currentX = transform.localPosition.x;
        float t = Mathf.InverseLerp(minX, maxX, currentX);
        float targetRotationZ = Mathf.Lerp(90f, -90f, t);

        Quaternion targetRotation = Quaternion.Euler(0f, initialRotationY, targetRotationZ);

        // Dùng MoveRotation cho Rigidbody
        rb.MoveRotation(Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * rotationSpeed));
    }

    // --- THÊM MỚI: Hàm reset vị trí vợt về minZ ---
    public void ResetToServePosition()
    {
        // Đặt X về 0 (giữa bàn), Y giữ nguyên, Z về minZ (cuối bàn)
        Vector3 startPos = new Vector3(0f, transform.position.y, minZ);

        // Cập nhật ngay lập tức
        transform.position = startPos;
        
        // Cập nhật cả Rigidbody để đồng bộ vật lý
        if (rb != null)
        {
            rb.MovePosition(startPos);
            // Reset vận tốc (nếu có dùng lực, dù kinematic không cần nhưng cứ an toàn)
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;
        }

        // Ngắt trạng thái đang kéo chuột để tránh vợt bị dính theo chuột
        isDragging = false;

      
    }
}