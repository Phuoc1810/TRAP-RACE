using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer2 : MonoBehaviour
{
    public Color highlightColor = Color.green; // Màu khi vuốt chọn

    [Header("Object References")]
    public PlayerMovement player; // Tham chiếu đến script PlayerMovement
    public MultiStageGridManager multiStageGridManager;
    private Camera mainCamera;

    // === THAY ĐỔI ===
    // 'currentPath' là đường đang VẼ (trong khi giữ chuột)
    private List<TileInfo> currentPath = new List<TileInfo>();

    // 'confirmedPath' là đường ĐÃ VẼ XONG (sau khi thả chuột)
    private List<TileInfo> confirmedPath = new List<TileInfo>();
    // === KẾT THÚC THAY ĐỔI ===

    public bool enableDrawing = false;
    private bool isDrawing = false;

    [Header("UI")]
    [SerializeField] private TextScale titleText;

    void Start()
    {
        mainCamera = Camera.main;
        //titleText.SetActiveText(false);
        //DisableDrawing();
    }

    void Update()
    {
        if (!enableDrawing)
            return;

        // 1. Khi bắt đầu nhấn chuột
        if (Input.GetMouseButtonDown(0))
        {
            TileInfo tileInfo = GetTileUnderMouse();
            if (tileInfo != null)
            {
                HandleTileClicked(tileInfo);
            }
        }
        // 2. Khi đang giữ chuột (đang vuốt)
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            ContinueDrawing();
        }
        // 3. Khi thả chuột
        else if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            StopDrawing();

        }

        // === THAY ĐỔI ===
        // 4. Khi nhấn phím 'E'
        if (Input.GetKeyDown(KeyCode.E))
        {
            ValidateAndMovePlayer(currentPath);
        }
        // === KẾT THÚC THAY ĐỔI ===
    }

    void StartDrawing(TileInfo tile)
    {
        // === THAY ĐỔI: YÊU CẦU RESET ===
        // Nếu người chơi bắt đầu vẽ một đường mới
        // (trong khi đã có một đường 'confirmedPath' đang chờ)
        // thì chúng ta XÓA đường 'confirmedPath' cũ đi.
        if (confirmedPath.Count > 0)
        {
            ResetConfirmedPath();
        }
        // === KẾT THÚC THAY ĐỔI ===

        //TileInfo tile = GetTileUnderMouse();
        if (tile != null)
        {
            isDrawing = true;
            currentPath.Clear(); // Xóa đường đang vẽ (nếu có)
            AddTileToPath(tile);
        }
    }

    void ContinueDrawing()
    {
        TileInfo tile = GetTileUnderMouse();

        if (tile != null && !currentPath.Contains(tile))
        {
            TileInfo lastTile = currentPath[currentPath.Count - 1];

            if (IsAdjacent(lastTile, tile))
            {
                AddTileToPath(tile);
            }
        }
    }

    // === THAY ĐỔI: Hàm này giờ chỉ 'xác nhận' đường đi ===
    void StopDrawing()
    {
        isDrawing = false;

        // Chuyển đường đi tạm thời (currentPath) thành đường đi đã xác nhận (confirmedPath)
        confirmedPath = new List<TileInfo>(currentPath);

        // Xóa đường đi tạm thời (để chuẩn bị cho lần vẽ tiếp)
        //currentPath.Clear();

        if (confirmedPath.Count > 0)
        {
            Debug.Log($"Đã xác nhận đường đi! Gồm {confirmedPath.Count} ô. Nhấn E để di chuyển.");
        }
    }




    void TriggerPlayerMovement()
    {
        // Chỉ chạy nếu có đường đi đã xác nhận VÀ player đang không di chuyển
        if (confirmedPath.Count > 0 && player != null)
        {
            Debug.Log("Đã nhấn E. Player bắt đầu chạy!");

            // 1. Ra lệnh cho player chạy
            // (PlayerMovement.cs sẽ tự xử lý cờ 'isMoving' của nó)
            player.FollowPath(new List<TileInfo>(confirmedPath));

            // 2. Sau khi ra lệnh, chúng ta reset đường đi ngay
            // (Chúng ta KHÔNG reset màu ở đây, mà để PlayerMovement tự làm...
            // Hoặc đơn giản là reset màu ở đây và xóa path)
            ResetConfirmedPath();
        }
        else if (player == null)
        {
            Debug.LogWarning("Chưa gán Player vào PathDrawer!");
        }
        else
        {
            Debug.Log("Không có đường đi nào (confirmedPath) để chạy!");
        }
    }


    void ResetConfirmedPath()
    {
        // Đặt lại màu cho tất cả các ô trong đường đi đã xác nhận
        foreach (var tile in confirmedPath)
        {
            if (tile != null) // Kiểm tra phòng khi ô bị hủy
            {
                tile.GetComponent<Renderer>().material.color = tile.originalColor;
            }
        }
        // Xóa đường đi
        confirmedPath.Clear();
        Debug.Log("Đã reset đường đi đã xác nhận.");
    }



    // (Các hàm GetTileUnderMouse, AddTileToPath, IsAdjacent giữ nguyên như cũ)

    TileInfo GetTileUnderMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        int layerToIgnore = 1 << LayerMask.NameToLayer("Trap");
        int layerMask = ~layerToIgnore; // lấy tất cả layer trừ layer này

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            if (hit.collider.CompareTag("LandTile") || hit.collider.CompareTag("FinishLine") || hit.collider.CompareTag("ENDLINE") || hit.collider.CompareTag("Checkpoint"))
            {
                return hit.collider.GetComponent<TileInfo>();
            }
        }
        return null;
    }

    void AddTileToPath(TileInfo tile)
    {
        // Thêm vào đường đi 'đang vẽ'
        currentPath.Add(tile);
        tile.GetComponent<Renderer>().material.color = highlightColor;
    }

    bool IsAdjacent(TileInfo lastTile, TileInfo newTile)
    {
        int deltaX = Mathf.Abs(lastTile.x - newTile.x);
        int deltaZ = Mathf.Abs(lastTile.z - newTile.z);
        return (deltaX + deltaZ == 1);
    }
    void HandleTileClicked(TileInfo clickedTile)
    {
        // Lấy vị trí Player hiện tại
        Vector2Int tileCoords = clickedTile.GetCoords();

        // Tính khoảng cách từ ô được click đến Player
        float distanceToPlayer = Vector3.Distance(clickedTile.transform.position, player.transform.position);

        // ĐIỀU KIỆN BẮT ĐẦU VẼ:
        // 1. Hoặc là ô thuộc hàng Z=0 (cho lần đầu tiên)
        // 2. Hoặc là ô Player đang đứng (cho giai đoạn 2 từ Midpoint)
        // 3. Hoặc là ô ngay cạnh Player (đơn giản nhất là check khoảng cách)

        // Logic linh hoạt nhất: Chỉ cần click vào ô GẦN Player (sai số < 1.5f)
        if (tileCoords.y != (multiStageGridManager.height + 1))
        {
            if (tileCoords.y != 0) // Hoặc tileCoords.z, tùy vào cách bạn thiết lập
            {
                Debug.Log("Không thể bắt đầu vẽ đường đi từ ô này. Phải bắt đầu từ hàng đầu tiên (Z=0).");
                return; // Dừng lại, không cho phép vẽ
            }
        }
        if (distanceToPlayer > 2f)
        {
            return;
        }
        else
        {
            Debug.Log("Hãy bắt đầu vẽ từ vị trí nhân vật đang đứng!");
        }
        StartDrawing(clickedTile);
    }
    public void ValidateAndMovePlayer(List<TileInfo> drawnPath)
    {
        if (drawnPath == null || drawnPath.Count == 0)
        {
            ResetConfirmedPath();
            return;
        }

        TileInfo lastTile = drawnPath[drawnPath.Count - 1];
        string tag = lastTile.gameObject.tag;

        // CHẤP NHẬN CẢ 2 LOẠI ĐÍCH
        if (tag == "FinishLine" || tag == "Checkpoint")
        {
            Debug.Log($"Hợp lệ! Đang di chuyển đến {tag}");

            //PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null)
            {
                player.FollowPath(drawnPath);
            }
        }
        else
        {
            Debug.LogWarning("Đường đi phải kết thúc tại Checkpoint hoặc FinishLine!");
            ResetConfirmedPath();
        }
    }
   
    public void EnableDrawing()
    {
        titleText.SetActiveText(true);
        titleText.SetScaleText(new Vector3(0.5f, 0.5f, 0.5f));
        titleText.StartCoroutine(titleText.ScaleText(Vector3.one));
        enableDrawing = true;
    }

    public IEnumerator DisableDrawing()
    {
        enableDrawing = false;
        yield return titleText.StartCoroutine(titleText.ScaleText(new Vector3(0.5f, 0.5f, 0.5f)));
        titleText.SetActiveText(false);
    }
}
