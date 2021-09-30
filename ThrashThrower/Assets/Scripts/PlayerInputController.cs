using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private CameraLook playerCamera;

    private PlayerActions playerActions;
    private CharacterMovement playerController;

    #region MonoBehaviour
    private void Awake()
    {
        playerActions = new PlayerActions();
        playerController = GetComponent<CharacterMovement>();
        if (playerCamera == null) Debug.LogError("camera not set");
        playerActions.InGame.Enable();

        InputSubscribing();
    }
    private void Update()
    {
        playerController.Move(GetMovementVector());
        playerController.RotateCharacter(GetLookVector().x);
        playerCamera.RotateUp(GetLookVector().y);
    }
    private void OnDisable()
    {
        playerActions.InGame.Disable();
        InputUnsubscribing();
    }
    #endregion

    #region InputSubscription
    private void InputSubscribing()
    {
        playerActions.InGame.Jump.performed += Jump_performed;
    }
    private void InputUnsubscribing()
    {
        playerActions.InGame.Jump.performed -= Jump_performed;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.Jump();
    }
    #endregion

    #region Methods
    private Vector2 GetLookVector() => playerActions.InGame.Look.ReadValue<Vector2>();
    private Vector2 GetMovementVector() => playerActions.InGame.Movement.ReadValue<Vector2>();
    #endregion
}
