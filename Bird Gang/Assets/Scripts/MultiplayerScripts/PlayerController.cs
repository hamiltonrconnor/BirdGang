using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    // [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    private float forwardSpeed = 25f, hoverSpeed = 5f;
    private float activeForwardSpeed, activeHoverSpeed;
    private float forwardAcceleration = 2.5f, hoverAcceleration = 2f;
    // private float mouseSensitivity = 50f;
    private float xRotation, yRotation;

    private float speed = 1.5f;
    private Quaternion rotationReset;

    float verticalLookRoation;    
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    
    Rigidbody rb;
    PhotonView PV;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        // Look();
        Turning();
        // Move();
        // Jump();
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        Movement();
        // rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }


    // void Look()
    // {
    //     transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
    //     verticalLookRoation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
    //     verticalLookRoation = Mathf.Clamp(verticalLookRoation, -90f, 90f);

    //     cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRoation;
    // }

    void Turning()
    {
        if (Input.GetKey(KeyCode.A)) 
        {
          transform.Rotate(Vector3.down * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) 
        {
          transform.Rotate(Vector3.up * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E)) 
        {
          transform.Rotate(Vector3.left * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q)) 
        {
          transform.Rotate(Vector3.right * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.R)) 
        {
          rotationReset = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
          transform.rotation = Quaternion.Slerp(transform.rotation, rotationReset, Time.deltaTime * speed);
        }
    }

    void Movement()
    {
      activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
      activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration * Time.deltaTime);
      rb.velocity = (transform.forward * activeForwardSpeed * Input.GetAxisRaw("Vertical")) + (transform.up * activeHoverSpeed * Input.GetAxisRaw("Hover"));
    }


    // void Move()
    // {
    //     Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

    //     moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

    //     // (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed)
    //     // Compact if statement saying "if left shift pressed down, use sprint speed, otherwise use walk speed.
    // }

    // void Jump()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space) && grounded)
    //     {
    //         rb.AddForce(transform.up * jumpForce);
    //     }
    // }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }


}
