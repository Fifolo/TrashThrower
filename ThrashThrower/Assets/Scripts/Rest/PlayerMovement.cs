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

    private float currentMoveSpeed = 0;
    public float MovementSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (value > 1 && value < 100)
            {
                currentMoveSpeed = value;
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
        currentMoveSpeed = playerData.movementSpeed;
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

        if (isGrounded) characterController.Move(movementVector * currentMoveSpeed * Time.deltaTime);
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
