using UnityEngine;

public class HammerTrigger : MonoBehaviour
{
    public HammerTrap hammer;   // kéo script HammerRotate vào đây

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hammer.ActivateHammer();
        }
    }
}
