using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCameraData", menuName = "Data/Player/Camera Data")]
public class PlayerCameraData : ScriptableObject
{
    [Header("Camera Settings")]
    public float defaultHorizontalRotationSpeed = 220;
    public float defaultVerticalRotationSpeed = 220;
    public float minimumPivot = -30;
    public float maximumPivot = 60;
    public float cameraCollisionsRadius = 0.2f;
    public LayerMask collideWithLayers;

    [Header("Mouse Values")]
    public float mouseHorizontalRotationSpeed = 30;
    public float mouseVerticalRotationSpeed = 20;

    [Header("Gamepad Values")]
    public float gamepadHorizontalRotationSpeed = 220;
    public float gamepadVerticalRotationSpeed = 220;
}
