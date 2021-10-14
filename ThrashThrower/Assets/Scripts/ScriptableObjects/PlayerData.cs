using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Health data")]
    [Range(100f,1000f)]
    public float baseHealth = 1000f;
    [Range(10f, 100f)]
    public float baseHealthRegenPerSecond = 50f;
    [Range(10f,1f)]
    public float autoRegenWaitTime = 5f;
   
    [Header("Movement data")]
    [Range(3f,15f)]
    public float movementSpeed = 5f;
    [Range(2f, 5f)]
    public float jumpSpeedControl = 3f;
    [Range(0.5f, 2f)]
    public float jumpHeight = 1f;
    [Range(20f, 150f)]
    public float rotationSpeed = 70f;
    [Range(9.81f, 100f)]
    public float gravityForce = 9.81f;

    [Header("Shooting data")]
    [Range(1f, 0.01f)]
    public float reloadTimeMultiplier = 2f;
    public Gun[] guns;
}
