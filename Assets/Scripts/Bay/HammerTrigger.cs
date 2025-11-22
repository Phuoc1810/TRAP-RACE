using UnityEngine;

public class HammerTrigger : MonoBehaviour
{
    public HammerTrap hammer;   // kéo script HammerRotate vào đây

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckIfPlayerMoveToExitPoint(other))
            {
                return;
            }

            hammer.ActivateHammer();
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
