using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class CameraLook : MonoBehaviour
{
    [Range(30f, 90f)]
    [SerializeField] private float maxCameraUpAngle = 80f;

    private Transform cameraTransform;
    private float xRotation = 0f;

    private void Awake() => cameraTransform = transform;

    public void RotateUp(float yInput)
    {
        xRotation -= yInput;
        xRotation = Mathf.Clamp(xRotation, -maxCameraUpAngle, maxCameraUpAngle);
        Quaternion rotation = Quaternion.Euler(xRotation, 0f, 0f);
        cameraTransform.localRotation = rotation;
    }
}
