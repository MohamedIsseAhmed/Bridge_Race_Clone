using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float followSpeed = 9;
    [SerializeField] private float turnSpeedlowSpeed = 5;
    [SerializeField] private Vector3 followOffset;
    [SerializeField] private Vector3 camPositionOnWin;

    private void LateUpdate()
    {
        if (!WinManager.Instance.IsGameOver)
        {
            FollowPlayer();
        }
       
    }

  
    private void FollowPlayer()
    {
        Vector3 toPlayer=(player.position-transform.position).normalized;
        Quaternion lookDirection = Quaternion.LookRotation(toPlayer);
        transform.position = Vector3.Lerp(transform.position, player.position + followOffset, followSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, turnSpeedlowSpeed * Time.deltaTime);
    }
    private void CamerPositionOnWin()
    {
        transform.position = transform.position+camPositionOnWin;
    }
}
