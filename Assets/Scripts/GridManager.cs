using UnityEngine;
using System.Collections.Generic; // Cần dùng cho List
using System.Collections;
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject landTilePrefab; // Prefab cho ô đất (kéo từ Project vào)
    public int width = 3;             // Chiều rộng (X)
    public int height = 3;            // Chiều cao (Z)
    public float spacing = 1.0f;      // Khoảng cách giữa các ô
                                      // (Nếu ô của bạn rộng 1m, spacing 1.0f sẽ làm chúng sát nhau)
   [ Header("Exit Tile Settings")]
   [Tooltip("Prefab cho ô đích cuối cùng (Exit Tile)")]
    public GameObject exitTilePrefab;
    public GameObject exitPointPrefab;

    [Tooltip("Khoảng cách offset từ ô grid cuối cùng đến Exit Tile")]
    public Vector3 exitOffset = new Vector3(2f, 0f, 0f); // Ví dụ: Offset 2 đơn vị theo trục X

    [HideInInspector]
    public GameObject spawnedExitTile; // Biến lưu trữ đối tượng Exit Tile đã được spawn
    [Header("Spawn Points")]
    // Đây là thứ bạn cần để "bắt vị trí"
    // Cách 1: Lưu trực tiếp các GameObject đã spawn (khuyên dùng)
    public GameObject[,] gridArray;
    private List<Vector3> spawnPositions;
    [Header("Trap Settings")]
    public GameObject[] trapPrefabs;    // Prefab cho cái bẫy
    public int numberOfTraps = 2;  // Số lượng bẫy cần spawn
    public float trapYOffset = 0.5f; // Độ cao Y để bẫy không bị lún
    public List<Vector2Int> trapPositions = new List<Vector2Int>();

    [Header("Game Logic")]
    [Tooltip("Tọa độ (X, Z) của ô vạch đích")]
    public Vector2Int goalCoordinates = new Vector2Int(2, 2); // Ví dụ: ô (2, 2)

    [Header("Random Shape Gaps Settings")]
    [Tooltip("Xác suất một ô được spawn.")]
    [Range(0.1f, 1f)]
    public float spawnChance = 0.9f;
    [Header("References")]
    public Camera mainCamera;
    public PlayerMovement playerMovement;
    private float totalZOffset = 0f;
    public SkillManager skillManager;
    public ScoreController scoreController;
    public ShowTrap showTrap;

    [Header("UI")]
    public GameObject winPanel;


    void Start()
    {
        // KHÔNG CÓ LOGIC MỞ RỘNG KÍCH THƯỚC NỮA. Lưới là W x H bạn đặt.

        // 1. ĐẶT Ô CHẠM ĐÍCH (Touch Point)
        goalCoordinates.x = width - 1;
        goalCoordinates.y = height - 1;

        // Đảm bảo số bẫy không vượt quá số ô trống
        numberOfTraps = Mathf.Min(LevelManager.Instance.levels[LevelManager.Instance.currentLevelIndex].trapCount, (width * height) - 1);

        // 2. KHỞI TẠO VÀ SPAWN LƯỚI
        gridArray = new GameObject[width, height];
        spawnPositions = new List<Vector3>();

        SpawnGrid(); // Hàm này sẽ spawn cả lưới và 1 ô đích dư ra
        SpawnRandomTraps();
        if (playerMovement != null)
        {
            playerMovement.gridManager = this;
        }
    }

    void SpawnGrid()
    {

        // ... (Khai báo và Khởi tạo) ...

        gridArray = new GameObject[width, height];
        GameObject goalTouchTile = null;

        GameObject lastGridTile = null;

        if (landTilePrefab == null)
        {
            Debug.LogError("Lỗi: Chưa gán landTilePrefab!");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // 1. Kiểm tra ngẫu nhiên (Tạo sự thiếu/dư ngẫu nhiên)
                bool isGoalTile = (x == goalCoordinates.x && z == goalCoordinates.y);

                if (!isGoalTile)
                {
                    // Nếu KHÔNG phải ô đích VÀ xác suất không đạt, thì bỏ qua
                    // (Ví dụ: spawnChance = 0.9. Nếu Random.Range > 0.9, bỏ qua spawn)
                    if (Random.Range(0f, 1f) > spawnChance)
                    {
                        continue; // Bỏ qua spawn ô này (tạo ra sự "thiếu đi" ngẫu nhiên)
                    }
                }

                // 2. CODE SPAWN VẪN NHƯ CŨ (Chỉ spawn nếu vượt qua kiểm tra xác suất)


                // 1. Kiểm tra Goal Tile


                bool isLastTile = (x == width - 1 && z == height - 1);

                Vector3 position = new Vector3(x * spacing, 0, (z * spacing) + totalZOffset);

                GameObject newTile = Instantiate(landTilePrefab, position, Quaternion.identity, transform);
                newTile.name = $"Tile ({x}, {z}) (Z Offset: {totalZOffset})";

                TileInfo tileInfo = newTile.GetComponent<TileInfo>();
                if (tileInfo == null) tileInfo = newTile.AddComponent<TileInfo>();

                tileInfo.SetCoords(x, z);
                tileInfo.originalColor = newTile.GetComponent<Renderer>().material.color;

                newTile.tag = "LandTile";

                if (isLastTile)
                {
                    lastGridTile = newTile;
                }

                gridArray[x, z] = newTile;

                
            }
        }
        gridArray[goalCoordinates.x, goalCoordinates.y].tag = "ENDLINE";
        // 2. SPAWN 1 Ô DƯ RA DUY NHẤT LÀ FINISH LINE
        if (lastGridTile != null)
        {
            // FinishLine được đặt dựa trên vị trí của Tile cuối cùng đã dịch chuyển
            Vector3 exitOffset = new Vector3(0, 0, spacing);
            Vector3 finishPosition = lastGridTile.transform.position + exitOffset;

            GameObject finishLine = Instantiate(landTilePrefab, finishPosition, Quaternion.identity, transform);
            finishLine.name = "FINISH LINE (Extra Tile)";
            finishLine.tag = "FinishLine";
            finishLine.GetComponent<Renderer>().material.color = Color.blue;

            TileInfo finishInfo = finishLine.GetComponent<TileInfo>();
            if (finishInfo == null) finishInfo = finishLine.AddComponent<TileInfo>();
            finishInfo.SetCoords(-1, -1);

            // 3. ĐẶT EXIT POINT MỚI CHO CẤP ĐỘ SAU

            if (exitPointPrefab != null && GameObject.FindGameObjectWithTag("ExitPoint") == null)
            {
                // Vị trí Z mới: Gốc map mới + Chiều dài map
                Vector3 newExitPosition = new Vector3(
                    1f,
                    -0.1f, // Đặt ở Y=0 (Mặt đất)
                    totalZOffset + (height * spacing)
                );

                // * SỬ DỤNG INSTANTIATE PREFAB *
                GameObject newExitPoint = Instantiate(exitPointPrefab, newExitPosition, Quaternion.identity);
                newExitPoint.name = "ExitPoint_Active";

                // Prefab đã có tag, nhưng chúng ta vẫn nên kiểm tra
                if (!newExitPoint.CompareTag("ExitPoint"))
                {
                    newExitPoint.tag = "ExitPoint";
                }
            }
        }
    }
    /// <summary>
    /// Spawn bẫy ngẫu nhiên, đảm bảo KHÔNG ở goal và KHÔNG chặn hết đường ra
    /// </summary>
    void SpawnRandomTraps()
    {
        if (trapPrefabs == null || trapPrefabs.Length == 0 || numberOfTraps <= 0) return;

        // 1. Lấy danh sách vị trí khả dụng (trừ Start và Finish)
        List<Vector2Int> spawnableLocations = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (x == 0 && z == 0) continue; // Tránh Start
                if (x == width - 1 && z == height - 1) continue; // Tránh ô cuối (trước FinishLine)

                spawnableLocations.Add(new Vector2Int(x, z));
            }
        }

        int trapsToSpawn = Mathf.Min(numberOfTraps, spawnableLocations.Count);
        if (trapsToSpawn <= 0) return;

        // 2. Thử tìm vị trí đặt bẫy hợp lệ (không chặn đường)
        List<Vector2Int> chosenTrapLocations = new List<Vector2Int>();
        int maxTries = 100;
        bool foundValid = false;

        for (int tryCount = 0; tryCount < maxTries; tryCount++)
        {
            chosenTrapLocations.Clear();
            List<Vector2Int> tempList = new List<Vector2Int>(spawnableLocations);

            // Chọn ngẫu nhiên N vị trí từ tempList
            for (int i = 0; i < trapsToSpawn; i++)
            {
                if (tempList.Count == 0) break;
                int r = Random.Range(0, tempList.Count);
                chosenTrapLocations.Add(tempList[r]);
                tempList.RemoveAt(r);
            }

            // Kiểm tra đường đi từ (0,0) đến (W-1, H-1)
            if (IsFullyConnected(new Vector2Int(0, 0), new Vector2Int(width - 1, height - 1), chosenTrapLocations))
            {
                foundValid = true;
                break;
            }
        }

        // 3. Spawn bẫy nếu hợp lệ
        if (foundValid)
        {
            trapPositions = new List<Vector2Int>(chosenTrapLocations);

            foreach (Vector2Int coords in chosenTrapLocations)
            {
                // Tính toán vị trí thế giới (Có tính đến totalZOffset)
                Vector3 spawnPosition = new Vector3(
                    coords.x * spacing,
                    trapYOffset,
                    (coords.y * spacing) + totalZOffset // Dùng .y vì Vector2Int(x,z) lưu z vào y
                );

                // 🔥 Chọn ngẫu nhiên loại bẫy
                GameObject randomTrap = trapPrefabs[Random.Range(0, trapPrefabs.Length)];

                GameObject trapObj = Instantiate(randomTrap, spawnPosition, Quaternion.identity, transform);

                // Gán làm con của Tile tương ứng để gọn (Tùy chọn)
                if (gridArray[coords.x, coords.y] != null)
                {
                    trapObj.transform.SetParent(gridArray[coords.x, coords.y].transform);
                }

            }
            Debug.Log($"Đã spawn {trapsToSpawn} bẫy.");
        }
        else
        {
            Debug.LogWarning("Không tìm được vị trí đặt bẫy hợp lệ.");
        }
    }
    /// <summary>
    /// Kiểm tra xem tất cả các ô không bẫy có kết nối thành một khối duy nhất (với ô đích) không.
    /// Nếu tất cả ô trống đều kết nối được với ô đích, bản đồ không bị chặn.
    /// </summary>


    bool IsFullyConnected(Vector2Int start, Vector2Int end, List<Vector2Int> obstacles)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        HashSet<Vector2Int> obstacleSet = new HashSet<Vector2Int>(obstacles);

        queue.Enqueue(start);
        visited.Add(start);

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == end) return true;

            for (int i = 0; i < 4; i++)
            {
                Vector2Int neighbor = new Vector2Int(current.x + dx[i], current.y + dy[i]);

                if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
                {
                    if (!visited.Contains(neighbor) && !obstacleSet.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
        return false;
    }
    public void StartNextLevel()
    {
        //0. Tăng LEVEL, nếu level tiếp theo cần đổi scene thì đổi scene
        LevelManager.Instance.LoadNextLevel();
        // Cập nhật UI hiển thị level hiện tại


        // 1. LẤY VỊ TRÍ NEO MỚI TỪ EXIT POINT CŨ
        GameObject oldExit = GameObject.FindGameObjectWithTag("ExitPoint");

        if (oldExit != null)
        {
            // Lấy vị trí Z của ExitPoint cũ (Đây sẽ là tọa độ Z bắt đầu của map mới)
            totalZOffset = oldExit.transform.position.z+1f;

            // Bỏ tag của ExitPoint cũ (để giữ lại nhưng không kích hoạt lại)
            oldExit.tag = "UsedStartPoint";
            oldExit.name = "StartPoint_Used";
        }
        // Nếu oldExit == null (lần chơi đầu), totalZOffset là 0.
        GameObject startPointToRemove = GameObject.Find("StartPoint_Used");
        if (startPointToRemove != null && startPointToRemove != oldExit) // Tránh xóa chính ExitPoint vừa đổi tag
        {
            // Có thể bạn cần một tag riêng cho StartPoint ban đầu nếu nó không phải là ExitPoint cũ
            Destroy(startPointToRemove);
        }
        // 2. PHÁ HỦY MAP CŨ (giữ lại Start/ExitPoint đã đổi tag)
        DestroyCurrentGrid();

        // 3. TẠO MAP MỚI
        gridArray = null;
        spawnPositions.Clear();
        Start(); // Gọi lại Start() để spawn grid mới ở vị trí totalZOffset

        // 4. KÉO CAMERA TỚI VỊ TRÍ MỚI
        if (mainCamera != null)
        {
            // Tính toán tâm Z mới: Z_bắt đầu + nửa chiều dài map
            Vector3 newCenter = new Vector3(6.77f,//HOẶC (width - 1) * spacing / 2f
                                            mainCamera.transform.position.y,
                                            totalZOffset + (height - 1) * spacing / 2f);

            StartCoroutine(MoveCameraSmoothly(newCenter));
        }

        //5. Cho phép chọn kỹ năng lại
        skillManager.EnableSelectSkill();

        //6. Reset điểm số
        scoreController.ResetScore();

        //7. Reset kỹ năng của người chơi
        playerMovement.playerSkill.ResetSkill();

        //8.Bắt đầu hiển thị bẫy cho cấp độ mới
        showTrap.BeginShowTrap();
    }

    public void ResetLevel()
    {
        // 1. LẤY VỊ TRÍ BAN ĐẦU VÀ TRẢ NHÂN VẬT VỀ ĐÓ
        Vector3 startPosition = new Vector3(0, 0.5f, totalZOffset); // Giả sử Start luôn ở (0,0) với offset Z hiện tại
        playerMovement.transform.position = startPosition;

        // 2. PHÁ HỦY MAP HIỆN TẠI VÀ TẠO LẠI
        DestroyCurrentGrid();
        gridArray = null;
        spawnPositions.Clear();
        Start(); // Gọi lại Start() để spawn grid mới ở vị trí totalZOffset

        // 3. KÉO CAMERA VỀ VỊ TRÍ BAN ĐẦU
        if (mainCamera != null)
        {
            Vector3 newCameraPosition = new Vector3(6.77f,//HOẶC (width - 1) * spacing / 2f
                                                 mainCamera.transform.position.y,
                                                 totalZOffset + (height - 1) * spacing / 2f);
           StartCoroutine(MoveCameraSmoothly(newCameraPosition));
        }

        //4. Cho phép chọn kỹ năng lại
        skillManager.EnableSelectSkill();

        //5. Reset điểm số
        scoreController.ResetScore();

        //6. Reset kỹ năng của người chơi
        playerMovement.playerSkill.ResetSkill();

        //Reset animation nhân vật
        playerMovement.playerAnimator.SetBool("isDead", false);

        //7.Bắt đầu hiển thị bẫy cho cấp độ hiện tại
        showTrap.BeginShowTrap();

        //8. Reset trạng thái của người chơi

        //9. Reset lại bẫy

    }


    void DestroyCurrentGrid()
    {
        // Hủy tất cả các tile (con của GridManager)
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // Hủy ExitPoint (Cube thủ công)
        GameObject exit = GameObject.FindGameObjectWithTag("ExitPoint");
        //if (exit != null) Destroy(exit);
    }

    IEnumerator MoveCameraSmoothly(Vector3 targetCenter)
    {
        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        targetCenter.y = startPosition.y;

        while (elapsed < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetCenter, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = targetCenter;
    }
}

