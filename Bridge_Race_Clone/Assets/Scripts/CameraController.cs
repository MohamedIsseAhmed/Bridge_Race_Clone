using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float followSpeed = 9;
    [SerializeField] private float turnSpeedlowSpeed = 5;
    [SerializeField] private Vector3 followOffset;
    [SerializeField] private Vector3 camPositionOnWin;
    private void Start()
    {
        WinManager.OnWinCameraPositionEvent += CamerPositionOnWin;
    }
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
        Vector3 desiredPos = player.position + followOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
      
    }
    private void CamerPositionOnWin(object s,EventArgs e)
    {
        transform.position =camPositionOnWin;
    }
    private void OnDisable()
    {
        WinManager.OnWinCameraPositionEvent -= CamerPositionOnWin;
    }
}
