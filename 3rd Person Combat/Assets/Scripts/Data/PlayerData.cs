using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player/Base Player Data")]
public class PlayerData : ScriptableObject
{

    [Header("Grounded State")]
    public float groundCheckRadius;
    public float groundCheckDistance;
    public LayerMask whatIsGround;

    [Header("Gravity")]
    public float gravity = -10f;
    public float downwardForce = -3f;

    [Header("Feet IK")]
    [Range(0f, 2f)] public float heightFromGroundRaycast = 1.14f;
    [Range(0f, 2f)] public float raycastDownDistance = 1.5f;
    public float pelvisOffset = 0f;
    [Range(0f, 1f)] public float pelvisUpAndDownSpeed = 0.28f;
    [Range(0f, 1f)] public float feetToIKPositionSpeed = 0.5f;
    public LayerMask environmentLayer;

    [Header("Move State")]
    public float walkingSpeed = 4f;
    public float movementSpeed = 5f;
    public float sprintSpeed = 7f;
    public float movementRotation = 15f;

    [Header("Jump State")]
    public float maxJumpHeight = 5f;
    public float inAirMoveSpeed = 5f;

    [Header("Dodge State")]
    public float slideSpeed = 5f;
    public float dodgeTime = 1.5f;

    [Header("Aim State")]
    public float aimRotationSpeed = 10f;
    public float aimWalkingSpeed = 3f;

    [Header("Reload State")]
    public float reloadRotationSpeed = 20f;
    public float reloadWalkingSpeed = 3.5f;

    [Header("Melee Attack State")]
    public float timeToReadAttackInput = 0.2f;
    public float forwardAttackMoveSpeed = 2f;

    [Header("Gun Drop")]
    public float gunCheckRadius = 3f;
    public LayerMask gunDropLayerMask;

}
