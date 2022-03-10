using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointAnimate : MonoBehaviour
{
    [Header("Reference Objects")]
    public GameObject mainCamera;

    [Header("Animation Variables")]
    public float animationSpeed;
    public float delta;

    private float startY;

    private void Start()
    {
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float y = startY + Mathf.PingPong(Time.time * animationSpeed, delta);
        Vector3 pos = new Vector3(transform.position.x, y, transform.position.z);
        transform.position = pos;

        transform.LookAt(mainCamera.transform);
    }
}
