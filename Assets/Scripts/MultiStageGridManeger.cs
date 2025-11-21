using UnityEngine;
using System.Collections;

public class MultiStageGridManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject landTilePrefab;   // Ô đất thường
    public GameObject checkpointPrefab; // Ô Đích Giai đoạn 1 (Thay thế ô đất cuối)
    public GameObject midpointPrefab;   // Bục đứng giữa khoảng không
    public GameObject finishLinePrefab; // Ô Đích Giai đoạn 2 (Thay thế ô đất cuối)
    public GameObject exitPointPrefab;  // Điểm chuyển cấp (Vẫn nằm ngoài để Player đứng)

    [Header("Grid Settings")]
    public int width = 5;
    public int height = 5;
    public float spacing = 1.1f;
    public float gapBetweenStages = 0.5f; // Khoảng cách giữa 2 stage

    [Header("Camera")]
    public Camera mainCamera;
    public float cameraMoveDuration = 1.5f;

    private float currentZCursor = 0f;

    void Start()
    {
        currentZCursor = 0f;
        SpawnFullLevel();
    }

    public void StartNextLevel()
    {
        // 1. LẤY ĐIỂM NEO TỪ EXIT POINT CŨ
        GameObject oldExit = GameObject.FindGameObjectWithTag("ExitPoint");
        if (oldExit != null)
        {
            currentZCursor = oldExit.transform.position.z+1;
            oldExit.tag = "UsedStartPoint";
            oldExit.name = "StartPoint_Used";
        }

        // 2. XÓA MAP CŨ
        DestroyCurrentGrid();

        // 3. SPAWN LẠI
        SpawnFullLevel();
    }

    void SpawnFullLevel()
    {
        float levelStartZ = currentZCursor;

        // ====================================================
        // GIAI ĐOẠN 1: TẠO MẢNG 1 (Ô CUỐI LÀ CHECKPOINT)
        // ====================================================

        // Gọi hàm tạo lưới, truyền vào Prefab đặc biệt là Checkpoint
        GameObject lastTileS1 = SpawnGridSegment(checkpointPrefab, "Checkpoint");

        // ====================================================
        // KHOẢNG TRỐNG: MIDPOINT
        // ====================================================

        // Midpoint nằm giữa khoảng trống sau Checkpoint
        // (Lưu ý: lastTileS1 bây giờ chính là Checkpoint)
        Vector3 cpPos = lastTileS1.transform.position;

        // Tính vị trí Midpoint: Cách Checkpoint một nửa khoảng Gap
        Vector3 midPos = new Vector3(
            1,
            0,
            cpPos.z + (gapBetweenStages / 2f) + (spacing / 2f) // Căn chỉnh một chút cho đẹp
        );

        GameObject mid = Instantiate(midpointPrefab, midPos, Quaternion.identity, transform);
        mid.name = "MIDPOINT";
        mid.tag = "Midpoint";
        if (mid.GetComponent<TileInfo>() == null) mid.AddComponent<TileInfo>();
        if (mid.tag == "Midpoint") mid.GetComponent<Renderer>().material.color = Color.gray;

        // Cập nhật con trỏ Z nhảy qua khoảng trống
        currentZCursor += gapBetweenStages;

        // ====================================================
        // GIAI ĐOẠN 2: TẠO MẢNG 2 (Ô CUỐI LÀ FINISH LINE)
        // ====================================================

        // Gọi hàm tạo lưới, truyền vào Prefab đặc biệt là FinishLine
        GameObject lastTileS2 = SpawnGridSegment(finishLinePrefab, "FinishLine");

        // ====================================================
        // TẠO EXIT POINT (NẰM NGOÀI LƯỚI)
        // ====================================================

        // ExitPoint vẫn phải nằm tách biệt sau FinishLine để Player đi tới đó và chuyển màn
        if (exitPointPrefab != null)
        {
            // Đặt ngay sau FinishLine (theo trục Z hoặc X tùy bạn, ở đây là trục Z)
            Vector3 exitPos = lastTileS2.transform.position + new Vector3(-1, 0, spacing);

            GameObject exit = Instantiate(exitPointPrefab, exitPos, Quaternion.identity);
            exit.name = "ExitPoint_Active";
            exit.tag = "ExitPoint";
        }

        // DI CHUYỂN CAMERA
        if (mainCamera != null)
        {
            float levelEndZ = currentZCursor;
            float centerZ = (levelStartZ + levelEndZ) / 2f;
            Vector3 newCamPos = new Vector3((width - 1) * spacing / 2f, mainCamera.transform.position.y, centerZ);
            StartCoroutine(MoveCameraSmoothly(newCamPos));
        }
    }

    // --- HÀM SPAWN ĐƯỢC SỬA ĐỔI ---
    // Nhận vào: Prefab đặc biệt cho ô cuối cùng và Tag của nó
    GameObject SpawnGridSegment(GameObject endTilePrefab, string endTileTag)
    {
        GameObject lastTile = null;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * spacing, 0, (z * spacing) + currentZCursor);
                GameObject newTile;

                // KIỂM TRA: NẾU LÀ Ô CUỐI CÙNG (Góc trên phải)
                if (x == width - 1 && z == height - 1)
                {
                    // * THAY THẾ LAND TILE BẰNG PREFAB ĐẶC BIỆT (Checkpoint/FinishLine) *
                    newTile = Instantiate(endTilePrefab, pos, Quaternion.identity, transform);
                    newTile.name = $"{endTileTag} ({x}, {z})";
                    newTile.tag = endTileTag; // Gán tag (Checkpoint hoặc FinishLine)

                    // Đổi màu để dễ nhìn (Tùy chọn)
                    if (endTileTag == "FinishLine") newTile.GetComponent<Renderer>().material.color = Color.blue;
                    if (endTileTag == "Checkpoint") newTile.GetComponent<Renderer>().material.color = Color.yellow;
                }
                else
                {
                    // CÁC Ô CÒN LẠI LÀ LAND TILE BÌNH THƯỜNG
                    newTile = Instantiate(landTilePrefab, pos, Quaternion.identity, transform);
                    newTile.name = $"Tile ({x}, {z})";
                    newTile.tag = "LandTile";
                }

                // Setup chung cho mọi ô (bao gồm cả ô đặc biệt)
                TileInfo ti = newTile.GetComponent<TileInfo>();
                if (ti == null) ti = newTile.AddComponent<TileInfo>();
                ti.SetCoords(x, z);

                // Lưu lại ô cuối cùng
                if (x == width - 1 && z == height - 1) lastTile = newTile;
            }
        }

        // Cập nhật Z cho lần sau
        currentZCursor += (height * spacing);
        return lastTile;
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
}