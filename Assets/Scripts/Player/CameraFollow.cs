using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    private Vector3 cameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // LateUpdate because we should update camera location only after everything else has updated
    private void LateUpdate()
    {
        cameraRotation += GetRotation();
        cameraRotation = CameraLimit(cameraRotation);
        transform.eulerAngles = player.transform.eulerAngles + cameraRotation;
    }

    Vector3 GetRotation()
    {
        Vector3 mouseMovement = Vector3.zero;
        mouseMovement.x = -1 * Input.GetAxis("Mouse Y") * player.GetComponent<PlayerController>().lookSensitivityY;
        return mouseMovement;
    }

    Vector3 CameraLimit(Vector3 cameraRotation)
    {
        if (cameraRotation.x < -50)
        {
            cameraRotation.x = -50;
        }
        if (cameraRotation.x > 50)
        {
            cameraRotation.x = 50;
        }
        return cameraRotation;
    }
}
