using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    public Vector2 Offset;

    public float CameraMaxY = 11.5f;
    public float CameraMinY = 1.5f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Vector3 trackPos = target.position + new Vector3(Offset.x, Offset.y, 0);

            if (trackPos.y > CameraMaxY)
                trackPos.y = CameraMaxY;
            if (trackPos.y < CameraMinY)
                trackPos.y = CameraMinY;

            Vector3 point = GetComponent<Camera>().WorldToViewportPoint(trackPos);
            Vector3 delta = trackPos - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }
}
