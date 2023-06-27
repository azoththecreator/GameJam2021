using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Transform player;

    float smoothSpeed = .125f;
    [SerializeField]
    Vector3 offset;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = player.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
        transform.position = smoothPos;
    }
}
