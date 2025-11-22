using UnityEngine;
using System.Collections;

public class RockTrap : MonoBehaviour
{
    [Header("Cài đặt bẫy")]
    public Vector3 rockSpawnOffset = new Vector3(0, 9, 0);
    public float fallSpeed = 5f;
    public float destroyDelay = 3f;
    public GameObject rockPrefab;

    private GameObject currentRock;
    private bool isRockFalling = false;
    private BoxCollider triggerCollider;
    private float lastActivationTime = -10f; // Thời gian kích hoạt cuối
    private float activationCooldown = 0.5f; // Thời gian chờ giữa các lần kích hoạt

    void Start()
    {
        triggerCollider = GetComponent<BoxCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<BoxCollider>();
        }
        triggerCollider.isTrigger = true;
    }

    void Update()
    {
        if (isRockFalling && currentRock != null)
        {
            UpdateRockFall();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra cooldown và trạng thái hiện tại
        if (other.CompareTag("Player") &&
            !isRockFalling &&
            Time.time - lastActivationTime > activationCooldown)
        {
            if (CheckIfPlayerMoveToExitPoint(other.gameObject))
            {
                return;
            }

            Debug.Log("Kích hoạt bẫy - chỉ 1 cục đá được tạo");
            lastActivationTime = Time.time;
            ActivateTrap();
        }
    }

    void ActivateTrap()
    {
        isRockFalling = true;

        // Tạo cục đá ở vị trí phía trên trap
        Vector3 spawnPosition = transform.position + rockSpawnOffset;

        if (rockPrefab != null)
        {
            currentRock = Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            currentRock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            currentRock.transform.position = spawnPosition;
            currentRock.name = "FallingRock";
        }

        Debug.Log("Đã tạo 1 cục đá tại: " + spawnPosition);
        StartCoroutine(DestroyRockAfterDelay());
    }

    void UpdateRockFall()
    {
        currentRock.transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        if (currentRock.transform.position.y <= transform.position.y)
        {
            Vector3 landedPosition = currentRock.transform.position;
            landedPosition.y = transform.position.y;
            currentRock.transform.position = landedPosition;
        }
    }

    IEnumerator DestroyRockAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);

        if (currentRock != null)
        {
            Destroy(currentRock);
            currentRock = null;
            isRockFalling = false;
            Debug.Log("Đã xóa cục đá");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (triggerCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(triggerCollider.center, triggerCollider.size);
        }

        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.identity;
        Vector3 spawnPosition = transform.position + rockSpawnOffset;
        Gizmos.DrawWireSphere(spawnPosition, 0.5f);
        Gizmos.DrawLine(transform.position, spawnPosition);
    }

    public bool CheckIfPlayerMoveToExitPoint(GameObject other)
    {
        var player = other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            if (player.isMoveToExitPoint)
            {
                return true;
            }
        }

        return false;
    }
}