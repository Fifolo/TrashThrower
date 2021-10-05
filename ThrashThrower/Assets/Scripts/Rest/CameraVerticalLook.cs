using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraVerticalLook : MonoBehaviour
{
    [SerializeField] private float maxCameraUpAngle = 40f;

    //[SerializeField] private float minAngle = -10f;
    //[SerializeField] private float maxAngle =  20f;
    [SerializeField] private Transform weaponHolder;
    private Transform cameraTransform;
    private float xRotation = 0f;

    private void Awake()
    {
        cameraTransform = transform;
        weaponHolder.SetParent(cameraTransform);
    }

    public void RotateUp(float yInput)
    {
        if (yInput == 0) return;
        xRotation -= yInput;
        xRotation = Mathf.Clamp(xRotation, -maxCameraUpAngle, maxCameraUpAngle);
        Quaternion rotation = Quaternion.Euler(xRotation, 0f, 0f);
        cameraTransform.localRotation = rotation;
    }
}
