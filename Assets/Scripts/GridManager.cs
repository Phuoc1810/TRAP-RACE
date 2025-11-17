using UnityEngine;
using System.Collections.Generic; // Cần dùng cho List

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
    public GameObject trapPrefab;   // Prefab cho cái bẫy
    public int numberOfTraps = 2;  // Số lượng bẫy cần spawn
    public float trapYOffset = 0.5f; // Độ cao Y để bẫy không bị lún

    [Header("Game Logic")]
    [Tooltip("Tọa độ (X, Z) của ô vạch đích")]
    public Vector2Int goalCoordinates = new Vector2Int(2, 2); // Ví dụ: ô (2, 2)

    [Header("Random Shape Gaps Settings")]
    [Tooltip("Xác suất một ô được spawn.")]
    [Range(0.1f, 1f)]
    public float spawnChance = 0.9f;

    void Start()
    {
        // KHÔNG CÓ LOGIC MỞ RỘNG KÍCH THƯỚC NỮA. Lưới là W x H bạn đặt.

        // 1. ĐẶT Ô CHẠM ĐÍCH (Touch Point)
        goalCoordinates.x = width - 1;
        goalCoordinates.y = height - 1;

        // Đảm bảo số bẫy không vượt quá số ô trống
        numberOfTraps = Mathf.Min(numberOfTraps, (width * height) - 1);

        // 2. KHỞI TẠO VÀ SPAWN LƯỚI
        gridArray = new GameObject[width, height];
        spawnPositions = new List<Vector3>();

        SpawnGrid(); // Hàm này sẽ spawn cả lưới và 1 ô đích dư ra
        SpawnRandomTraps();
    }

    void SpawnGrid()
    {

        // ... (Khai báo và Khởi tạo) ...

        gridArray = new GameObject[width, height];
        GameObject goalTouchTile = null;



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


                bool isGoalTouchPoint = (x == goalCoordinates.x && z == goalCoordinates.y);

                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject newTile = Instantiate(landTilePrefab, position, Quaternion.identity, transform);
                newTile.name = $"Tile ({x}, {z})";

                TileInfo tileInfo = newTile.GetComponent<TileInfo>();
                if (tileInfo != null)
                {
                    tileInfo.SetCoords(x, z);
                    tileInfo.originalColor = newTile.GetComponent<Renderer>().material.color;
                }

                // Gán Tag cho Ô Chạm Đích (Goal Touch Point)
                if (isGoalTouchPoint)
                {
                    newTile.tag = "GoalTouch"; // Ô cuối cùng của đường đi
                   
                    goalTouchTile = newTile; // Lưu lại ô này
                }
                else
                {
                    newTile.tag = "LandTile";
                }

                gridArray[x, z] = newTile;



                // 2. SPAWN 1 Ô ĐÍCH DUY NHẤT (Vạch đích thực sự)
                if (goalTouchTile != null)
                {
                    // Tính toán vị trí dư ra 1 ô (Offset 1 đơn vị spacing theo trục X)
                    Vector3 exitOffset = new Vector3(0, 0, spacing);
                    Vector3 exitPosition = goalTouchTile.transform.position + exitOffset;

                    GameObject finishLine = Instantiate(landTilePrefab, exitPosition, Quaternion.identity, transform);
                    finishLine.name = "FINISH LINE (Extra Tile)";
                    finishLine.tag = "FinishLine"; // Tag vạch đích cuối cùng
                    finishLine.GetComponent<Renderer>().material.color = Color.blue;

                    gridArray[x, z] = newTile;

                    if (tileInfo != null)
                    {
                        tileInfo.SetCoords(x, z);
                        // Lưu lại màu gốc
                        tileInfo.originalColor = newTile.GetComponent<Renderer>().material.color;
                    }
                    else
                    {
                        Debug.LogError($"Prefab {landTilePrefab.name} thiếu script TileInfo!");
                    }



                    // 4. Lưu lại để "bắt vị trí" sau này
                    gridArray[x, z] = newTile;

                }
            }
        } 
            // ... (phần Debug.Log giữ nguyên) ...
        
    

        Debug.Log($"Đã tạo xong lưới {width}x{height}!");

        // Ví dụ cách lấy vị trí ô ở giữa (1, 1)
        if (width > 1 && height > 1)
        {
            GameObject middleTile = gridArray[1, 1];
            Debug.Log($"Vị trí 'point' của ô ở giữa (1,1) là: {middleTile.transform.position}");
        }
    }
    /// <summary>
    /// Spawn bẫy ngẫu nhiên, đảm bảo KHÔNG ở goal và KHÔNG chặn hết đường ra
    /// </summary>
    void SpawnRandomTraps()
    {
        if (trapPrefab == null)
        {
            Debug.LogWarning("Chưa gán 'Trap Prefab', bỏ qua spawn bẫy.");
            return;
        }

        // 1. Tạo danh sách các VỊ TRÍ CÓ THỂ ĐẶT BẪY (Chỉ loại trừ Goal)
        List<Vector2Int> spawnableLocations = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Bỏ qua nếu là ô Goal
                if (x == goalCoordinates.x && z == goalCoordinates.y) continue;

                spawnableLocations.Add(new Vector2Int(x, z));
            }
        }

        int trapsToSpawn = Mathf.Min(numberOfTraps, spawnableLocations.Count);
        if (trapsToSpawn <= 0) return;

        // 2. Vòng lặp "thử" đặt bẫy và kiểm tra tính liên kết
        List<Vector2Int> chosenTrapLocations = new List<Vector2Int>();
        int maxTries = 100; // Số lần thử tối đa để tránh vòng lặp vô hạn
        bool foundValidPlacement = false;

        for (int tryCount = 0; tryCount < maxTries; tryCount++)
        {
            chosenTrapLocations.Clear();
            List<Vector2Int> tempList = new List<Vector2Int>(spawnableLocations);

            // Chọn ngẫu nhiên N vị trí
            for (int i = 0; i < trapsToSpawn; i++)
            {
                if (tempList.Count == 0) break;
                int randomIndex = Random.Range(0, tempList.Count);
                chosenTrapLocations.Add(tempList[randomIndex]);
                tempList.RemoveAt(randomIndex);
            }

            // KIỂM TRA TÍNH LIÊN KẾT (Bước Pathfinding): 
            // Bắt đầu kiểm tra từ ô đích.
            if (IsFullyConnected(goalCoordinates, chosenTrapLocations))
            {
                foundValidPlacement = true;
                break;
            }
        }

        // 3. Spawn bẫy nếu tìm thấy vị trí hợp lệ
        if (foundValidPlacement)
        {
            Debug.Log($"Đã tìm thấy vị trí đặt bẫy hợp lệ. Đang spawn {trapsToSpawn} bẫy.");
            foreach (Vector2Int pos in chosenTrapLocations)
            {
                GameObject tile = gridArray[pos.x, pos.y];
                Vector3 trapPosition = new Vector3(
                    tile.transform.position.x,
                    trapYOffset,
                    tile.transform.position.z
                );

                GameObject newTrap = Instantiate(trapPrefab, trapPosition, Quaternion.identity);
                newTrap.name = $"Trap ({pos.x}, {pos.y})";
                newTrap.transform.SetParent(tile.transform);
            }
        }
        else
        {
            Debug.LogWarning($"Không thể tìm vị trí đặt {trapsToSpawn} bẫy mà không chặn đường sau {maxTries} lần thử!");
        }
    }
    /// <summary>
    /// Kiểm tra xem tất cả các ô không bẫy có kết nối thành một khối duy nhất (với ô đích) không.
    /// Nếu tất cả ô trống đều kết nối được với ô đích, bản đồ không bị chặn.
    /// </summary>
   
   
    private bool IsFullyConnected(Vector2Int startNode, List<Vector2Int> trapPositions)
    {
        // Dùng HashSet để tra cứu bẫy nhanh hơn
        HashSet<Vector2Int> traps = new HashSet<Vector2Int>(trapPositions);
        
        // Tính tổng số ô không bẫy
        int totalNonTrapTiles = (width * height) - traps.Count;
        if (totalNonTrapTiles <= 0) return true; 

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startNode);
        visited.Add(startNode);
        
        int reachableNonTrapTiles = 0;

        // Các hướng di chuyển Ngang, Dọc
        int[] dX = { 0, 0, 1, -1 };
        int[] dZ = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            reachableNonTrapTiles++;

            // Kiểm tra 4 hướng
            for (int i = 0; i < 4; i++)
            {
                Vector2Int neighbor = new Vector2Int(current.x + dX[i], current.y + dZ[i]);

                // 1. Kiểm tra biên
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height) continue;

                // 2. Kiểm tra bẫy
                if (traps.Contains(neighbor)) continue;

                // 3. Kiểm tra đã thăm
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        // So sánh số ô có thể đến được với tổng số ô trống
        return reachableNonTrapTiles == totalNonTrapTiles;
    }
}

