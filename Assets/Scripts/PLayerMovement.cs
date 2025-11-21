using System.Collections; // Cần cho Coroutine
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float PlayerYOffset=0.5f;
    [Tooltip("Tốc độ di chuyển của player")]
    public float moveSpeed = 5f;
   public float count = 0;
    Coroutine currentMovementCoroutine;

    [Tooltip("Độ cao Y mà player nên đứng (tránh lún xuống sàn)")]
    public float yOffset = 0.5f; // Nếu dùng Plane, có thể là 0.5f (cho Capsule) hoặc 0 (cho Sphere)

    // Cờ (flag) để ngăn player nhận lệnh di chuyển mới khi đang di chuyển
    private bool isMoving = false;
    [Header("Game Logic")]
    [Tooltip("Kéo 'ExitPoint' Cube từ Hierarchy vào đây")]
    public Transform exitPoint;

    // Cờ (flag) để ngăn chặn các hành động khác khi đã thắng
    private bool hasReachedGoal = false;

    [Header("Animation")]
    public Animator playerAnimator;
    private int isMovingHash;

    public GridManager gridManager;
    public MultiStageGridManager multiStageGridManager;
    private List<TileInfo> path = new List<TileInfo>();
    private int currentPathIndex = 0;
    /// <summary>
    /// Hàm này được PathDrawer gọi để bắt đầu di chuyển
    /// </summary>
    /// 
    public void StartMovement(List<TileInfo> newPath)
    {
        if (isMoving) return;
        path = newPath;
        currentPathIndex = 0;

        if (path.Count > 0)
        {
            // Bắt đầu di chuyển từ ô đầu tiên trong danh sách
            MoveToNextTile();
        }
    }

    private void Start()
    {
        isMovingHash = Animator.StringToHash("isMoving");
    }

   
    public void FollowPath(List<TileInfo> path)
    {
        // Kiểm tra điều kiện dừng: đã di chuyển hoặc đã đạt mục tiêu
        if (isMoving || hasReachedGoal) return;

        // 1. Lưu đường đi mới vào biến nội bộ của class (tôi dùng 'this.path' để đảm bảo)
        this.path = path;
        this.currentPathIndex = 0; // Đảm bảo index bắt đầu từ 0

        // 2. Reset trạng thái
        hasReachedGoal = false;
        isMoving = true;
        playerAnimator.SetBool(isMovingHash, true); // Cập nhật tham số Speed cho Animator

        // 3. Bắt đầu quá trình di chuyển bằng hàm điều phối
        MoveToNextTile();
    }





    private IEnumerator MoveAlongPath(Vector3 targetPosition)
    {
        isMoving = true;
        playerAnimator.SetBool(isMovingHash, true); // Cập nhật tham số Speed cho Animator

        // Đảm bảo trục Y khớp để không bị lún hoặc bay lên
        targetPosition.y = transform.position.y;

        float stoppingDistance = 0.01f;

        // * KHẮC PHỤC LỖI STACK OVERFLOW QUAN TRỌNG *
        // Nếu Player đã ở vị trí đích (hoặc rất gần) ngay từ đầu, Coroutine sẽ bị nhảy qua
        // và gọi hàm tiếp theo mà không đợi một frame nào, gây lỗi Stack Overflow.
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            // Yield một lần để cho phép Unity chạy các Coroutine khác và phá vỡ chuỗi đệ quy
            yield return null;

            // Tiếp tục đi đến ô tiếp theo
            MoveToNextTile();
            yield break; // Kết thúc Coroutine này
        }

        // Vòng lặp di chuyển bình thường
        while (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            //Nhân vật luôn hướng về phía di chuyển
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Đảm bảo dừng chính xác
        transform.position = targetPosition;
        if (currentPathIndex > 0)
        {
            // currentPathIndex đã được tăng trước khi gọi MoveAlongPath.
            // Tile vừa đi đến là path[currentPathIndex - 1]
            TileInfo reachedTile = path[currentPathIndex - 1];

            if (reachedTile.gameObject.CompareTag("ENDLINE"))
            {
                // Đã đến tâm FinishLine, bây giờ mới chuyển hướng sang ExitPoint
                isMoving = false;

                GameObject exitPoint = GameObject.FindGameObjectWithTag("ExitPoint");
                if (exitPoint != null)
                {
                    StopAllCoroutines();
                    currentMovementCoroutine = StartCoroutine(MoveToExitPoint(exitPoint.transform.position));
                    yield break; // Hoàn thành Coroutine MoveAlongPath
                }
            }
            if (reachedTile.gameObject.CompareTag("Checkpoint"))
            {
                // Tìm Midpoint trong scene
                GameObject midpoint = GameObject.FindGameObjectWithTag("Midpoint");
                if (midpoint != null)
                {
                    StopAllCoroutines();
                    // Di chuyển đến Checkpoint trước, sau đó nối tiếp đi Midpoint
                    // Nhưng để đơn giản: Hãy đi thẳng đến Midpoint từ vị trí hiện tại (ô trước Checkpoint)
                    // HOẶC: Đi đến Checkpoint (bằng MoveAlongPath) rồi mới đi Midpoint.

                    // Cách mượt nhất: Chuyển target sang Midpoint luôn
                    StartCoroutine(MoveToMidpointAndStop(midpoint.transform.position));
                    yield break;
                }
            }

            // 2. GẶP FINISH LINE -> TỰ ĐI ĐẾN EXIT POINT
            if (reachedTile.gameObject.CompareTag("FinishLine"))
            {
                GameObject exitPoint = GameObject.FindGameObjectWithTag("ExitPoint");
                if (exitPoint != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(MoveToExitPoint(exitPoint.transform.position));
                    path.Clear();
                    yield break;
                }
            }
        }

        // Nếu không phải FinishLine, tiếp tục đến Tile tiếp theo trong Path
        
        // Tiếp tục di chuyển đến ô kế tiếp
        MoveToNextTile();
    }


    private void MoveToNextTile()
    {
        // === KIỂM TRA ĐIỀU KIỆN DỪNG HOẶC CHUYỂN TIẾP ===
        if (currentPathIndex >= path.Count) // Đã đi hết path
        {
            isMoving = false;
            playerAnimator.SetBool(isMovingHash, false); // Cập nhật tham số Speed cho Animator
            return;
        }

        TileInfo nextTile = path[currentPathIndex];
        //if (nextTile.gameObject.CompareTag("Checkpoint"))
        //{
        //    // Tìm Midpoint trong scene
        //    GameObject midpoint = GameObject.FindGameObjectWithTag("Midpoint");
        //    if (midpoint != null)
        //    {
        //        StopAllCoroutines();
        //        // Di chuyển đến Checkpoint trước, sau đó nối tiếp đi Midpoint
        //        // Nhưng để đơn giản: Hãy đi thẳng đến Midpoint từ vị trí hiện tại (ô trước Checkpoint)
        //        // HOẶC: Đi đến Checkpoint (bằng MoveAlongPath) rồi mới đi Midpoint.

        //        // Cách mượt nhất: Chuyển target sang Midpoint luôn
        //        StartCoroutine(MoveToMidpointAndStop(midpoint.transform.position));
        //        return;
        //    }
        //}

        //// 2. GẶP FINISH LINE -> TỰ ĐI ĐẾN EXIT POINT
        //if (nextTile.gameObject.CompareTag("FinishLine"))
        //{
        //    GameObject exitPoint = GameObject.FindGameObjectWithTag("ExitPoint");
        //    if (exitPoint != null)
        //    {
        //        StopAllCoroutines();
        //        StartCoroutine(MoveToExitPoint(exitPoint.transform.position));
        //        path.Clear();
        //        return;
        //    }
        //}
        // KIỂM TRA TỰ ĐỘNG CHUYỂN TIẾP (Chạm FinishLine)
        //if (nextTile != null && nextTile.gameObject.CompareTag("ENDLINE"))
        //{


        //        GameObject exitPoint = GameObject.FindGameObjectWithTag("ExitPoint");

        //        if (exitPoint != null)
        //        {
        //            StopAllCoroutines();
        //            currentMovementCoroutine = StartCoroutine(MoveToExitPoint(exitPoint.transform.position));

        //            path.Clear();
        //            count = 0;
        //            return;
        //        }

        //}

        // Di chuyển bình thường
        currentMovementCoroutine = StartCoroutine(MoveAlongPath(nextTile.transform.position));
        currentPathIndex++; // Chỉ tăng index sau khi gọi thành công MoveAlongPath
    }
    private IEnumerator MoveToMidpointAndStop(Vector3 targetPosition)
    {
        isMoving = true;
        targetPosition.y = transform.position.y; // Giữ độ cao

        // Di chuyển
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;

        // DỪNG LẠI
        isMoving = false;
        playerAnimator.SetBool(isMovingHash, false);
        path.Clear(); // Xóa đường đi cũ (Giai đoạn 1)

        Debug.Log("Đã đến Midpoint. Người chơi có thể vẽ Giai đoạn 2.");
    }
    private IEnumerator MoveToExitPoint(Vector3 targetPosition)
    {
       float targetGoundY = targetPosition.y;
        Vector3 finalTargetPosition = targetPosition;
        finalTargetPosition.y = targetGoundY + 0.5f;
        while (Vector3.Distance(transform.position, finalTargetPosition) > 0.01f)
        {
            //Nhân vật luôn hướng về phía di chuyển
            Vector3 direction = (finalTargetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, finalTargetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        transform.position = finalTargetPosition;
        isMoving = false;
        playerAnimator.SetBool(isMovingHash, false); // Cập nhật tham số Speed cho Animator

        // KÍCH HOẠT CHUYỂN CẤP ĐỘ
        if (gridManager != null)
        {
            // Reset vị trí Player về (0, Y_offset, 0) của map mới
           
            gridManager.StartNextLevel();
        }
        if (multiStageGridManager != null)
        {
            multiStageGridManager.StartNextLevel();
        }
    }


    public void StopMovement()
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
            currentMovementCoroutine = null;
        }
        isMoving = false;
        playerAnimator.SetBool(isMovingHash, false); // Cập nhật tham số Speed cho Animator
    }
    
}
