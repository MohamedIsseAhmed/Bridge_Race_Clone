using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : CharacterBase
{

    [SerializeField] private DynamicJoystick dynamicJoystick;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float smoothJoystickSpeed = 0.7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform pickedCubesParent;
    [SerializeField] private PickObject  PickUpComponent;
    [SerializeField] private Vector3 pickedCubesParentRotaion;

    [SerializeField] private float desiredBlendSpeed = 0;
    private CharacterController characterController;
    private CapsuleCollider capsuleCollider;
    private void Awake()
    {
        isCubesSpwanedOnUpperPlane = false;
         
    }
    private void Start()
    {
      
        levelNumberForAWin = 1;
        characterController = GetComponent<CharacterController>();
        WinManager.OnWin += WinManager_OnWin;
    }

    private void WinManager_OnWin(CharacterBase character, Vector3 position)
    {
        CharacterBase characterBase =  (Player)character;
        characterBase.transform.position = position;
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (WinManager.Instance.IsGameOver)
        {
            return;
        }
       
        HandleMovemnt();
        
        ChechGround();     
    }
   
    public override void HandleMovemnt()
    {

        if (PickUpComponent.PickecObjects.Count <= 1 && isOnStairs && dynamicJoystick.Vertical > 0)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
           
            if (isOnStairs)
            {
                pickedCubesParent.eulerAngles = transform.localEulerAngles;
            }
            else
            {
                pickedCubesParent.eulerAngles = new Vector3(pickedCubesParentRotaion.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
            float blendSpeedNormalized = desiredPosition.magnitude / 1;

            desiredBlendSpeed = Mathf.Lerp(animator.GetFloat("BlendSpeed"), 1, blendSpeedNormalized);
            animator.SetFloat("BlendSpeed", desiredBlendSpeed);


        }
        if (Input.GetMouseButtonUp(0))
        {
            desiredBlendSpeed = 0;
            animator.SetFloat("BlendSpeed", desiredBlendSpeed);
            if (isOnStairs)
            {
                pickedCubesParent.eulerAngles = transform.localEulerAngles;
            }
            else
            {
                pickedCubesParent.eulerAngles = new Vector3(pickedCubesParentRotaion.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
          
        }
      

        float horizontal = dynamicJoystick.Horizontal* smoothJoystickSpeed;
        float vertical = dynamicJoystick.Vertical* smoothJoystickSpeed;
        desiredPosition = new Vector3(horizontal * moveSpeed * Time.deltaTime, 0, vertical * moveSpeed * Time.deltaTime);


        
        if (!isOnStairs)
        {
            desiredTurn = Vector3.forward * vertical + Vector3.right * horizontal;
        }
       
        if (desiredTurn != Vector3.zero && !isOnStairs)
        { 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredTurn), turnSpeed * Time.deltaTime);
        }


       

        velocityXZ = velocity;
        velocityXZ.y = 0;

        velocityXZ = Vector3.Lerp(velocityXZ,  desiredPosition, 10 * Time.deltaTime);
        velocity = new Vector3(velocityXZ.x, velocity.y, velocityXZ.z);
        
        characterController.Move(velocity);
       
    }
  
    public override void ChechGround()
    {
      
        Ray ray = new Ray(transform.position+Vector3.up*0.1f, Vector3.down);
        RaycastHit hit;
      
        if(Physics.Raycast(ray,out hit, rayDistance, groundLayer))
        {
            
            velocity.y = 0;
          
            isGrounded = true;
           
            Debug.DrawLine(ray.origin, ray.direction * 10, Color.blue);
            if (hit.collider.CompareTag("Slope"))
            {
                
                isOnStairs = true;
                if (dynamicJoystick.Vertical < 0)
                {
                    desiredPosition = RotateVector(hit.normal, -90);
                    Debug.DrawLine(ray.origin, desiredPosition * 10, Color.green);
                    ClimpLader(desiredPosition);
                }
                else if(dynamicJoystick.Vertical > 0)
                {
                    desiredPosition = RotateVector(hit.normal, 90);
                    Debug.DrawLine(ray.origin, desiredPosition * 10, Color.red);
                    ClimpLader(desiredPosition);
                    print("stop pressing");

                }
               
            }

            else
            {
                isOnStairs = false;
                velocity.y += gravityModifier * Time.deltaTime;
              
            }
          
        }
        else
        {
            if (!isOnStairs)
            {
                velocity.y += gravityModifier * Time.deltaTime;
                isGrounded = false;
            }


        }

        velocity.y = Mathf.Clamp(velocity.y, -10, 10);
    }
    private void OnCollisionEnter(Collision collision)
    {
        print("collisin");
    }
    private void ClampY()
    {
        Vector3 position = transform.position;
        position.y = Mathf.Clamp(position.y,0, 1);
        transform.position = position;
    }
    private Vector3 RotateVector(Vector3 direction,float angel)
    {
        return Quaternion.Euler(angel, 0, 0) * direction;
    }
    private void ClimpLader(Vector3 turnDirection)
    {
        velocity.y += gravityModifier * Time.deltaTime;
        transform.forward = turnDirection;
        desiredTurn = turnDirection;
    }
    public override void IncreaseNumber()
    {
       levelNumberForAWin++;
    }
    public override void SpawnCubes(Transform GroundPlane)
    {
        print("Tray on Upper Plane");
        if (!isCubesSpwanedOnUpperPlane && levelNumberForAWin<=2)
        {
            isCubesSpwanedOnUpperPlane = true;
            upperPlane = GroundPlane;
            SpawnSytem.Instance.OnDoorOpened(upperPlane);
            levelNumberForAWin++;
            print(levelNumberForAWin);
            print("spawnCubes on Upper Plane");
        }
        else if (levelNumberForAWin >= 2)
        {
            WinManager.Instance.DeclareWinner();
        }
    }

    public override void PlayAnimationsOnWin(bool isThisWiner)
    {
        if (isThisWiner)
        {
            animator.SetTrigger("Win");
        }
        else
        {
            animator.SetTrigger("Sad");
        }
    }
}
