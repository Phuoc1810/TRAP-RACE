using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiStageGridManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject landTilePrefab;   // Ô đất thường
    public GameObject checkpointPrefab; // Ô Đích Giai đoạn 1 (Thay thế ô đất cuối)
    public GameObject midpointPrefab;   // Bục đứng giữa khoảng không
    public GameObject finishLinePrefab; // Ô Đích Giai đoạn 2 (Thay thế ô đất cuối)
    public GameObject exitPointPrefab;  // Điểm chuyển cấp (Vẫn nằm ngoài để Player đứng)
    public GameObject[] trapPrefabs;

    [Header("Grid Settings")]
    public int width = 5;
    public int height = 5;
    public float spacing = 1.1f;
    public float gapBetweenStages = 0.5f; // Khoảng cách giữa 2 stage

    [Header("Camera")]
    public Camera mainCamera;
    public float cameraMoveDuration = 1.5f;

    private float currentZCursor = 0f;
    public int trapsPerStage = 3; // Số lượng bẫy muốn spawn trong mỗi giai đoạn
    public float trapYOffset = 0.5f; // Độ cao của bẫy so với mặt đất
    void Start()
    {
        currentZCursor = 0f;
        SpawnFullLevel();
    }

    public void StartNextLevel()
    {
        GameObject oldExit = GameObject.FindGameObjectWithTag("ExitPoint");
        if (oldExit != null)
        {
            currentZCursor = oldExit.transform.position.z + spacing;
            oldExit.tag = "UsedStartPoint";
            oldExit.name = "StartPoint_Used";
        }
        DestroyCurrentGrid();
        SpawnFullLevel();
    }

    void SpawnFullLevel()
    {
        float levelStartZ = currentZCursor;

        // --- GIAI ĐOẠN 1 ---
        // 1. Tạo lưới và lấy về mảng 2 chiều các ô đất
        GameObject[,] stage1Grid = SpawnGridSegment(checkpointPrefab, "Checkpoint");

        // 2. Spawn bẫy dựa trên mảng đất vừa tạo
        SpawnRandomTraps(stage1Grid);

        // --- MIDPOINT ---
        // Lấy vị trí tile cuối cùng từ mảng stage1Grid
        GameObject lastTileS1 = stage1Grid[width - 1, height - 1];
        Vector3 cpPos = lastTileS1.transform.position;
        Vector3 midPos = new Vector3(1, 0, cpPos.z + (gapBetweenStages / 2f) + (spacing / 2f));

        GameObject mid = Instantiate(midpointPrefab, midPos, Quaternion.identity, transform);
        mid.tag = "Midpoint";
        if (mid.GetComponent<TileInfo>() == null) mid.AddComponent<TileInfo>();

        currentZCursor += gapBetweenStages;

        // --- GIAI ĐOẠN 2 ---
        GameObject[,] stage2Grid = SpawnGridSegment(finishLinePrefab, "FinishLine");
        SpawnRandomTraps(stage2Grid);

        // --- EXIT POINT ---
        GameObject lastTileS2 = stage2Grid[width - 1, height - 1];
        if (exitPointPrefab != null)
        {
            Vector3 exitPos = lastTileS2.transform.position + new Vector3(-1, 0, spacing);
            GameObject exit = Instantiate(exitPointPrefab, exitPos, Quaternion.identity);
            exit.name = "ExitPoint_Active";
            exit.tag = "ExitPoint";
        }

        // --- CAMERA ---
        if (mainCamera != null)
        {
            float levelEndZ = currentZCursor;
            float centerZ = (levelStartZ + levelEndZ) / 2f;
            Vector3 newCamPos = new Vector3((width - 1) * spacing / 2f, mainCamera.transform.position.y, centerZ);
            StartCoroutine(MoveCameraSmoothly(newCamPos));
        }
    }

    // 🔥 SỬA ĐỔI QUAN TRỌNG: Hàm này giờ trả về mảng 2 chiều GameObject[,]
    GameObject[,] SpawnGridSegment(GameObject endTilePrefab, string endTileTag)
    {
        GameObject[,] segmentGrid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * spacing, 0, (z * spacing) + currentZCursor);
                GameObject newTile;

                if (x == width - 1 && z == height - 1)
                {
                    newTile = Instantiate(endTilePrefab, pos, Quaternion.identity, transform);
                    newTile.tag = endTileTag;
                    newTile.name = $"{endTileTag} ({x}, {z})";
                }
                else
                {
                    newTile = Instantiate(landTilePrefab, pos, Quaternion.identity, transform);
                    newTile.tag = "LandTile";
                    newTile.name = $"Tile ({x}, {z})";
                }

                TileInfo ti = newTile.GetComponent<TileInfo>();
                if (ti == null) ti = newTile.AddComponent<TileInfo>();
                ti.SetCoords(x, z);

                // Lưu vào mảng
                segmentGrid[x, z] = newTile;
            }
        }
        currentZCursor += (height * spacing);
        return segmentGrid; // Trả về mảng để dùng cho việc đặt bẫy
    }

   

    void DestroyCurrentGrid()
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    IEnumerator MoveCameraSmoothly(Vector3 targetPos)
    {
        Vector3 startPos = mainCamera.transform.position;
        float elapsed = 0f;
        while (elapsed < cameraMoveDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / cameraMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = targetPos;
    }
    void SpawnRandomTraps(GameObject[,] gridSegment)
    {
        if (trapPrefabs == null || trapPrefabs.Length == 0 || trapsPerStage <= 0) return;

        // 1. Lấy danh sách vị trí có thể đặt (trừ Start 0,0 và End W-1,H-1)
        List<Vector2Int> spawnableLocations = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (x == 0 && z == 0) continue; // Tránh Start
                if (x == width - 1 && z == height - 1) continue; // Tránh End

                spawnableLocations.Add(new Vector2Int(x, z));
            }
        }

        int trapsToSpawn = Mathf.Min(trapsPerStage, spawnableLocations.Count);
        if (trapsToSpawn <= 0) return;

        // 2. Thử tìm vị trí hợp lệ (Pathfinding Check)
        List<Vector2Int> chosenTrapLocations = new List<Vector2Int>();
        int maxTries = 100;
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

            // Kiểm tra xem đặt bẫy ở đây có chặn đường từ (0,0) đến (W-1, H-1) không
            if (IsFullyConnected(new Vector2Int(0, 0), new Vector2Int(width - 1, height - 1), chosenTrapLocations))
            {
                foundValidPlacement = true;
                break; // Tìm thấy rồi thì thoát vòng lặp thử
            }
        }

        // 3. Tiến hành Spawn nếu tìm thấy
        if (foundValidPlacement)
        {
            foreach (Vector2Int pos in chosenTrapLocations)
            {
                // Lấy ô đất tại vị trí đó từ mảng gridSegment
                GameObject tile = gridSegment[pos.x, pos.y];

                Vector3 trapPos = new Vector3(
                    tile.transform.position.x,
                    trapYOffset,
                    tile.transform.position.z
                );

                // Chọn ngẫu nhiên loại bẫy
                GameObject randomTrap = trapPrefabs[Random.Range(0, trapPrefabs.Length)];

                GameObject newTrap = Instantiate(randomTrap, trapPos, Quaternion.identity);
                newTrap.transform.SetParent(tile.transform); // Gán làm con của ô đất
            }
            Debug.Log($"Đã spawn {trapsToSpawn} bẫy thành công.");
        }
        else
        {
            Debug.LogWarning("Không tìm được vị trí đặt bẫy hợp lệ (bị chặn đường).");
        }
    }

    // ===========================================================
    // 🔥 THUẬT TOÁN BFS KIỂM TRA ĐƯỜNG ĐI (PATHFINDING)
    // ===========================================================
    bool IsFullyConnected(Vector2Int start, Vector2Int end, List<Vector2Int> obstacles)
    {
        // Sử dụng BFS để tìm đường
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        // Biến đổi List obstacles thành HashSet để tra cứu nhanh hơn
        HashSet<Vector2Int> obstacleSet = new HashSet<Vector2Int>(obstacles);

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Nếu đến được đích
            if (current == end) return true;

            // Kiểm tra 4 hướng xung quanh
            for (int i = 0; i < 4; i++)
            {
                Vector2Int neighbor = new Vector2Int(current.x + dx[i], current.y + dy[i]);

                // Kiểm tra biên
                if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
                {
                    // Nếu chưa thăm VÀ không phải là bẫy
                    if (!visited.Contains(neighbor) && !obstacleSet.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        return false; // Không tìm thấy đường
    }

    // Các hàm phụ trợ giữ nguyên
   

}