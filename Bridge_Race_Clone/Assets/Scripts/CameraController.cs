using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float followSpeed = 9;
    [SerializeField] private float folturnSpeedlowSpeed = 5;
    [SerializeField] private Vector3 followOffset;
    private void LateUpdate()
    {
        FollowPlayer();
    }
    private void FollowPlayer()
    {
        Vector3 toPlayer=(player.position-transform.position).normalized;
        Quaternion lookDirection = Quaternion.LookRotation(toPlayer);
        transform.position = Vector3.Lerp(transform.position, player.position + followOffset, followSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, folturnSpeedlowSpeed*Time.deltaTime);
    }

}
