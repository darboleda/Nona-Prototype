using UnityEngine;
using System.Collections;

using Zenject;

public class NonaController : MonoBehaviour
{
    [Inject]
    public INonaInput<string> Input { get; set; }

    [Inject]
    public BulletMeter BulletDisplay { get; set; }

    public float JumpHeight = 2.5f;
    public float WalkSpeed = 4f;
    public float WalkAcceleration = 10f;
    public float WalkFriction = 10f;
    public float Gravity = 20f;

    public int MaxJumps;

    public LayerMask GunShotLayer;

    public Animator Animator;
    public AudioSource Audio;

    public AudioClip GunSound;
    public AudioClip ReloadSound;
    public AudioClip EmptyGunSound;
    public GameObject BulletTrail;

    public Transform LeftGunShotOrigin;
    public Transform RightGunShotOrigin;
    public float ShotCooldown = 0.1f;
    public float GunSlowdownTime = 0.5f;
    public float ReloadTime = 0.5f;

    new private Transform transform;

    public NonaDeltaLimit[] floorChecks;

    [HideInInspector]
    public Vector2 velocity;
    private float horizontalAxis;

    private NonaCollision nonaCollision;
    private bool onGround 
    {
        get { return this.nonaCollision.Grounded; }
    }

    private float cooldownTimer = 0;
    private float gunSlowdownTimer = 0;
    private float reloadTimer = 0;
    private bool lastFiredLeft = false;
    private float gravityMultiplier = 1;
    private bool facingRight = true;

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

        bool crouching = Input.GetButton("Crouch") && onGround && reloadTimer <= 0;
        Animator.SetBool("Crouching", crouching);
        if (crouching) horizontalAxis = 0;

        
        if (onGround)
        {
            currentJumps = MaxJumps;
        }

        if (!crouching && Input.GetButtonPress("Jump") && reloadTimer <= 0 && currentJumps > 0)
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

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        gravityMultiplier = 1;

        gunSlowdownTimer -= Time.deltaTime;
        if (gunSlowdownTimer > 0)
        {
            horizontalAxis *= (onGround ? 0.8f : 1f);
        }

        if (shotsFired < 18)
        {
            Vector3 facingDirection = (facingRight ? Vector3.right : Vector3.left);
            if (Input.GetButton("Attack") && !lastFiredLeft && cooldownTimer <= 0)
            {
                Animator.SetTrigger("Left Gun Fire");
                this.StartCoroutine(FireGun(LeftGunShotOrigin, transform.TransformDirection(facingDirection)));
                lastFiredLeft = true;

            }
            else if (!Input.GetButton("Attack") && lastFiredLeft && cooldownTimer <= 0)
            {
                Animator.SetTrigger("Right Gun Fire");
                this.StartCoroutine(FireGun(RightGunShotOrigin, transform.TransformDirection(facingDirection)));
                lastFiredLeft = false;
            }
        }
        else if (onGround && reloadTimer <= 0)
        {
            reloadTimer = ReloadTime;
            BulletDisplay.StartReload(18, ReloadTime);
            Audio.PlayOneShot(ReloadSound);
        }
        else if (reloadTimer <= 0 && Input.GetButtonPress("Attack") || Input.GetButtonRelease("Attack"))
        {
            //Audio.PlayOneShot(EmptyGunSound);
        }

        if (reloadTimer > 0 && onGround)
        {
            Animator.SetBool("Reloading", true);
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                shotsFired = 0;
                Animator.SetBool("Reloading", false);
            }
            horizontalAxis = 0;
        }
        else if (reloadTimer > 0 && !onGround)
        {
            Animator.SetBool("Reloading", false);
            reloadTimer = ReloadTime;
        }

        if (horizontalAxis > 0 && gunSlowdownTimer <= 0)
        {
            Animator.SetBool("Facing Right", true);
            facingRight = true;
        }
        else if (horizontalAxis < 0 && gunSlowdownTimer <= 0)
        {
            Animator.SetBool("Facing Right", false);
            facingRight = false;
        }

        Animator.SetFloat("Horizontal Speed", horizontalAxis);

        if (reloadTimer <= 0)
        {
            BulletDisplay.SetBulletCount(18 - shotsFired, 18);
        }
    }

    private int shotsFired;

    private IEnumerator FireGun(Transform origin, Vector3 direction)
    {
        cooldownTimer = ShotCooldown;
        gunSlowdownTimer = GunSlowdownTime;
        yield return new WaitForSeconds(0.01f);
        Vector3 position = origin.position;
        shotsFired++;
        Audio.PlayOneShot(GunSound, 0.7f);
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        LineRenderer line = (GameObject.Instantiate(BulletTrail) as GameObject).GetComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.SetPosition(0, position);
        line.useWorldSpace = true;
        if (Physics.Raycast(ray, out hit, 30f, GunShotLayer))
        {
            Component h = (Component)(hit.rigidbody) ?? (Component)(hit.collider);
            Shootable s = h.GetComponent<Shootable>();
            if (s != null)
            {
                s.TakeShot(hit, this);
                line.SetPosition(1, hit.point);
                yield break;
            }
        }
        line.SetPosition(1, position + (30f * direction));
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
