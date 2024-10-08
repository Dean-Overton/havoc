using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Make the health bar face the camera
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
