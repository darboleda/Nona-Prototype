using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Zenject;

public class NonaController : MonoBehaviour
{
    [Inject]
    public INonaInput<string> Input { get; set; }

    public float JumpHeight = 2.5f;
    public float WalkSpeed = 4f;
    public float WalkAcceleration = 10f;
    public float WalkFriction = 10f;
    public float Gravity = 20f;

    public int MaxJumps;

    public Animator Animator;
    public AudioSource Audio;

    new private Transform transform;

    public NonaDeltaLimit[] floorChecks;
    public List<ActionBehavior> actions;

    [System.NonSerialized]
    public bool DisableInput;

    [System.NonSerialized]
    public bool InFocusMode;

    [System.NonSerialized]
    public Vector2 velocity;

    [System.NonSerialized]
    public float horizontalAxis;

    private NonaCollision nonaCollision;
    public bool onGround 
    {
        get { return this.nonaCollision.Grounded; }
    }

    private float gravityMultiplier = 1;
    public bool facingRight = true;

    private float jumpVelocityOnFire;
    private int currentJumps;

    public void Awake()
    {
        this.transform = base.transform;
        Animator.SetBool("Facing Right", true);
        facingRight = true;

        this.nonaCollision = new NonaCollision(new NonaInformation(this));
    }

    public void Update()
    {
        horizontalAxis = Input.GetAxis("Move Horizontal");
        horizontalAxis = (horizontalAxis > 0.5f ? 1 : horizontalAxis < -0.5f ? -1 : 0);

        foreach (var action in actions)
        {
            action.Tick();
        }

        bool crouching = Input.GetButton("Crouch") && onGround && !DisableInput;
        Animator.SetBool("Crouching", crouching);
        if (crouching) horizontalAxis = 0;

        if (InFocusMode)
        {
            horizontalAxis *= (onGround ? 0.8f : 1f);
        }
        
        if (onGround)
        {
            currentJumps = MaxJumps;
        }

        if (!crouching && Input.GetButtonPress("Jump") && !DisableInput && currentJumps > 0)
        {
            velocity.y = Mathf.Sqrt(2 * Gravity * JumpHeight);
            if(!onGround)
            {
                currentJumps--;
            }
        }

        if (velocity.y > 0 && !Input.GetButton("Jump"))
        {
            velocity.y = AddAndZeroSignChange(velocity.y, 2 * -Gravity * Time.deltaTime);
        }

        Animator.SetBool("Falling", velocity.y <= 0 && !onGround);
        Animator.SetBool("Jumping", velocity.y > 0);

        gravityMultiplier = 1;

        if (DisableInput)
        {
            horizontalAxis = 0;
        }
        if (horizontalAxis > 0 && !InFocusMode)
        {
            Animator.SetBool("Facing Right", true);
            facingRight = true;
        }
        else if (horizontalAxis < 0 && !InFocusMode)
        {
            Animator.SetBool("Facing Right", false);
            facingRight = false;
        }

        Animator.SetFloat("Horizontal Speed", horizontalAxis);

    }

    public void FixedUpdate()
    {
        float walkFriction = (velocity.x > 0.01f ? -WalkFriction : velocity.x < -0.01f ? WalkFriction : 0);
        float targetSpeed = horizontalAxis * WalkSpeed;
        float walkAcceleration = 0;
        if (targetSpeed > velocity.x)
        {
            walkAcceleration = -walkFriction + WalkAcceleration;
        }
        else if (targetSpeed < velocity.x)
        {
            walkAcceleration = -walkFriction - WalkAcceleration;
        }
        else if (targetSpeed != 0)
        {
            walkAcceleration = (-walkFriction);
        }

        float gravity = -Gravity * gravityMultiplier;
        if (onGround)
        {
            if (velocity.y < 0) velocity.y = 0;
            gravity = 0;
        }

        Vector2 finalAcceleration = new Vector2(walkAcceleration, gravity);
        Vector3 translationDelta = velocity * Time.deltaTime;

        translationDelta = CheckFloor(translationDelta);

        transform.Translate(translationDelta, Space.World);
        velocity += finalAcceleration * Time.deltaTime;
        velocity.x = AddAndZeroSignChange(velocity.x, walkFriction * Time.deltaTime);
        velocity.x = Mathf.Clamp(velocity.x, -WalkSpeed, WalkSpeed);
    }

    private Vector3 CheckFloor(Vector3 desiredDelta)
    {
        Vector2 updatedDelta = (Vector2)desiredDelta;
        NonaCollision updatedInfo = new NonaCollision(this.nonaCollision.Nona);

        foreach(NonaDeltaLimit floorCheck in floorChecks)
        {
            NonaDeltaLimit.ApplicationResult result = floorCheck.Apply(updatedDelta, this.nonaCollision, updatedInfo);
            updatedDelta = result.updatedDelta;
            updatedInfo = result.updatedInfo;
        }
        this.nonaCollision = updatedInfo;
        return (Vector3)updatedDelta;
    }

    private float AddAndZeroSignChange(float value, float add)
    {
        float sum = value + add;
        if (sum == 0) return 0;
        if (Mathf.Sign(sum) != Mathf.Sign(value)) return 0;
        if (Mathf.Abs(sum) < 0.01f) return 0;
        return sum;
    }
}
