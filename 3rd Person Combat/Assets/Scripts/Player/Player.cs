using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour
{
    #region State Variables

    //Creating State Machine
    public PlayerStateMachine StateMachine { get; private set; }

    //Creating States
    public PlayerLocomotionState LocomotionState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerDodgeState DodgeState { get; private set; }
    public PlayerAimState AimState { get; private set; }
    public PlayerMeleeAttackState MeleeAttackState { get; private set; }
    public PlayerReloadState ReloadState { get; private set; }

    #endregion

    #region Components Variables
    [Header("Debug Animation Bool Name")]
    public bool DebugBool;
    public Animator animator { get; private set; }
    public CharacterController playerController { get; private set; }
    public PlayerInputManager InputManager { get; private set; }
    public PlayerEquipmentManager EquipmentManager { get; private set; }

    public PlayerCamera playerCamera;
    public PlayerUIManager UIManager { get; private set; }

    #endregion

    #region Camera References

    [Header("Camera Ref")]
    public CinemachineVirtualCamera playerFollowCamera;
    public CinemachineVirtualCamera playerAimCamera;

    #endregion

    #region Feet IK Variables

    private Vector3 rightFootPosition, rightFootIKPosition, leftFootPosition, leftFootIKPosition;
    private Quaternion leftFootIKRotation, rightFootIKRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    [Header("Feet IK Placement")]
    [SerializeField] bool enableFeetIK = true;
    [SerializeField] string leftFootAnimVariableName = "LeftFootCurve";
    [SerializeField] string rightFootAnimVariableName = "RightFootCurve";
    [SerializeField] bool useProIKFeature = false;
    [SerializeField] bool showSolverDebug = true;
    [SerializeField] Transform leftFoot;
    [SerializeField] Transform rightFoot;

    #endregion

    #region Aim IK Variables
    [Header("Aim IK")]
    public MultiAimConstraint headAim;
    public MultiAimConstraint spineIK;
    public MultiAimConstraint chestIK;

    [HideInInspector] public RigBuilder rigBuilder;

    #endregion

    #region Other Variables
    [Header("References")]
    [SerializeField] PlayerData playerData;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform gunCheck;

    public Vector3 currentVelocity { get; private set; }
    private Vector3 playerVelocity;

    public Transform bulletPoolHolder;

    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        LocomotionState = new PlayerLocomotionState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        DodgeState = new PlayerDodgeState(this, StateMachine, playerData, "dodge");
        AimState = new PlayerAimState(this, StateMachine, playerData, "aim");
        MeleeAttackState = new PlayerMeleeAttackState(this, StateMachine, playerData, "meleeAttack");
        ReloadState = new PlayerReloadState(this, StateMachine, playerData, "reload");
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController>();
        InputManager = GetComponent<PlayerInputManager>();
        rigBuilder = GetComponent<RigBuilder>();
        EquipmentManager = GetComponent<PlayerEquipmentManager>();
        UIManager = GetComponent<PlayerUIManager>();

        playerCamera.SetInputManager(InputManager);

        StateMachine.InitializeState(LocomotionState);

        StateMachine.CurrentState.Enter();

        SetIKWeights(0);

    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        currentVelocity = playerController.velocity;
        HandleGravity();
    }

    private void FixedUpdate()
    {
        FixedUpdateFeetIK();
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void LateUpdate()
    {
        PlayerCamera.instance.HandleAllCameraActions();
    }

    #endregion

    #region Set Funtions

    public void HandleGravity()
    {
        playerVelocity.y = playerVelocity.y + playerData.gravity * Time.deltaTime;
        if (isGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = playerData.downwardForce;
        }
        playerController.Move(playerVelocity * Time.deltaTime);

    }

    public void SetMovement(float movementSpeed)
    {
        Vector3 moveDirection;
        moveDirection = PlayerCamera.instance.transform.forward * InputManager.MovementInput.y;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * InputManager.MovementInput.x;
        moveDirection.Normalize();
        moveDirection = moveDirection * movementSpeed;

        Vector3 movementVelocity = moveDirection;
        playerController.Move(movementVelocity * Time.deltaTime);
    }

    public void SetRotation(float rotationSpeed)
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = PlayerCamera.instance.cameraObject.transform.forward * InputManager.MovementInput.y;
        targetDirection = targetDirection + PlayerCamera.instance.cameraObject.transform.right * InputManager.MovementInput.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    public void SetAimRotation(float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    public void SetJump(float JumpVelocity)
    {
        playerVelocity.y = Mathf.Sqrt(JumpVelocity * -4.5f * playerData.gravity);
    }

    public void SetSlide(float slideSpeed)
    {
        playerController.Move(transform.forward * slideSpeed * Time.deltaTime);
    }

    public void SetAimIK(Vector3 spineOffset, Vector3 chestOffset, Vector3 headOffset)
    {
        spineIK.data.offset = spineOffset;
        chestIK.data.offset = chestOffset;
        headAim.data.offset = headOffset;
        rigBuilder.Build();
    }

    public void SetIKWeights(float Weight)
    {
        //Aim IK
        headAim.weight = Weight;
        spineIK.weight = Weight;
        chestIK.weight = Weight;
    }

    private void isAnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void isAnimationTriggerFinished() => StateMachine.CurrentState.AnimationFinishTrigger();

    #endregion

    #region DoCheck Functions

    public bool isGrounded()
    {
        Collider[] hitColliders = new Collider[10];
        int numColliders = Physics.OverlapSphereNonAlloc(groundCheck.position, playerData.groundCheckRadius, hitColliders, playerData.whatIsGround);

        foreach (Collider collider in hitColliders)
        {
            if(collider != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (playerController.isGrounded)
        {
            return true;
        }

        return false;
    }


    public WeaponItem CheckGunDrops()
    {
        Collider[] hitColliders = new Collider[10];

        Physics.OverlapSphereNonAlloc(gunCheck.transform.position, playerData.gunCheckRadius, hitColliders, playerData.gunDropLayerMask);

        foreach (Collider collider in hitColliders)
        {
            if(collider == null)
            {
                UIManager.pickUpWeaponIcon.sprite = UIManager.DefaultSprite;
                UIManager.pickUpWeaponText.text = "";
                return null;
            }

            if (collider.TryGetComponent<WeaponPickUpItem>(out WeaponPickUpItem weaponPickUpItem))
            {
                if(weaponPickUpItem != null)
                {
                    return weaponPickUpItem.weapon;
                }
            }
            else
            {
                return null;
            }
        }

        return null;
    }
    
    #endregion

    #region FeetIK

    private void FixedUpdateFeetIK()
    {
        if(enableFeetIK == false || animator == null) return;

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        //Find and Raycast the ground to find positions
        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableFeetIK == false || animator == null) return;

        MovePelvisHeight();

        // Right Foot IK position and rotation
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        if (useProIKFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat(rightFootAnimVariableName));
        }

        MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIKRotation, ref lastRightFootPositionY);

        // Left Foot IK position and rotation
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        if (useProIKFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat(leftFootAnimVariableName));
        }

        MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
    }

    private void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    {
        Vector3 targetIKPosition = animator.GetIKPosition(foot);

        if(positionIKHolder != Vector3.zero)
        {
            targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, playerData.feetToIKPositionSpeed);
            targetIKPosition.y += yVariable;

            lastFootPositionY = yVariable;

            targetIKPosition = transform.TransformPoint(targetIKPosition);
            animator.SetIKRotation(foot, rotationIKHolder);
        }
        animator.SetIKPosition(foot, targetIKPosition);
    }

    private void MovePelvisHeight()
    {
        if(rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = animator.bodyPosition.y;
            return;
        }

        float L_OffsetPosition = leftFootIKPosition.y - transform.position.y;
        float R_OffsetPosition = rightFootIKPosition.y - transform.position.y;

        float totalOffset = (L_OffsetPosition < R_OffsetPosition) ? L_OffsetPosition : R_OffsetPosition;

        Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * totalOffset;

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, playerData.pelvisUpAndDownSpeed);

        animator.bodyPosition = newPelvisPosition;

        lastPelvisPositionY = animator.bodyPosition.y;
    }

    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPosition, ref Quaternion feetIKRotation)
    {
        RaycastHit feetOutHit;

        if (showSolverDebug)
        {
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (playerData.raycastDownDistance + playerData.heightFromGroundRaycast), Color.yellow);
        }

        if(Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, playerData.raycastDownDistance + playerData.heightFromGroundRaycast, playerData.environmentLayer))
        {
            //Finding feet IK positions from ground check position
            feetIKPosition = fromSkyPosition;
            feetIKPosition.y = feetOutHit.point.y + playerData.pelvisOffset;
            feetIKRotation = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }
        feetIKPosition = Vector3.zero;

    }

    private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + playerData.heightFromGroundRaycast;
    }

    #endregion

    #region Gizmos Functions

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, playerData.groundCheckRadius);
        Gizmos.DrawWireSphere(gunCheck.position, playerData.gunCheckRadius);
    }

    #endregion

}
