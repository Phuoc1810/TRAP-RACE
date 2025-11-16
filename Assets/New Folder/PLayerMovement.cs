using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Cần cho Coroutine

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Tốc độ di chuyển của player")]
    public float moveSpeed = 5f;

    [Tooltip("Độ cao Y mà player nên đứng (tránh lún xuống sàn)")]
    public float yOffset = 0.5f; // Nếu dùng Plane, có thể là 0.5f (cho Capsule) hoặc 0 (cho Sphere)

    // Cờ (flag) để ngăn player nhận lệnh di chuyển mới khi đang di chuyển
    private bool isMoving = false;
    [Header("Game Logic")]
    [Tooltip("Kéo 'ExitPoint' Cube từ Hierarchy vào đây")]
    public Transform exitPoint;

    // Cờ (flag) để ngăn chặn các hành động khác khi đã thắng
    private bool hasReachedGoal = false;
    /// <summary>
    /// Hàm này được PathDrawer gọi để bắt đầu di chuyển
    /// </summary>
    public void FollowPath(List<TileInfo> path)
    {
        
        // Nếu đang di chuyển, HOẶC đã thắng rồi, không nhận lệnh mới
        if (isMoving || hasReachedGoal)
        {
            return;
        }

        // Reset cờ 'thắng' mỗi khi bắt đầu đường đi mới
        hasReachedGoal = false;
        // Bắt đầu một Coroutine để xử lý việc di chuyển theo tuần tự
        StartCoroutine(MoveAlongPath(path));
    }

    /// <summary>
    /// Coroutine xử lý việc di chuyển mượt mà qua từng ô
    /// </summary>
    private IEnumerator MoveAlongPath(List<TileInfo> path)
    {
        isMoving = true;

        foreach (TileInfo tile in path)
        {
            if (tile == null)
            {
                Debug.LogWarning("Ô tiếp theo đã bị hủy. Dừng di chuyển!");
                break;
            }

            // ... (code di chuyển MoveTowards vẫn như cũ) ...

            Vector3 targetPosition = new Vector3(tile.transform.position.x, yOffset, tile.transform.position.z);
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPosition;


            // (Sau khi đã đến ô 'tile')

            // 1. Kiểm tra xem ô này có phải Vạch Đích không?
           
            if (tile.CompareTag("FinishLine"))
            {
                Debug.Log("HOÀN THÀNH! Đã đến vạch đích!");
                hasReachedGoal = true;

                // 2. Bắt đầu di chuyển ra 'Điểm Thoát'
                if (exitPoint != null)
                {
                    StartCoroutine(MoveToExitPoint());
                }

                // 3. THAO TÁC CỰC KỲ QUAN TRỌNG:
                // Lệnh 'break' phải được gọi để NGỪNG VÒNG LẶP FOREACH
                // ngay lập tức, nếu không, Coroutine sẽ cố gắng đi đến ô tiếp theo
                // trong đường dẫn đã vẽ (path) và ngăn MoveToExitPoint() hoàn thành.
                break;
            }

            // (logic bẫy, nên nằm ở đây)


        }

     
        // Chỉ đặt 'isMoving = false' nếu KHÔNG đi đến vạch đích
        // (Nếu đến vạch đích, coroutine 'MoveToExitPoint' sẽ xử lý việc này)
        if (!hasReachedGoal)
        {
            isMoving = false;
            Debug.Log("Player đã đi hết đường!");
        }
        
    }
    // === DÁN COROUTINE MỚI NÀY VÀO CUỐI SCRIPT ===

    /// <summary>
    /// Coroutine tự động di chuyển player ra khỏi màn
    /// </summary>
    private IEnumerator MoveToExitPoint()
    {
        Debug.Log("Bắt đầu di chuyển ra điểm thoát...");

        // Đảm bảo Y offset chính xác cho điểm thoát
        Vector3 exitTarget = new Vector3(exitPoint.position.x, yOffset, exitPoint.position.z);

        while (Vector3.Distance(transform.position, exitTarget) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                exitTarget,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Đảm bảo đến đúng vị trí
        transform.position = exitTarget;

        Debug.Log("Đã thoát!");

        // === TẮT CỜ isMoving SAU KHI ĐÃ ĐẾN EXIT POINT ===
        isMoving = false;
    }
}
