using System.Collections;
using UnityEngine;

public class RockTrap : MonoBehaviour
{
    public GameObject[] rockPrefabs;   // danh sách đá có thể rơi
    public Transform spawnArea;        // box collider để giới hạn spawn
    public int rockCount = 5;          // số lượng đá rơi
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
        BoxCollider box = spawnArea.GetComponent<BoxCollider>();

        for (int i = 0; i < rockCount; i++)
        {
            // Chọn prefab ngẫu nhiên
            GameObject rock = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

            // Random trong Box Collider
            Vector3 randomPos = GetRandomPointInBox(box);

            // Set chiều cao Y = spawnHeightY
            randomPos.y = spawnHeightY;

            Instantiate(rock, randomPos, Quaternion.identity);

            yield return new WaitForSeconds(delayPerRock);
        }
    }

    private Vector3 GetRandomPointInBox(BoxCollider box)
    {
        Vector3 center = box.transform.position + box.center;
        Vector3 size = box.size;

        return new Vector3(
            Random.Range(center.x - size.x / 2, center.x + size.x / 2),
            Random.Range(center.y - size.y / 2, center.y + size.y / 2),
            Random.Range(center.z - size.z / 2, center.z + size.z / 2)
        );
    }
}
