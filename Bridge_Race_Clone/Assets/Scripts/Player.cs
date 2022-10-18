using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private DynamicJoystick dynamicJoystick;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float smoothJoystickSpeed = 0.7f;
    [SerializeField] private float rayDistance = 0.15f;
    [SerializeField] private float gravityModifier = -9.81f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform pickedCubesParent;
    [SerializeField] private Vector3 pickedCubesParentRotaion;

    private Vector3 inputVector;
    private Vector3 desiredPosition;
    private Vector3 desiredTurn;
    private Vector3 velocity;
    private Vector3 velocityXZ;
    private Vector3 lookDirection;

    private float desiredBlendSpeed = 0;
   
    private bool isPressing = false;
    private bool isGrounded = false;
 
    [SerializeField]private bool isOnSlope = false;

    private CharacterController characterController;

 
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.P))
        {
            print(PickObject.Instance.PickecObjects.Count);
        }
        Process›nput();
        HandleMovemnt();
        ChechGround();
        // Makes the reflected object appear opposite of the original object,
        // mirrored along the z-axis of the world
     
    }
    private void Process›nput()
    {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector = Vector3.ClampMagnitude(inputVector, 1);
    }
    private void HandleMovemnt()
    {
        if (PickObject.Instance.PickecObjects.Count <= 1 && isOnSlope && dynamicJoystick.Vertical > 0)
        {
          
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            isPressing = true;
        }
        if (Input.GetMouseButton(0))
        {
            if (isOnSlope)
            {
                pickedCubesParent.eulerAngles = transform.localEulerAngles;
            }
            else
            {
                pickedCubesParent.eulerAngles = new Vector3(pickedCubesParentRotaion.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
           
        }
        if (Input.GetMouseButtonUp(0))
        {
            desiredBlendSpeed = 0;
            animator.SetFloat("BlendSpeed", desiredBlendSpeed);
            if (isOnSlope)
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


        //transform.position += desiredPosition;
        if (!isOnSlope)
        {
            desiredTurn = Vector3.forward * vertical + Vector3.right * horizontal;
        }
       
        if (desiredTurn != Vector3.zero && !isOnSlope)
        { 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredTurn), turnSpeed * Time.deltaTime);
        }

        float blendSpeedNormalized = desiredPosition.magnitude / 1;
        desiredBlendSpeed = Mathf.Lerp(animator.GetFloat("BlendSpeed"), 1, blendSpeedNormalized);
        animator.SetFloat("BlendSpeed", desiredBlendSpeed);



        velocityXZ = velocity;
        velocityXZ.y = 0;

        velocityXZ = Vector3.Lerp(velocityXZ,  desiredPosition, 10 * Time.deltaTime);
        velocity = new Vector3(velocityXZ.x, velocity.y, velocityXZ.z);
     
        characterController.Move(velocity);
       
    }
    private void ChechGround()
    {
     
   
        Ray ray = new Ray(transform.position+Vector3.up*0.1f, Vector3.down);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, rayDistance, groundLayer))
        {
            
            velocity.y = 0;
          
            isGrounded = true;
        
            
            if (hit.collider.CompareTag("Slope"))
            {
                
                isOnSlope = true;
                if (dynamicJoystick.Vertical < 0)
                {
                    desiredPosition = RotateVector(hit.normal, -90);
                    ClimpLader(desiredPosition);
                }
                else if(dynamicJoystick.Vertical > 0)
                {
                    desiredPosition = RotateVector(hit.normal, 90);
                    ClimpLader(desiredPosition);
                }
            }

            else
            {
                isOnSlope = false;
                velocity.y += gravityModifier * Time.deltaTime;
            }
           
        }
        else
        {
            if (!isOnSlope)
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
}
