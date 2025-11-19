using System.Collections;
using UnityEngine;

public class RockTrap : MonoBehaviour
{
    public GameObject[] rockPrefabs;   // danh sách đá có thể rơi
    public Transform tramDorm;         // vị trí tram dorm nơi đá sẽ rơi
    public int rockCount = 1;          // số lượng đá rơi
    public float spawnHeightY = 9f;    // height Y nơi đá rơi xuống
    public float delayPerRock = 0.1f;  // thời gian giữa mỗi viên đá

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            activated = true;
            StartCoroutine(SpawnRocks());
        }
    }

    private IEnumerator SpawnRocks()
    {
        for (int i = 0; i < rockCount; i++)
        {
            // Chọn prefab ngẫu nhiên
            GameObject rock = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

            // Lấy vị trí của tram dorm
            Vector3 spawnPos = tramDorm.position;

            // Set chiều cao Y = spawnHeightY
            spawnPos.y = spawnHeightY;

            // Tạo đá và bắt đầu coroutine xóa sau 2 giây
            GameObject rockInstance = Instantiate(rock, spawnPos, Quaternion.identity);
            StartCoroutine(DestroyRockAfterDelay(rockInstance, 3f));

            yield return new WaitForSeconds(delayPerRock);
        }
    }

    private IEnumerator DestroyRockAfterDelay(GameObject rock, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(rock);
    }
}