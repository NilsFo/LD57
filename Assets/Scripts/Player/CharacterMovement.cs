using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class CharacterMovement : MonoBehaviour
{
    [Header("Meta")] public GameState gameState;
    public NodeGraph.NodeGraph nodeGraph;

    [Header("Movement Parameters")] public float maximumSpeed = 6f;
    public float sprintSpeedModifier = 1.5f;
    public float crouchSpeedModifier = 0.5f;
    public float acceleration = 50f;
    public float dampening = 30f;
    public float terminalYVelocity = 55f;
    public float gravity = 20f;
    public float jumpSpeed = 10f;
    public float airControlMultiplier = 0.1f;
    public float coyoteTime = 0.1f;
    private float _jumpCoyoteTimer;

    [Header("Gamepad Config")] public bool useGamepadOverKBM = false;

    [Header("Movement Config")] public bool sprintEnabled = true;
    public bool crouchEnabled = true;
    public bool jumpEnabled = true;
    public bool inputDisabled = false;
    public bool tethered = true;

    [Header("Physics Interaction")] public bool interactWithRigidbodies = false;
    public float pushForceMultiplier = 0.2f;
    public float pushForceMax = 1f;
    public float characterMass = 80f;

    [Header("Player Collision")] public float playerHeightStanding = 1.8f;
    public float playerHeightCrouching = 1.1f;
    public bool crouching;
    public bool sprinting;
    private float _eyesToHeadDist;

    [Header("Game Hookup")] private CharacterController _controller;
    private Camera _camera;

    [Header("Readonly")] public Vector3 velocity;
    private Vector3 _acc;
    private bool _isOnLadder;
    private Vector3 _ladderNormal;
    private int _myLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3();
        _controller = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _myLayerMask = GetPhysicsLayerMask(gameObject.layer);
        gameState = FindFirstObjectByType<GameState>();
        nodeGraph = gameState.GetComponent<NodeGraph.NodeGraph>();

        if (_camera)
            _eyesToHeadDist = (playerHeightStanding - _camera.transform.localPosition.y * 2);
    }

    private void FixedUpdate()
    {
        velocity.x = _controller.velocity.x;
        velocity.z = _controller.velocity.z;
        DampenXZ();
        
        Vector3 groundNormal = GetNormalBelow();

        var velocityXZ = Vector3.ProjectOnPlane(velocity, Vector3.up);

        bool isGrounded = _controller.isGrounded &&
                          Vector3.Angle(groundNormal, Vector3.up) < _controller.slopeLimit;
        // Coyote Time
        if (isGrounded)
            _jumpCoyoteTimer = 0;
        _jumpCoyoteTimer += Time.deltaTime;

        if (_isOnLadder)
        {
            // LadderUpdate();
            // Skip the rest of the movement
            return;
        }

        if (!isGrounded)
        {
            if (velocity.y > 0)
            {
                velocity.y = Mathf.Min(velocity.y, terminalYVelocity);
            }
        }

        float x = 0, z = 0;
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed)
            {
                x -= 1;
            }

            if (keyboard.dKey.isPressed)
            {
                x += 1;
            }

            if (keyboard.wKey.isPressed)
            {
                z += 1;
            }

            if (keyboard.sKey.isPressed)
            {
                z -= 1;
            }
        }

        if (gamepad != null && useGamepadOverKBM)
        {
            StickControl stick = gamepad.leftStick;
            if (stick != null)
            {
                var stickX = stick.x.ReadValue();
                var stickY = stick.y.ReadValue();
                x = stickX;
                z = stickY;
            }
        }

        // Movement
        _acc.x = 0;
        _acc.y = 0;
        _acc.z = 0;
        bool hasJumped = false;
        sprinting = false;
        bool sprint = false;

        if (!inputDisabled && (keyboard != null || (gamepad != null && useGamepadOverKBM)))
        {
            // ############################
            //   MOVEMENT
            // ############################

            bool sprintPressed = false;
            if (sprintEnabled)
            {
                if (useGamepadOverKBM)
                {
                    sprintPressed = gamepad.buttonEast.isPressed;
                }
                else
                {
                    sprintPressed = keyboard.shiftKey.isPressed;
                }

                sprint = sprintPressed;
            }


            if (isGrounded)
            {
                //DampenXZ();

                _acc.x = x;
                _acc.z = z;

                _acc.Normalize();
                _acc *= acceleration;
            }
            else
            {
                // Air control
                var localVelocity = transform.worldToLocalMatrix.rotation * velocity;
                if ((localVelocity.x * x + localVelocity.z * z) /
                    (Mathf.Sqrt(localVelocity.x * localVelocity.x + localVelocity.z * localVelocity.z) *
                     Mathf.Sqrt(x * x + z * z)) < -0.5f)
                {
                    // dot product
                    // Stop on backwards
                    //DampenXZ();
                }

                _acc.x = x;
                _acc.z = z;

                _acc.Normalize();
                if (velocityXZ.magnitude < 0.25f)
                {
                    // air control when very slow (for jumping up ledges)
                    _acc *= acceleration;
                }
                else
                {
                    _acc *= acceleration * airControlMultiplier;
                }
            }

            // ############################
            //   JUMPING
            // ############################
            bool jumpPressed = false;
            if (jumpEnabled)
            {
                if (useGamepadOverKBM)
                {
                    jumpPressed = gamepad.buttonSouth.isPressed;
                }
                else
                {
                    jumpPressed = keyboard.spaceKey.wasPressedThisFrame;
                }
            }

            if (jumpPressed && !crouching)
            {
                if (isGrounded || _jumpCoyoteTimer <= coyoteTime)
                {
                    /*if (!isGrounded && _jumpCoyoteTimer <= coyoteTime)
                    Debug.Log("Coyote Jump!");
                else {
                    Debug.Log("Normal Jump");
                }*/
                    velocity.y = jumpSpeed;
                    if (z > 0.5 && sprint)
                    {
                        Vector3 sprintJump = new Vector3(x, 0, z).normalized;
                        sprintJump *= maximumSpeed * sprintSpeedModifier;

                        velocity += transform.rotation * sprintJump;
                    }

                    hasJumped = true;
                }
                else
                {
                    // We are in the air
                }
            }

            // ############################
            //   CROUCHING
            // ############################
            bool crouchPressed = false;
            if (crouchEnabled)
            {
                if (useGamepadOverKBM)
                {
                    crouchPressed = gamepad.buttonNorth.isPressed;
                }
                else
                {
                    crouchPressed = keyboard.leftCtrlKey.wasPressedThisFrame;
                }
            }

            // Crouching
            if (crouchPressed && !crouching)
            {
                crouching = true;
                _controller.height = playerHeightCrouching;
                _controller.center.Set(0, playerHeightCrouching / 2f, 0);
                _camera.transform.localPosition = new Vector3(0, (playerHeightCrouching - _eyesToHeadDist) / 2, 0);
            }
            else if (((crouchPressed && crouchEnabled) ||
                      (jumpPressed && jumpEnabled) ||
                      (sprintPressed && sprintEnabled)) && crouching)
            {
                bool couldStandUp = StandUp();
            }
        }

        if (inputDisabled && isGrounded)
        {
            //DampenXZ();
        }

        // local acceleration to world velocity
        _acc = transform.rotation * _acc;
        velocity += _acc * Time.deltaTime;
        if (isGrounded && !hasJumped)
        {
            velocity.y = 0;
        }

        // Handle max speed
        float speedSqrd = velocity.x * velocity.x + velocity.z * velocity.z;
        float maxSpeed;
        if (crouching)
            maxSpeed = maximumSpeed * crouchSpeedModifier;
        else if (sprint && !hasJumped)
            maxSpeed = maximumSpeed * sprintSpeedModifier;
        else if (sprint)
            maxSpeed = maximumSpeed * sprintSpeedModifier;
        else
            maxSpeed = maximumSpeed;
        sprinting = sprint;
        float maxSpeedSqrd = maxSpeed * maxSpeed;
        if (speedSqrd > maxSpeedSqrd && (isGrounded || hasJumped))
        {
            velocity.x *= Mathf.Sqrt(maxSpeedSqrd / speedSqrd);
            velocity.z *= Mathf.Sqrt(maxSpeedSqrd / speedSqrd);
        }

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;
    }

    private void Update()
    {
        // Move
        var moveDistance = velocity * Time.deltaTime;
        CollisionFlags flags = _controller.Move(moveDistance);

        // Leash
        Vector3 nearestPoint = nodeGraph.FindNearestPoint(transform.position);

        Vector3 distance = transform.position - nearestPoint;
        if (distance.magnitude > nodeGraph.GetLeashDistance() && tethered)
        {
            Vector3 moveCorrection = nodeGraph.FindNearestBorderPoint(transform.position);
            Vector3 diff = moveCorrection - transform.position;
            flags |= _controller.Move(diff);
        }

        if ((flags & CollisionFlags.Above) != 0)
        {
            // head bump :(
            velocity.y = Mathf.Min(0, velocity.y);
        }
    }
    
    private Vector3 GetNormalBelow()
    {
        RaycastHit hit;
        int layerMask = _myLayerMask;
        Vector3 result = Vector3.zero;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 3f, layerMask))
        {
            result = hit.normal;
        }

        return result;
    }

    public static LayerMask GetPhysicsLayerMask(int currentLayer)
    {
        int finalMask = 0;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(currentLayer, i)) finalMask = finalMask | (1 << i);
        }

        return finalMask;
    }

    private bool StandUp()
    {
        // Raycast to check if we can stand up
        int terrainLayerMask = LayerMask.GetMask(new String[]
        {
            "Default"
        });
        bool hit = Physics.CheckCapsule(transform.position + new Vector3(0, playerHeightCrouching, 0),
            transform.position + new Vector3(0, playerHeightStanding, 0), _controller.radius, terrainLayerMask);
        if (!hit)
        {
            crouching = false;
            _controller.height = playerHeightStanding;
            _controller.center.Set(0, playerHeightStanding / 2f, 0);
            _camera.transform.localPosition = new Vector3(0, (playerHeightStanding - _eyesToHeadDist) / 2, 0);
        }

        return !hit;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (interactWithRigidbodies && hit.rigidbody?.isKinematic == false)
        {
            KickObject(hit.point, hit.normal, _controller.velocity, hit.rigidbody);
        }
    }

    public void KickObject(Vector3 point, Vector3 normal, Vector3 velocity, Rigidbody rb)
    {
        //Debug.Log(hit.collider.gameObject.name);
        Vector3 pushVec = pushForceMultiplier * velocity;
        pushVec *= Vector3.Dot(velocity.normalized, -normal);
        pushVec.y = 0.1f;
        Vector3 pushPos = new Vector3(point.x, rb.position.y, point.z);
        Vector3 impulse = Vector3.ClampMagnitude(pushVec, pushForceMax);
        rb.AddForceAtPosition(impulse, pushPos, ForceMode.Impulse);
        //Debug.Log("Hitting for " + impulse + ", " + pushPos);
        Debug.DrawRay(pushPos, impulse, Color.red, 5f);
        /*velocity -= Vector3.ProjectOnPlane(velocity, normal) *
        Mathf.Min(pushForceMultiplier * rb.mass / characterMass, 1f);*/
    }


    private void DampenXZ()
    {
        Vector3 vXY = new Vector3(velocity.x, 0, velocity.z);
        if (vXY.magnitude == 0)
            return;
        var damp = Vector3.MoveTowards(vXY, Vector3.zero, dampening*vXY.magnitude);
        /*if (damp.magnitude > vXY.magnitude)
        {
            vXY.x = 0;
            vXY.z = 0;
        }
        else
        {
            vXY += damp;
        }*/

        velocity.x = damp.x;
        velocity.z = damp.z;
    }

    public void SetUseGamepadOverKbm(bool newValue)
    {
        useGamepadOverKBM = newValue;
    }
}