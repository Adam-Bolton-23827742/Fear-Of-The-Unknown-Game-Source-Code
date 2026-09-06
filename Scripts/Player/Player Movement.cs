using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody RB;
    [HideInInspector] public Vector2 Move;
    [HideInInspector] public Vector3 MoveDirection;
    private Vector3 PreviousForward;
    private Vector3 CamForward, CamRight;
    [SerializeField] private float MoveSpeed, TurnSpeed, EnemyDetectionRadius, ForwardAngle;
    private Animator Animator;
    [SerializeField] private GameObject[] RightHand;
    private PlayerStats PlayerStats;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();
        PlayerStats = GetComponent<PlayerStats>();

        // Stores the current camera's forward directions
        CamForward = Camera.main.transform.forward;
        CamRight = Camera.main.transform.right;

        // Sets the current camera's forward Y directions to 0 as the player does not need to move upwards/downwards
        CamForward.y = 0;
        CamRight.y = 0;
        PreviousForward = transform.forward;
    }

    private void FixedUpdate()
    {
        // if the players forward direction changes by a certain angle or they stop moving
        if (Vector3.Angle(transform.forward, PreviousForward) > ForwardAngle || MoveDirection == Vector3.zero)
        {
            // Stores the current camera's forward directions
            CamForward = Camera.main.transform.forward;
            CamRight = Camera.main.transform.right;

            // Sets the current camera's forward Y directions to 0 as the player does not need to move upwards/downwards
            CamForward.y = 0;
            CamRight.y = 0;

            CamForward.Normalize();
            CamRight.Normalize();

            PreviousForward = transform.forward;
        }

        // Sets the direction the player needs to move in relative to the current camera
        Vector3 ForwardLook = Move.y * CamForward;
        Vector3 HorizontalLook = Move.x * CamRight;
        MoveDirection = ForwardLook + HorizontalLook;
        MoveDirection.Normalize();

        // Set the player look direction to the direction they are moving
        transform.forward = Vector3.Slerp(transform.forward, MoveDirection, TurnSpeed * Time.fixedDeltaTime);

        RB.linearVelocity = new Vector3(MoveDirection.x * MoveSpeed * Time.fixedDeltaTime, RB.linearVelocity.y, MoveDirection.z * MoveSpeed * Time.fixedDeltaTime);

        // Moves the player by setting their velocity at a given speed when they are not aiming down sights
        if (Input.GetAxis("Aim") > 0 || Input.GetButton("Aim"))
        {
            foreach (GameObject Item in RightHand)
            {
                if (Item.gameObject.activeSelf)
                {
                    RB.linearVelocity = Vector3.zero;
                    break;
                }
            }
        }

        if (RB.linearVelocity == new Vector3(0f, RB.linearVelocity.y, 0f))
        {
            foreach (GameObject Item in RightHand)
            {
                if (Item.gameObject.activeSelf && Item.name == "Shotgun" && !PlayerStats.BeingHit)
                {
                    Animator.SetInteger("State", 6);
                    break;
                }

                else
                {
                    if (!PlayerStats.BeingHit)
                    {
                        Animator.SetInteger("State", 1);
                    }
                }
            }
            return;
        }

        else
        {
            if (!PlayerStats.BeingHit)
            {
                Animator.SetInteger("State", 2);
            }

            foreach (GameObject Item in RightHand)
            {
                if (Item.gameObject.activeSelf && Item.name == "Shotgun" && !PlayerStats.BeingHit)
                {
                    Animator.SetInteger("State", 4);
                    break;
                }
            }
        }
    }

    private void OnMove(InputValue Value)
    {
        // If aiming and a gun is active them stop moving
        if (Input.GetAxis("Aim") > 0 || Input.GetButton("Aim"))
        {
            foreach (GameObject Item in RightHand)
            {
                if (Item.gameObject.activeSelf)
                {
                    Move = Vector2.zero;
                    return;
                }
            }
        }
        
        // Stores the value for which of the WASD keys are being pressed
        Move = Value.Get<Vector2>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, EnemyDetectionRadius);
    }
}
