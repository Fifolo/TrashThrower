using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    public void Move(Vector2 inputVector)
    {
        float sideInput = inputVector.x;
        float forwardInput = inputVector.y;
        Vector3 movementVector = transform.right * sideInput + transform.forward * forwardInput;

        if (isGrounded) characterController.Move(movementVector * movementSpeed * Time.deltaTime);
        else characterController.Move(movementVector * jumpControlSpeed * Time.deltaTime);
    }

    public void RotateCharacter(float xInput) => characterTransform.Rotate(Vector3.up * xInput * rotationSpeed * Time.deltaTime);
}
