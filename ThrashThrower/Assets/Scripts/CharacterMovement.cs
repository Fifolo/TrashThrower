using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CharacterMovement : MonoBehaviour
{
    #region Editor Exposed Fields
    [Range(1f, 30f)]
    [SerializeField] private float movementSpeed = 5f;
    [Range(1f, 10f)]
    [SerializeField] private float jumpControlSpeed = 2f;
    [Range(1f, 350f)]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = 15f;
    #endregion

    #region Instance Variables
    //jumping, gravity
    private Transform groundCheck;
    private LayerMask groundMask;
    private bool isGrounded;
    private float groundDistance = 0.15f;
    private Vector3 velocity;

    private CharacterController characterController;
    private Transform characterTransform;
    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set
        {
            if (value > 1 && value < 350)
            {
                rotationSpeed = value;
                Debug.Log($"Rotation speed for {name} succesfully changed");
            }
            else Debug.LogWarning($"Rotation speed not changed, {value} is not appropraite");
        }
    }
    public float MovementSpeed
    {
        get { return movementSpeed; }
        set
        {
            if (value > 1 && value < 100)
            {
                movementSpeed = value;
                Debug.Log($"Movement speed for {name} succesfully changed");
            }
            else Debug.LogWarning($"Movement speed not changed, {value} is not appropraite");
        }
    }
    #endregion

    #region Mono Behaviour
    private void Awake()
    {
        GettingReferences();
    }
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1.5f;
        }

        ApplyGravity();
    }
    #endregion

    #region Methods
    private void GettingReferences()
    {
        characterController = GetComponent<CharacterController>();
        characterTransform = transform;
        groundCheck = characterTransform.Find("GroundCheck");
        groundMask = LayerMask.GetMask("Ground");
    }
    public void Move(Vector2 inputVector)
    {
        float sideInput = inputVector.x;
        float forwardInput = inputVector.y;
        Vector3 movementVector = transform.right * sideInput + transform.forward * forwardInput;

        if(isGrounded) characterController.Move(movementVector * movementSpeed * Time.deltaTime);
        else characterController.Move(movementVector * jumpControlSpeed * Time.deltaTime);
    }

    public void RotateCharacter(float xInput) => characterTransform.Rotate(Vector3.up * xInput * rotationSpeed * Time.deltaTime);

    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    #endregion
}
