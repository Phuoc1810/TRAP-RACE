using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float hiddenY = 1.2f;   // vị trí khi ẩn
    public float shownY = 2.2f;    // vị trí khi hiện lên
    public float moveSpeed = 5f;   // tốc độ di chuyển

    private bool playerInside = false;

    void Start()
    {
        // Set vị trí ban đầu (ẩn dưới đất)
        Vector3 pos = transform.position;
        pos.y = hiddenY;
        transform.position = pos;
    }

    void Update()
    {
        // Nếu có người chơi → đi lên, không thì đi xuống
        float targetY = playerInside ? shownY : hiddenY;

        Vector3 targetPos = new Vector3(transform.position.x, targetY, transform.position.z);

        // Di chuyển mượt
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckIfPlayerMoveToExitPoint(other))
            {
                return;
            }

            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    public bool CheckIfPlayerMoveToExitPoint(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            if (playerMovement.isMoveToExitPoint)
            {
                return true;
            }
        }

        return false;
    }
}
