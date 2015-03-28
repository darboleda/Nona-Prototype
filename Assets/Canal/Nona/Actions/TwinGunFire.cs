using UnityEngine;
using System.Collections;

using Zenject;

public class TwinGunFire : ActionBehavior {

    [Inject]
    public BulletMeter BulletDisplay { get; set; }

    private NonaController controller;

    public AudioClip GunSound;
    public AudioClip ReloadSound;
    public AudioClip EmptyGunSound;
    public GameObject BulletTrail;
    
    public Transform LeftGunShotOrigin;
    public Transform RightGunShotOrigin;
    public float ShotCooldown = 0.1f;
    public float GunSlowdownTime = 0.5f;
    public float ReloadTime = 0.5f;
    public int MaxBullets;

    private int shotsFired;
    private float reloadTimer;

    public LayerMask GunShotLayer;

    private float gunSlowdownTimer;
    private float cooldownTimer;
    private bool lastFiredLeft = false;

    private bool facingRight { get { return controller.facingRight; } }
    private Animator Animator { get { return controller.Animator; } }
    private bool onGround { get { return controller.onGround; } }
    private AudioSource Audio { get { return controller.Audio; } }
    private INonaInput<string> Input { get { return controller.Input; } }
    private float horizontalAxis { get { return controller.horizontalAxis; } set { controller.horizontalAxis = value; } }

	public void Awake()
    {
        controller = GetComponentInParent<NonaController>();
        controller.actions.Add(this);
    }

    public override void Tick()
    {
        gunSlowdownTimer -= Time.deltaTime;
        controller.InFocusMode = gunSlowdownTimer > 0; 

        if (shotsFired < MaxBullets)
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
            BulletDisplay.StartReload(MaxBullets, ReloadTime);
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

        }
        else if (reloadTimer > 0 && !onGround)
        {
            Animator.SetBool("Reloading", false);
            reloadTimer = ReloadTime;
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (reloadTimer <= 0)
        {
            BulletDisplay.SetBulletCount(MaxBullets - shotsFired, MaxBullets);
        }

        controller.DisableInput = reloadTimer > 0;
    }

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
}
