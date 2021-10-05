using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private CameraVerticalLook playerCamera;

    private PlayerActions playerActions;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    #region MonoBehaviour
    private void Awake()
    {
        playerActions = new PlayerActions();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        if (playerCamera == null) Debug.LogError("camera not set");
        playerActions.InGame.Enable();

        InputSubscribing();
    }
    private void Update()
    {
        playerMovement.Move(GetMovementVector());
        playerMovement.RotateCharacter(GetLookVector().x);
        playerCamera.RotateUp(GetLookVector().y);
    }
    private void OnDestroy()
    {
        playerActions.InGame.Disable();
        InputUnsubscribing();
    }
    #endregion

    #region InputSubscription
    private void InputSubscribing()
    {
        playerActions.InGame.Jump.performed += Jump_performed;
        playerActions.InGame.Pause.performed += Pause_performed;
        playerActions.InGame.Reload.performed += Reload_performed;
        playerActions.InGame.SwapWeapon.performed += NextWeapon_performed;

        playerActions.InGame.Fire.started += Fire_started;
        playerActions.InGame.Fire.performed += Fire_performed;
        playerActions.InGame.Fire.canceled += Fire_canceled;
    }

    private void InputUnsubscribing()
    {
        playerActions.InGame.Jump.performed -= Jump_performed;
        playerActions.InGame.Pause.performed -= Pause_performed;
        playerActions.InGame.Reload.performed -= Reload_performed;
        playerActions.InGame.SwapWeapon.performed -= NextWeapon_performed;

        playerActions.InGame.Fire.started -= Fire_started;
        playerActions.InGame.Fire.performed -= Fire_performed;
        playerActions.InGame.Fire.canceled -= Fire_canceled;
    }

    #endregion

    #region Methods

    private void NextWeapon_performed(InputAction.CallbackContext obj) => playerShooting.SwapToNextWeapon();

    private void Reload_performed(InputAction.CallbackContext obj) => playerShooting.ReloadGun();

    private void Fire_performed(InputAction.CallbackContext obj) => playerShooting.StartFiring();

    private void Fire_canceled(InputAction.CallbackContext obj) => playerShooting.StopFiring();

    private void Fire_started(InputAction.CallbackContext obj) => playerShooting.FireOneTime();

    private void Pause_performed(InputAction.CallbackContext obj) => GameManager.Instance.PauseGame();

    private void Jump_performed(InputAction.CallbackContext obj) => playerMovement.Jump();

    private Vector2 GetLookVector() => playerActions.InGame.Look.ReadValue<Vector2>();

    private Vector2 GetMovementVector() => playerActions.InGame.Movement.ReadValue<Vector2>();
    #endregion
}
