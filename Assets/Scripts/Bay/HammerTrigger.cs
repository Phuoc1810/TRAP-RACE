using UnityEngine;

public class HammerTrigger : MonoBehaviour
{
    public HammerTrap hammer;   // kéo script HammerRotate vào đây

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckIfPlayerMoveToExitPoint(other.gameObject))
            {
                return;
            }

            hammer.ActivateHammer();
        }
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
