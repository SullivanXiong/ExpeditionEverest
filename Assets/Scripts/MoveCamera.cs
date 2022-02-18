using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public Transform player;

    // camera update should occur after all other scripts
    void LateUpdate()
    {
        transform.position = player.transform.position;
    }
}