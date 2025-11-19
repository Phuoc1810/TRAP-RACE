using System.Collections;
using UnityEngine;

public class HammerTrap : MonoBehaviour
{
    public GameObject hammerMesh; // object chứa cây búa (có thể tắt/bật)

    public float startRotationZ = -170f;
    public float endRotationZ = -87f;
    public float rotateSpeed = 200f;

    private bool activated = false;

    void Start()
    {
        // Tắt cây búa lúc ban đầu
        if (hammerMesh != null)
            hammerMesh.SetActive(false);

        // Set góc ban đầu
        Vector3 rot = transform.localEulerAngles;
        rot.z = startRotationZ;
        transform.localEulerAngles = rot;
    }

    public void ActivateHammer()
    {
        if (!activated)
        {
            activated = true;

            // Bật cây búa lên
            if (hammerMesh != null)
                hammerMesh.SetActive(true);

            StartCoroutine(RotateHammer());
        }
    }

    private IEnumerator RotateHammer()
    {
        while (true)
        {
            Vector3 currentRot = transform.localEulerAngles;

            float newZ = Mathf.MoveTowardsAngle(
                currentRot.z,
                endRotationZ,
                rotateSpeed * Time.deltaTime
            );

            transform.localEulerAngles = new Vector3(currentRot.x, currentRot.y, newZ);

            if (Mathf.Abs(Mathf.DeltaAngle(newZ, endRotationZ)) < 0.1f)
                break;

            yield return null;
        }
        // Sau khi xoay xong, đợi 3 giây rồi xóa cây búa
        yield return new WaitForSeconds(1f);

        if (hammerMesh != null)
            Destroy(hammerMesh);
    }
}
