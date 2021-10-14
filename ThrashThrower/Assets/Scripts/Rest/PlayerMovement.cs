using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : Singleton<PlayerMovement>
{
    #region Editor Exposed Fields
    [SerializeField] private PlayerData playerData;
    #endregion

    #region Instance Variables
    public PlayerData PlayerData { get { return playerData; } }
    protected Transform groundCheck;
    protected LayerMask groundMask;
    protected bool isGrounded;
    protected float groundDistance = 0.15f;
    protected Vector3 velocity;

    protected CharacterController characterController;
    protected Transform characterTransform;
    public float RotationSpeed
    {
        get { return playerData.rotationSpeed; }
        set
        {
            if (value > 1 && value < 350)
            {
                playerData.rotationSpeed = value;
            }
            else Debug.LogWarning($"Rotation speed not changed, {value} is not appropraite");
        }
    }
    public float MovementSpeed
    {
        get { return playerData.movementSpeed; }
        set
        {
            if (value > 1 && value < 100)
            {
                //Debug.Log($"Previous speed = {movementSpeed}, new speed = {value}");
                playerData.movementSpeed = value;
            }
            else Debug.LogWarning($"Movement speed not changed, {value} is not appropraite");
        }
    }
    #endregion

    #region Mono Behaviour
    protected override void Awake()
    {
        base.Awake();
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

        if (isGrounded) characterController.Move(movementVector * playerData.movementSpeed * Time.deltaTime);
        else characterController.Move(movementVector * playerData.jumpSpeedControl * Time.deltaTime);
    }
    public void RotateCharacter(float xInput) => characterTransform.Rotate(Vector3.up * xInput * playerData.rotationSpeed * Time.deltaTime);
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(playerData.jumpHeight * 2f * playerData.gravityForce);
        }
    }

    private void ApplyGravity()
    {
        velocity.y -= playerData.gravityForce * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    #endregion
}
