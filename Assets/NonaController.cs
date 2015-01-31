using UnityEngine;
using System.Collections;

public class NonaController : MonoBehaviour
{
    public float JumpHeight = 2.5f;
    public float WalkSpeed = 4f;
    public float WalkAcceleration = 10f;
    public float WalkFriction = 10f;
    public float Gravity = 20f;

    public LayerMask SolidFloorLayer;
    public LayerMask GunShotLayer;

    public Animator Animator;
    public AudioSource Audio;

    public AudioClip GunSound;
    public GameObject BulletTrail;

    public Transform LeftGunShotOrigin;
    public Transform RightGunShotOrigin;
    public float ShotCooldown = 0.1f;
    public float GunSlowdownTime = 0.5f;
    public float ReloadTime = 0.5f;

    new private Transform transform;

    private Vector2 velocity;
    private float horizontalAxis;
    private bool onGround;

    private float cooldownTimer = 0;
    private float gunSlowdownTimer = 0;
    private float reloadTimer = 0;
    private bool lastFiredLeft = false;
    private float gravityMultiplier = 1;
    private bool facingRight = true;

    private float jumpVelocityOnFire;

    public UnityEngine.UI.Text ShotsFiredDisplay;

    public void Awake()
    {
        this.transform = base.transform;
        Animator.SetBool("Facing Right", true);
        facingRight = true;
    }

    public void Update()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        horizontalAxis = (horizontalAxis > 0.5f ? 1 : horizontalAxis < -0.5f ? -1 : 0);

        bool crouching = Input.GetAxisRaw("Vertical") < -0.5f && onGround && reloadTimer <= 0;
        Animator.SetBool("Crouching", crouching);
        if (crouching) horizontalAxis = 0;

        if (!crouching && Input.GetButtonDown("Jump") && reloadTimer <= 0)
        {
            velocity.y = Mathf.Sqrt(2 * Gravity * JumpHeight);
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
            horizontalAxis *= (onGround ? 0.8f : 0.3f);
            gravityMultiplier = 0.3f;
        }

        if (shotsFired < 18)
        {
            if (Input.GetButton("Fire1") && !lastFiredLeft && cooldownTimer <= 0)
            {
                Animator.SetTrigger("Left Gun Fire");
                FireGun(LeftGunShotOrigin.position, transform.TransformDirection(Vector3.right));
                lastFiredLeft = true;

            }
            else if (!Input.GetButton("Fire1") && lastFiredLeft && cooldownTimer <= 0)
            {
                Animator.SetTrigger("Right Gun Fire");
                FireGun(RightGunShotOrigin.position, transform.TransformDirection(Vector3.right));
                lastFiredLeft = false;
            }
        }
        else if (onGround && reloadTimer <= 0)
        {
            reloadTimer = ReloadTime;
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

        if (reloadTimer > 0)
        {
            ShotsFiredDisplay.text = string.Format("Reloading... {0:0.0}s", reloadTimer);
        }
        else
        {
            ShotsFiredDisplay.text = string.Format("Bullets: {0}", 18 - shotsFired);
        }
    }

    private int shotsFired;

    private void FireGun(Vector3 position, Vector3 direction)
    {
        cooldownTimer = ShotCooldown;
        gunSlowdownTimer = GunSlowdownTime;
        shotsFired++;
        if (!onGround)
        {
            if (velocity.y > 0)
                velocity.y = 0;
            else
                velocity.y *= 0.5f;
            velocity.x = 3f * (facingRight ? -1 : 1);
        }
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
                return;
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
        RaycastHit hit;
        Ray ray = new Ray(transform.position + desiredDelta - Vector3.down * 0.5f, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (velocity.y <= 0 && Physics.Raycast(ray, out hit, 1f, SolidFloorLayer))
        {
            Vector3 finalDelta = hit.point - transform.position;
            if (finalDelta.y <= 0 && desiredDelta.y <= finalDelta.y || Mathf.Abs(desiredDelta.y - finalDelta.y) < 0.01f)
            {
                onGround = true;
                return finalDelta;
            }
        }
        onGround = false;
        return desiredDelta;
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
