using Unity.VisualScripting;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    // Biến này sẽ lưu tọa độ grid (ví dụ: ô (0,0), (0,1), (1,2))
    public int x;
    public int z;

    // Một hàm tiện ích để gán tọa độ
    public void SetCoords(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    // (Tùy chọn) Lưu màu gốc để reset khi vuốt xong
    [HideInInspector] // Giấu đi cho gọn Inspector
    public Color originalColor;
    public Vector2Int GetCoords()
    {
        return new Vector2Int(x, z);
    }
}
