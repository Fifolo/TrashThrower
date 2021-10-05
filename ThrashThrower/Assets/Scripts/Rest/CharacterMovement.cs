using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour, IDamagable
{
    #region Editor Exposed Fields
    [Range(1f, 30f)]
    [SerializeField] protected float movementSpeed = 5f;
    [Range(1f, 10f)]
    [SerializeField] protected float jumpControlSpeed = 2f;
    [Range(1f, 350f)]
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float jumpHeight = 3f;
    [SerializeField] protected float gravity = 15f;
    #endregion

    #region Instance Variables
    //jumping, gravity
    protected Transform groundCheck;
    protected LayerMask groundMask;
    protected bool isGrounded;
    protected float groundDistance = 0.15f;
    protected Vector3 velocity;

    protected CharacterController characterController;
    protected Transform characterTransform;
    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set
        {
            if (value > 1 && value < 350)
            {
                rotationSpeed = value;
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
                //Debug.Log($"Previous speed = {movementSpeed}, new speed = {value}");
                movementSpeed = value;
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
    public void Move(Vector3 direction)
    {
        Vector3 movementVector = transform.right * direction.x + transform.forward * direction.z;

        if(isGrounded) characterController.Move(movementVector * movementSpeed * Time.deltaTime);
        else characterController.Move(movementVector * jumpControlSpeed * Time.deltaTime);
    }

    public void RotateCharacter(Vector3 direction) => characterTransform.Rotate(Vector3.up * direction.x * rotationSpeed * Time.deltaTime);

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

    public void TakeDamage(float damage)
    {
        Debug.Log($"{name} just took {damage} damage");
    }
    #endregion
}
